using GridDominance.Graphfileformat.Parser;
using Microsoft.Xna.Framework.Content.Pipeline;
using System;

namespace GridDominance.Content.Pipeline.GDWorldGraph
{
	[ContentProcessor(DisplayName = "GridDominance WorldGraph Processor")]
	public class GDWorldGraphProcessor : ContentProcessor<string, WorldGraphFile>
	{
		public override WorldGraphFile Process(string input, ContentProcessorContext context)
		{
			var parser = new WorldGraphFileParser(input);

			var gf = parser.Parse();
			
			Console.WriteLine("Parsing file with " + gf.Nodes.Count + " node definitions");

			return gf;
		}
	}
}
