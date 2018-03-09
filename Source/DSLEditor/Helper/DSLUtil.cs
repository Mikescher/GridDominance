using System;
using System.IO;
using System.Linq;
using System.Text;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Levelfileformat;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Graphfileformat;
using System.Collections.Generic;
using GridDominance.Shared.SCCM.PreCalculation;

namespace GridDominance.DSLEditor.Helper
{
	public static class DSLUtil
	{
		public static List<Tuple<string, string>> LastParsedIncludedSources;

		public static LevelBlueprint ParseLevelFromFile(string path, bool sim)
		{
			var folder = Path.GetDirectoryName(path) ?? "";

			var includes = Directory.EnumerateFiles(folder, "*.gsheader").ToDictionary(p => Path.GetFileName(p) ?? p, p => File.ReadAllText(p, Encoding.UTF8));
			Func<string, string> includesFunc = x => includes.FirstOrDefault(p => LevelBlueprint.IsIncludeMatch(p.Key, x)).Value;

			var fp = new LevelParser(File.ReadAllText(path), includesFunc);
			var lf = fp.Parse(Path.GetFileName(path));
			LastParsedIncludedSources = fp.IncludedSources;

			if (sim) BlueprintPreprocessor.ProcessLevel(lf);

			return lf;
		}

		public static LevelBlueprint ParseLevelFromString(string text, Func<string, string> includesFunc, bool sim)
		{
			var fp = new LevelParser(text, includesFunc);
			var lf = fp.Parse();
			LastParsedIncludedSources = fp.IncludedSources;

			if (sim) BlueprintPreprocessor.ProcessLevel(lf);

			return lf;
		}

		public static GraphBlueprint ParseGraphFromFile(string f)
		{
			var path = Path.GetDirectoryName(f) ?? "";

			var includesFunc = GetIncludesFunc(f);

			var gp = new GraphParser(File.ReadAllText(f), includesFunc);
			var r = gp.Parse();
			LastParsedIncludedSources = gp.IncludedSources;

			return r;
		}

		public static GraphBlueprint ParseGraphFromString(string input, Func<string, string> includesFunc)
		{
			var gp = new GraphParser(input, includesFunc);
			var r = gp.Parse();
			LastParsedIncludedSources = gp.IncludedSources;

			return r;
		}

		public static Func<string, string> GetIncludesFunc(string filepath)
		{
			Func<string, string> includesFunc = x => null;
			if (File.Exists(filepath))
			{
				var path = Path.GetDirectoryName(filepath) ?? "";
				var pattern = "*.gsheader";

				var includes = Directory.EnumerateFiles(path, pattern).ToDictionary(p => Path.GetFileName(p) ?? p, p => File.ReadAllText(p, Encoding.UTF8));

				includesFunc = x => includes.FirstOrDefault(p => LevelBlueprint.IsIncludeMatch(p.Key, x)).Value;
			}
			return includesFunc;
		} 
	}
}
