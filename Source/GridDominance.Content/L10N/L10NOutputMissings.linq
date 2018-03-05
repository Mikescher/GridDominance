<Query Kind="Program">
  <Connection>
    <ID>951bc30a-f75f-46a5-b7fa-91398aee62e1</ID>
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

private string OUT  = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), @"missing_{0}.txt");

private List<languages> langs;

void Main()
{
	langs = languages.ToList();
	var txs = texts.ToList();
	
	foreach (var lang in langs)
	{
		var missing = txs.Where(t => GetPropValue(t, lang.identifier).Trim() == "?").ToList();
		
		if (missing.Any()) Output(missing, string.Format(OUT, lang.name));
	}
}

void Output(List<texts> txt, string filename)
{
	var b = new StringBuilder();
	foreach (var text in txt)
	{
		foreach (var lang in langs)
		{
			b.AppendLine($"[{lang.languagecode}]  " + GetPropValue(text, lang.identifier).Replace("\r", "").Replace("\n", "\\n"));
		}
		b.AppendLine();
		b.AppendLine("(* " + (text.description??"") + " *)");
		b.AppendLine();
		b.AppendLine();
	}

	File.WriteAllText(filename, b.ToString());
	$"Output {filename}".Dump();
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