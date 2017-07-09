<Query Kind="Program" />

string PATH = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), @"..\Source\");

Regex REX = new Regex(@"SAMLog\.(Error|FatalError|Info)\(""([^""]+)""");

void Main()
{
	EnumerateErrorIDs()
		.GroupBy(p => p.Item1)
		.Where(p => p.Count() > 1)
		.Dump();
		
	EnumerateErrorIDs()
		.Dump();
}

IEnumerable<(string, string, int, string)> EnumerateErrorIDs()
{
	foreach (string file in Directory.EnumerateFiles(PATH, "*.cs", SearchOption.AllDirectories))
	{
		int ln = 0;
		foreach (var line in File.ReadAllLines(file))
		{
			ln++;
			
			var m = REX.Match(line);
			if (m.Success)
			{
				yield return (m.Groups[2].Value, m.Groups[1].Value, ln, file.Replace(PATH, ""));
			}
		}
	}
}
