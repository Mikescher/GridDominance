<Query Kind="Program" />

readonly string[] Files = new[]
{
	@"..\..\Source\MonoSAMFramework.Portable\ColorHelper\FlatColors.cs",
	@"..\..\Source\MonoSAMFramework.Portable\ColorHelper\ColorblindColors.cs",
};

void Main()
{
	Files.ToList().ForEach(Process);
}


void Process(string file)
{
	var path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), file);

	Regex rex = new Regex(@"^\s*public\s+static\s+readonly\s+(Microsoft\.Xna\.Framework\.)?Color\s+(?<name>[A-Za-z0-9]+)\s*=\s*new\s+(Microsoft\.Xna\.Framework.)?Color\(\s*(?<R>([0-9]+|(0x[A-Za-z0-9]+)))\s*,\s*(?<G>([0-9]+|(0x[A-Za-z0-9]+)))\s*,\s*(?<B>([0-9]+|(0x[A-Za-z0-9]+)))\s*\);.*$");

	var lines =File.ReadAllLines(path);

	int maxLen = 0;

	for (int i = 0; i < lines.Length; i++)
	{
		var match = rex.Match(lines[i]);
		if (match.Success)
		{
			maxLen = Math.Max(maxLen, match.Groups["name"].Value.Length);
		}
    }

	for (int i = 0; i < lines.Length; i++)
	{
		var match = rex.Match(lines[i]);
		if (match.Success)
		{
			var newlineFmt = "\t\tpublic static readonly Color {0} = new Color(0x{1:X2}, 0x{2:X2}, 0x{3:X2}); // #{1:X2}{2:X2}{3:X2} = ({1,3}, {2,3}, {3,3})";
			//var newlineFmt = "\t\tpublic static readonly Color {0} = new Color({1,3}, {2,3}, {3,3}); // #{1:X2}{2:X2}{3:X2} = ({1,3}, {2,3}, {3,3})";

			int r = match.Groups["R"].Value.StartsWith("0x") ? Convert.ToInt32(match.Groups["R"].Value.Substring(2), 16) : Convert.ToInt32(match.Groups["R"].Value, 10);
            int g = match.Groups["G"].Value.StartsWith("0x") ? Convert.ToInt32(match.Groups["G"].Value.Substring(2), 16) : Convert.ToInt32(match.Groups["G"].Value, 10);
            int b = match.Groups["B"].Value.StartsWith("0x") ? Convert.ToInt32(match.Groups["B"].Value.Substring(2), 16) : Convert.ToInt32(match.Groups["B"].Value, 10);
			
			var newline = string.Format(
				newlineFmt,
				match.Groups["name"].Value.PadRight(maxLen),
				r,
				g,
				b);

			if (lines[i] != newline)
			{
				lines[i].Dump();
				newline.Dump();
				"".Dump();

				lines[i] = newline;
			}
		}
	}
	
	File.WriteAllLines(path, lines);
}