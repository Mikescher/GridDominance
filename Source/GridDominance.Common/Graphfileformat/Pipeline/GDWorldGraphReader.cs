using GridDominance.Graphfileformat.Parser;
using Microsoft.Xna.Framework.Content;

namespace GridDominance.Graphfileformat.Pipeline
{
	public class GDWorldGraphReader : ContentTypeReader<WorldGraphFile>
	{
		protected override WorldGraphFile Read(ContentReader input, WorldGraphFile existingInstance)
		{
			var lf = new WorldGraphFile();

			lf.BinaryDeserialize(input);

			return lf;
		}
	}
}
