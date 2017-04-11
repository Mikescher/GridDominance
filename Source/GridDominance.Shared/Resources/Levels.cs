using System;
using System.Collections.Generic;
using GridDominance.Graphfileformat.Parser;
using GridDominance.Levelformat.Parser;
using Microsoft.Xna.Framework.Content;

// ReSharper disable NotAccessedField.Global
namespace GridDominance.Shared.Resources
{
	public static class Levels
	{
		public static WorldGraphFile WORLD_001;

		public static Dictionary<Guid, LevelFile> LEVELS;

		public static LevelFile LEVEL_DBG;

		public static LevelFile LEVEL_001;
		public static LevelFile LEVEL_002;
		public static LevelFile LEVEL_003;

		public static void LoadContent(ContentManager content)
		{
			LEVELS = new Dictionary<Guid, LevelFile>();

			LoadLevel(content, "levels/lvl001");
			LoadLevel(content, "levels/lvl002");
			LoadLevel(content, "levels/lvl003");
			LoadLevel(content, "levels/lvl_debug"); //TODO Laggs like shit on handy with speed++

			WORLD_001 = content.Load<WorldGraphFile>("levels/world_1");
		}

		private static void LoadLevel(ContentManager content, string id)
		{
			var lvl = content.Load<LevelFile>(id);
			LEVELS[lvl.UniqueID] = lvl;
		}
	}
}
