using GridDominance.Levelformat.Parser;
using Microsoft.Xna.Framework.Content;

namespace GridDominance.Levelfileformat.Pipeline
{
	public class GDLevelReader : ContentTypeReader<LevelFile>
	{
		protected override LevelFile Read(ContentReader input, LevelFile existingInstance)
		{
			var lf = new LevelFile();

			lf.BinaryDeserialize(input);

			return lf;
		}
	}
}
