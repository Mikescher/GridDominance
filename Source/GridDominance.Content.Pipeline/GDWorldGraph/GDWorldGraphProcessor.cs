using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Linq;
using GridDominance.Content.Pipeline.PreCalculation;
using GridDominance.Graphfileformat;
using GridDominance.Graphfileformat.Blueprint;

namespace GridDominance.Content.Pipeline.GDWorldGraph
{
	[ContentProcessor(DisplayName = "GridDominance WorldGraph Processor")]
	public class GDWorldGraphProcessor : ContentProcessor<GraphPackage, GraphBlueprint>
	{
		public override GraphBlueprint Process(GraphPackage input, ContentProcessorContext context)
		{
			var parser = new GraphParser(input.Content, x => input.Includes.FirstOrDefault(p => GraphBlueprint.IsIncludeMatch(p.Key, x)).Value);

			var gf = parser.Parse();

			BlueprintPreprocessor.ProcessGraph(gf);

			Console.WriteLine("Parsing file with " + gf.Nodes.Count + " node definitions");

			return gf;
		}
	}
}
