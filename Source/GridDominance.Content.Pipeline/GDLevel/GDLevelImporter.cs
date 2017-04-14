using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace GridDominance.Content.Pipeline.GDLevel
{
	[ContentImporter(".gslevel", DefaultProcessor = "GDLevelProcessor", DisplayName = "GridDominance Level Importer")]
	public class GDLevelImporter : ContentImporter<LevelPackage>
	{
		public override LevelPackage Import(string filename, ContentImporterContext context)
		{
			var content = File.ReadAllText(filename, Encoding.UTF8);

			Console.WriteLine("Reading {0} character long file", content.Length);

			var path = Path.GetDirectoryName(filename) ?? "";
			var pattern = "*.gsheader";

			var includes = Directory.EnumerateFiles(path, pattern).ToDictionary(p => Path.GetFileName(p) ?? p, p => File.ReadAllText(p, Encoding.UTF8));
				
			Console.WriteLine("Reading {0} possible include files with a total of {1} characters", includes.Count, includes.Sum(p => p.Value.Length));

			return new LevelPackage
			{
				Content = content,
				Includes = includes,
			};
		}
	}
}
