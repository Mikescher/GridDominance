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
			var lf = new WorldGraphFile(input);

			lf.Parse();
			
			Console.WriteLine("Parsing file with " + lf.Nodes.Count + " node definitions");

			return lf;
		}
	}
}
