<Query Kind="Program" />

string PATH = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), @"..\Source\GridDominance.Shared\Resources\L10NImpl.cs");
void Main()
{
	var lines = File.ReadAllLines(PATH);
	for (int i = 0; i < lines.Length; i++)
	{
		var split = GrammarSplit(lines[i], ',');
		if (split.Length > 2 && split[0].Trim().ToLower().StartsWith("l10n.add(")) Fmt(ref lines, ref i, split.Length);
	}
	File.WriteAllLines(PATH, lines);
}

void Fmt(ref string[] lines, ref int i, int colcount)
{
	int start = i;
	int[] lengths = new int[colcount];
	while (i < lines.Length && GrammarSplit(lines[i], ',').Length == colcount)
	{
		var line = lines[i];
		var spl = GrammarSplit(line, ',');
		for (int j = 0; j < colcount; j++) lengths[j] = Math.Max(lengths[j], TTrim(j, colcount, spl[j]).Length);
		i++;
	}
	int end = i;
	for (int j = start; j < end; j++) 
	{
		var line = lines[j];
		var spl = GrammarSplit(line, ',');
		for (int k = 0; k < colcount-1; k++) spl[k] = (TTrim(k, colcount, spl[k]) + ", ").PadRight(lengths[k]+2);
		spl[colcount-1] = TTrim(colcount-1, colcount, spl[colcount-1]);
		lines[j] = string.Join("", spl);
	}
	$"{(end - start)} x {colcount}".Dump();
}

string TTrim(int i, int m, string s) {
	if (i > 0) s = s.TrimStart();
	if (i+1 < m) s = s.TrimEnd();
	return s;
}

string[] GrammarSplit(string str, char sp)
{
	StringBuilder b = new StringBuilder();
	List<string> r = new List<string>();

	bool instr = false;
	bool escape = false;
	foreach (var chr in str)
	{
		if (escape) { b.Append(chr); escape = false; continue; }
		if (chr == '"') { b.Append(chr); instr = !instr; continue; }
		if (chr == '\\') { b.Append(chr); escape = true; continue; }
		if (!instr && chr == sp) 
		{
			r.Add(b.ToString());
			b.Clear();
			continue;
		}
		
		b.Append(chr);
	}
	if (b.Length > 0) r.Add(b.ToString());
	return r.ToArray();
}