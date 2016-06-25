using GridDominance.Levelformat.Parser;
using Microsoft.Xna.Framework.Content;

// ReSharper disable NotAccessedField.Global
namespace GridDominance.Shared.Resources
{
	public static class Levels
	{
		public static LevelFile LEVEL_DBG;
		public static LevelFile LEVEL_003;

		public static void LoadContent(ContentManager content)
		{
			LEVEL_003 = content.Load<LevelFile>("levels/lvl003");
			LEVEL_DBG = content.Load<LevelFile>("levels/lvl_debug");
		}
	}
}
