using GridDominance.Levelfileformat.Blueprint;
using Microsoft.Xna.Framework.Content;

namespace GridDominance.Levelfileformat.Pipeline
{
	public class GDLevelReader : ContentTypeReader<LevelBlueprint>
	{
		protected override LevelBlueprint Read(ContentReader input, LevelBlueprint existingInstance)
		{
			var lf = new LevelBlueprint();

			lf.BinaryDeserialize(input);

			return lf;
		}
	}
}
