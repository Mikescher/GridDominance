using GridDominance.Levelformat.Parser;
using Microsoft.Xna.Framework.Content;

// ReSharper disable NotAccessedField.Global
namespace GridDominance.Shared.Resources
{
	public static class Levels
	{
		public static LevelFile LEVEL_DBG;

		public static LevelFile LEVEL_001;
		public static LevelFile LEVEL_002;
		public static LevelFile LEVEL_003;

		public static void LoadContent(ContentManager content)
		{
			LEVEL_001 = content.Load<LevelFile>("levels/lvl001");
			LEVEL_002 = content.Load<LevelFile>("levels/lvl002");
			LEVEL_003 = content.Load<LevelFile>("levels/lvl003");
			LEVEL_DBG = content.Load<LevelFile>("levels/lvl_debug");
		}
	}
}
