using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using GridDominance.Graphfileformat.Blueprint;

namespace GridDominance.Content.Pipeline.GDWorldGraph
{
	[ContentProcessor(DisplayName = "GridDominance WorldGraph Processor")]
	public class GDWorldGraphProcessor : ContentProcessor<string, GraphBlueprint>
	{
		public override GraphBlueprint Process(string input, ContentProcessorContext context)
		{
			var parser = new GraphParser(input);

			var gf = parser.Parse();
			
			Console.WriteLine("Parsing file with " + gf.Nodes.Count + " node definitions");

			return gf;
		}
	}
}
