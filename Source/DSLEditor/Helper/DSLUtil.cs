using System;
using System.IO;
using System.Linq;
using System.Text;
using GridDominance.Content.Pipeline.PreCalculation;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Levelfileformat;

namespace GridDominance.DSLEditor.Helper
{
	public static class DSLUtil
	{
		public static LevelBlueprint ParseLevelFromFile(string path, bool sim)
		{
			var folder = Path.GetDirectoryName(path) ?? "";

			var includes = Directory.EnumerateFiles(folder, "*.gsheader").ToDictionary(p => Path.GetFileName(p) ?? p, p => File.ReadAllText(p, Encoding.UTF8));
			Func<string, string> includesFunc = x => includes.FirstOrDefault(p => LevelBlueprint.IsIncludeMatch(p.Key, x)).Value;

			var fp = new LevelParser(File.ReadAllText(path), includesFunc);
			var lf = fp.Parse(Path.GetFileName(path));

			if (sim) BlueprintPreprocessor.ProcessLevel(lf);

			return lf;
		}

		public static LevelBlueprint ParseLevelFromString(string text, Func<string, string> includesFunc, bool sim)
		{
			var fp = new LevelParser(text, includesFunc);
			var lf = fp.Parse();

			if (sim) BlueprintPreprocessor.ProcessLevel(lf);

			return lf;
		}
	}
}
