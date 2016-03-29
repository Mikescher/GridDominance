using GridDominance.Levelformat.Parser;
using Microsoft.Xna.Framework.Content;

namespace GridDominance.Shared.Resources
{
	public static class Levels
	{
		public static LevelFile LEVEL_003;

		public static void LoadContent(ContentManager content)
		{
			LEVEL_003 = content.Load<LevelFile>("levels/lvl003");
		}
	}
}
