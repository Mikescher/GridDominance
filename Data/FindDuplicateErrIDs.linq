<Query Kind="Program" />

string PATH = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), @"..\Source\");

Regex REX1 = new Regex(@"SAMLog\.(Error|FatalError|Info)\(""([^""]+)""");
Regex REX2 = new Regex(@"\.Show(Toast)\(""([^""]+)""");

void Main()
{
	EnumerateErrorIDs()
		.GroupBy(p => p.Item1)
		.Where(p => p.Count() > 1)
		.Dump();

	EnumerateErrorIDs()
		.GroupBy(p => p.Item2)
		.ToList()
		.ForEach(p => p.ToList().Dump());
}

IEnumerable<(string, string, int, string)> EnumerateErrorIDs()
{
	foreach (string file in Directory.EnumerateFiles(PATH, "*.cs", SearchOption.AllDirectories))
	{
		int ln = 0;
		foreach (var line in File.ReadAllLines(file))
		{
			ln++;
			
			var m1 = REX1.Match(line);
			if (m1.Success)
			{
				yield return (m1.Groups[2].Value, m1.Groups[1].Value, ln, file.Replace(PATH, ""));
			}

			var m2 = REX2.Match(line);
			if (m2.Success)
			{
				yield return (m2.Groups[2].Value, m2.Groups[1].Value, ln, file.Replace(PATH, ""));
			}
		}
	}
}