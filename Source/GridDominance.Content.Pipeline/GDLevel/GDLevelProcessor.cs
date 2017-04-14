using GridDominance.Levelfileformat.Parser;
using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Linq;

namespace GridDominance.Content.Pipeline.GDLevel
{
	[ContentProcessor(DisplayName = "GridDominance Level Processor")]
	public class GDLevelProcessor : ContentProcessor<LevelPackage, LevelFile>
	{
		public override LevelFile Process(LevelPackage input, ContentProcessorContext context)
		{
			var parser = new LevelFileParser(input.Content, x => input.Includes.FirstOrDefault(p => LevelFile.IsIncludeMatch(p.Key, x)).Value);

			var lf = parser.Parse();
			
			Console.WriteLine("Parsing file with " + lf.BlueprintCannons.Count + " cannon definitions");
			Console.WriteLine("Parsing file with " + lf.BlueprintVoidWalls.Count + " voidwall definitions");

			return lf;
		}
	}
}
