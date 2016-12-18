using GridDominance.Levelformat.Parser;
using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Linq;

namespace GridDominance.Content.Pipeline
{
	[ContentProcessor(DisplayName = "GridDominance Level Processor")]
	public class GDLevelProcessor : ContentProcessor<LevelPackage, LevelFile>
	{
		public override LevelFile Process(LevelPackage input, ContentProcessorContext context)
		{
			var lf = new LevelFile(input.Content, x => input.Includes.FirstOrDefault(p => LevelFile.IsIncludeMatch(p.Key, x)).Value);

			lf.Parse();
			
			Console.WriteLine("Parsing file with " + lf.BlueprintCannons.Count + " cannon definitions");

			return lf;
		}
	}
}
