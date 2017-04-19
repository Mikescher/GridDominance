using GridDominance.Graphfileformat.Blueprint;
using Microsoft.Xna.Framework.Content;

namespace GridDominance.Graphfileformat.Pipeline
{
	public class GDWorldGraphReader : ContentTypeReader<GraphBlueprint>
	{
		protected override GraphBlueprint Read(ContentReader input, GraphBlueprint existingInstance)
		{
			var lf = new GraphBlueprint();

			lf.BinaryDeserialize(input);

			return lf;
		}
	}
}
