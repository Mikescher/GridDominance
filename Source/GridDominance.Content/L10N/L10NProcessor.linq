<Query Kind="Program">
  <Connection>
    <ID>951bc30a-f75f-46a5-b7fa-91398aee62e1</ID>
    <Persist>true</Persist>
    <Driver Assembly="linq2db.LINQPad" PublicKeyToken="f19f8aed7feff67e">LinqToDB.LINQPad.LinqToDBDriver</Driver>
    <CustomCxString>Data Source=L10N.sqlite3;</CustomCxString>
    <ExcludeRoutines>true</ExcludeRoutines>
    <DisplayName>L10nTexts</DisplayName>
    <Provider>System.Data.SQLite</Provider>
    <Server>L10N</Server>
    <Database>main</Database>
    <DbVersion>3.19.3</DbVersion>
    <NoPluralization>true</NoPluralization>
    <NoCapitalization>true</NoCapitalization>
    <DriverData>
      <providerName>SQLite</providerName>
      <optimizeJoins>true</optimizeJoins>
      <allowMultipleQuery>false</allowMultipleQuery>
      <commandTimeout>0</commandTimeout>
    </DriverData>
  </Connection>
</Query>

private const string SOURCEDIR  = @"C:\Users\schwoerm\AppData\Local\M\gd\Source\GridDominance.Core\";
private const string SOURCEFILE = @"C:\Users\schwoerm\AppData\Local\M\gd\Source\GridDominance.Core\Resources\L10NImpl.cs";
private const string NAMESPACE = "GridDominance.Shared.Resources";

private readonly Regex REX_USAGE = new Regex(@"L10NImpl\.STR_(?<textid>[A-Z_0-9]+)");

void Main()
{
	UpdateUsages();
	
	CreateSourcefile();
}

void UpdateUsages()
{
	var ids = texts.Select(t => t.id).ToList();
	
	var missing = usages.Select(u => Tuple.Create(u.sourcepath + ":" + u.sourcelinenumber, u)).ToList();
	
	foreach (var file in EnumerateFilesDeep(SOURCEDIR).Where(f => f.ToLower().EndsWith(".cs")))
	{
		var lines = File.ReadAllLines(file);
		for (int i = 0; i < lines.Length; i++)
		{
			foreach (Match m in REX_USAGE.Matches(lines[i]))
			{
				var newsourcefile = Path.GetFileName(file);
				var newsourcepath = file.Substring(SOURCEDIR.Length);
				var newsourceline = i;
				var newtextid = m.Groups["textid"].Value;
				
				if (!ids.Contains(newtextid))
				{
					$"[ERROR] Unknown TextID in {newsourcefile}:{newsourceline}".Dump();
					continue;
				}
				
				var ident = newsourcepath + ":" + newsourceline;
				
				if (missing.Any(mx => mx.Item1 == ident))
				{
					missing.Remove(missing.First(mx => mx.Item1 == ident));
				}
				else
				{
					usages.Insert(() => new usages { textid=newtextid, sourcefile = newsourcefile, sourcepath = newsourcepath, sourcelinenumber = newsourceline });

					$"[+] {newsourcefile}:{newsourceline}".Dump();
				}
				
			}
		}
	}

	foreach (var miss in missing)
	{
		$"[-] {miss.Item1}".Dump();
		this.Delete(miss.Item2);
	}
}

void CreateSourcefile()
{
	var langs = languages.ToList();
	var texts = summary.ToList();
	
	var maxlen = summary.Max(s => s.id.Length+4);
	
	StringBuilder b = new StringBuilder();
	b.AppendLine("using MonoSAMFramework.Portable.Localization;");
	b.AppendLine("using MonoSAMFramework.Portable.LogProtocol;");
	b.AppendLine("");
	b.AppendLine("namespace " + NAMESPACE);
	b.AppendLine("{");
	b.AppendLine("\tpublic static class L10NImpl");
	b.AppendLine("\t{");
	for (int i = 0; i < langs.Count; i++)
	{
		b.AppendLine($"\t\tpublic const int LANG_{langs[i].languagecode.ToUpper().Replace('-', '_')} = {i};");
	}
	b.AppendLine();
	b.AppendLine($"\t\tpublic static int LANG_COUNT = {langs.Count};");
	b.AppendLine();
	b.AppendLine("//-----------------------------------------------------------------------");
	b.AppendLine();
	
	foreach (var grp in texts.GroupBy(t => t.id.Split('_').First()))
	{
		foreach (var txt in grp)
		{
			b.AppendLine($"\t\tpublic const int STR_{txt.id.PadRight(maxlen, ' ')} = {texts.IndexOf(txt)};");
		}
		b.AppendLine();
	}
	b.AppendLine($"\t\tprivate const int TEXT_COUNT = {texts.Count}; // = next idx");
	b.AppendLine();
	b.AppendLine($"\t\tpublic static void Init(int lang)");
	b.AppendLine("\t\t");
	b.AppendLine("\t\t\tL10N.Init(lang, TEXT_COUNT, LANG_COUNT);");
	b.AppendLine();
	b.AppendLine($"\t\t\t// {string.Join(" ", languages.Select(l => $"[{l.languagecode}]"))}");
	b.AppendLine();
	foreach (var txt in texts)
	{
		b.AppendLine($"\t\t\t// Description: {txt.description}");
		foreach (var usg in usages.Where(u => u.textid == txt.id))
		{
			b.AppendLine($"\t\t\t// Usage:       {usg.sourcefile}:{usg.sourcelinenumber}");
		}
		b.Append("\t\t\t");
		b.Append("L10N.Add(");
		b.Append("STR_" + txt.id);
		bool empty = false;
		foreach (var lang in languages)
		{
			var dat = GetPropValue(txt, lang.identifier)
				.Replace("\\", "\\\\")
				.Replace("\r", "")
				.Replace("\n", "\\n")
				.Replace("\"", "\\\"");

			if (dat == "?") empty = true;

			b.Append(", \"" + dat + "\"");
		}
		b.Append(");");
		if (empty) b.Append(" //TODO translate me");
		b.AppendLine();
		b.AppendLine();
	}
	b.AppendLine();
	b.AppendLine($"\t\t\t// {string.Join(" ", languages.Select(l => $"[{l.languagecode}]"))}");
	b.AppendLine();
	b.AppendLine("#if DEBUG");
	b.AppendLine("\t\t\tL10N.Verify();");
	b.AppendLine("#endif");
	b.AppendLine("\t\t}");
	b.AppendLine("\t}");
	b.AppendLine("}");

	File.WriteAllText(SOURCEFILE, b.ToString());
	$"[x] File {SOURCEFILE} updated".Dump();
}

//  ==================================

public static string GetPropValue(object src, string propName)
{
	return (string)src.GetType().GetProperty(propName).GetValue(src, null);
}

static IEnumerable<string> EnumerateFilesDeep(string path)
{
	Queue<string> queue = new Queue<string>();
	queue.Enqueue(path);
	while (queue.Count > 0)
	{
		path = queue.Dequeue();
		try
		{
			foreach (string subDir in Directory.GetDirectories(path))
			{
				queue.Enqueue(subDir);
			}
		}
		catch (Exception ex)
		{
			ex.Dump();
		}
		string[] files = null;
		try
		{
			files = Directory.GetFiles(path);
		}
		catch (Exception ex)
		{
			ex.Dump();
		}
		if (files != null)
		{
			for (int i = 0; i < files.Length; i++)
			{
				yield return files[i];
			}
		}
	}
}