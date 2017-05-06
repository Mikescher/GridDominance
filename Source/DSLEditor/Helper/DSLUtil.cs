using System;
using System.IO;
using System.Linq;
using System.Text;
using GridDominance.DSLEditor.PreCalculation;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Levelfileformat;

namespace GridDominance.DSLEditor.Helper
{
	public static class DSLUtil
	{
		public static LevelBlueprint ParseLevelFromFile(string path)
		{
			var folder = Path.GetDirectoryName(path) ?? "";

			var includes = Directory.EnumerateFiles(folder, "*.gsheader").ToDictionary(p => Path.GetFileName(p) ?? p, p => File.ReadAllText(p, Encoding.UTF8));
			Func<string, string> includesFunc = x => includes.FirstOrDefault(p => LevelBlueprint.IsIncludeMatch(p.Key, x)).Value;

			var fp = new LevelParser(File.ReadAllText(path), includesFunc);
			var lf = fp.Parse(Path.GetFileName(path));

			if (lf.KIType == LevelBlueprint.KI_TYPE_PRECALC) LevelBulletPathTracer.Precalc(lf);

			return lf;
		}

		public static LevelBlueprint ParseLevelFromString(string text, Func<string, string> includesFunc)
		{
			var fp = new LevelParser(text, includesFunc);
			var lf = fp.Parse();

			if (lf.KIType == LevelBlueprint.KI_TYPE_PRECALC) LevelBulletPathTracer.Precalc(lf);

			return lf;
		}
	}
}
