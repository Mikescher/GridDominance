using GridDominance.Levelformat.Parser;
using Microsoft.Xna.Framework.Content.Pipeline;
using System;

namespace GridDominance.Content.Pipeline
{
	[ContentProcessor(DisplayName = "GridDominance Level Processor")]
	public class GDLevelProcessor : ContentProcessor<string, LevelFile>
	{
		public override LevelFile Process(string input, ContentProcessorContext context)
		{
			var lf = new LevelFile(input);

			lf.Parse();
			
			Console.WriteLine("Parsing file with " + lf.BlueprintCannons.Count + " cannon definitions");

			return lf;
		}
	}
}
