using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using Microsoft.Xna.Framework.Content;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Persistance.DataFile.PrimitiveWrapper;

// ReSharper disable NotAccessedField.Global
namespace GridDominance.Shared.Resources
{
	public static class Levels
	{
		public static GraphBlueprint WORLD_001;
		public static GraphBlueprint WORLD_002;
		public static GraphBlueprint WORLD_003;
		public static GraphBlueprint WORLD_004;

		public static LevelBlueprint LEVEL_TUTORIAL;
		public static LevelBlueprint LEVEL_DBG;
		public static LevelBlueprint LEVEL_1_1;

		public static Dictionary<Guid, GraphBlueprint> WORLDS;
		public static Dictionary<Guid, LevelBlueprint> LEVELS;
		public static Dictionary<Guid, int> WORLD_NAMES;
		public static Dictionary<Guid, int> WORLD_NUMBERS;

		public static Guid WORLD_ID_NONE        = new Guid("{d34db335-0001-4000-7711-000000100002}");
		public static Guid WORLD_ID_TUTORIAL    = new Guid("{d34db335-0001-4000-7711-000000100001}");
		public static Guid WORLD_ID_MULTIPLAYER = new Guid("{d34db335-0001-4000-7711-000000300001}");


		public static void LoadContent(ContentManager content)
		{
			LEVELS        = new Dictionary<Guid, LevelBlueprint>();
			WORLDS        = new Dictionary<Guid, GraphBlueprint>();
			WORLD_NAMES   = new Dictionary<Guid, int>();
			WORLD_NUMBERS = new Dictionary<Guid, int>();

			LEVEL_DBG      = LoadLevel(content, "levels/lvl_debug");
			LEVEL_TUTORIAL = LoadLevel(content, "levels/lvl_tutorial");

			LEVEL_1_1 = LoadLevel(content, "levels/lvl001");
			LoadLevel(content, "levels/lvl002");
			LoadLevel(content, "levels/lvl003");
			LoadLevel(content, "levels/lvl004");
			LoadLevel(content, "levels/lvl005");
			LoadLevel(content, "levels/lvl006");
			LoadLevel(content, "levels/lvl007");
			LoadLevel(content, "levels/lvl008");
			LoadLevel(content, "levels/lvl009");
			LoadLevel(content, "levels/lvl010");
			LoadLevel(content, "levels/lvl011");
			LoadLevel(content, "levels/lvl012");
			LoadLevel(content, "levels/lvl013");
			LoadLevel(content, "levels/lvl014");

			LoadLevel(content, "levels/lvl101");
			LoadLevel(content, "levels/lvl102");
			LoadLevel(content, "levels/lvl103");
			LoadLevel(content, "levels/lvl104");
			LoadLevel(content, "levels/lvl105");
			LoadLevel(content, "levels/lvl106");
			LoadLevel(content, "levels/lvl107");
			LoadLevel(content, "levels/lvl108");
			LoadLevel(content, "levels/lvl109");
			LoadLevel(content, "levels/lvl110");
			LoadLevel(content, "levels/lvl111");
			LoadLevel(content, "levels/lvl112");
			LoadLevel(content, "levels/lvl113");
			LoadLevel(content, "levels/lvl114");
			LoadLevel(content, "levels/lvl115");
			LoadLevel(content, "levels/lvl116");
			LoadLevel(content, "levels/lvl117");
			LoadLevel(content, "levels/lvl118");
			LoadLevel(content, "levels/lvl119");
			LoadLevel(content, "levels/lvl120");
			LoadLevel(content, "levels/lvl121");
			LoadLevel(content, "levels/lvl122");
			LoadLevel(content, "levels/lvl123");
			LoadLevel(content, "levels/lvl124");
			LoadLevel(content, "levels/lvl125");
			LoadLevel(content, "levels/lvl126");
			LoadLevel(content, "levels/lvl127");
			
			LoadLevel(content, "levels/lvl201");
			LoadLevel(content, "levels/lvl202");
			LoadLevel(content, "levels/lvl203");
			LoadLevel(content, "levels/lvl204");
			LoadLevel(content, "levels/lvl205");
			LoadLevel(content, "levels/lvl206");
			LoadLevel(content, "levels/lvl207");
			LoadLevel(content, "levels/lvl208");
			LoadLevel(content, "levels/lvl209");
			LoadLevel(content, "levels/lvl210");
			LoadLevel(content, "levels/lvl211");
			LoadLevel(content, "levels/lvl212");
			LoadLevel(content, "levels/lvl213");
			LoadLevel(content, "levels/lvl214");
			LoadLevel(content, "levels/lvl215");
			LoadLevel(content, "levels/lvl216");
			LoadLevel(content, "levels/lvl217");
			LoadLevel(content, "levels/lvl218");
			LoadLevel(content, "levels/lvl219");
			LoadLevel(content, "levels/lvl220");
			LoadLevel(content, "levels/lvl221");
			LoadLevel(content, "levels/lvl222");
			LoadLevel(content, "levels/lvl223");
			LoadLevel(content, "levels/lvl224");
			LoadLevel(content, "levels/lvl225");
			LoadLevel(content, "levels/lvl226");
			LoadLevel(content, "levels/lvl227");
			LoadLevel(content, "levels/lvl228");
			LoadLevel(content, "levels/lvl229");
			LoadLevel(content, "levels/lvl230");
			LoadLevel(content, "levels/lvl231");

			LoadLevel(content, "levels/lvl301");
			/* [MARK_LOAD_LEVEL] */

			WORLD_001 = LoadWorld(content, "levels/world_1");
			WORLD_002 = LoadWorld(content, "levels/world_2");
			WORLD_003 = LoadWorld(content, "levels/world_3");
			WORLD_004 = LoadWorld(content, "levels/world_4");

			WORLD_NAMES[LEVEL_TUTORIAL.UniqueID] = L10NImpl.STR_WORLD_TUTORIAL;
			WORLD_NAMES[WORLD_001.ID]            = L10NImpl.STR_WORLD_W1;
			WORLD_NAMES[WORLD_002.ID]            = L10NImpl.STR_WORLD_W2;
			WORLD_NAMES[WORLD_003.ID]            = L10NImpl.STR_WORLD_W3;
			WORLD_NAMES[WORLD_004.ID]            = L10NImpl.STR_WORLD_W4;

			WORLD_NUMBERS[WORLD_001.ID] = 0;
			WORLD_NUMBERS[WORLD_002.ID] = 1;
			WORLD_NUMBERS[WORLD_003.ID] = 2;
			WORLD_NUMBERS[WORLD_004.ID] = 3;

#if DEBUG
			Test();
#endif
		}

		private static LevelBlueprint LoadLevel(ContentManager content, string id)
		{
			var lvl = content.Load<LevelBlueprint>(id);
			LEVELS[lvl.UniqueID] = lvl;
			return lvl;
		}

		private static GraphBlueprint LoadWorld(ContentManager content, string id)
		{
			var grph = content.Load<GraphBlueprint>(id);
			WORLDS[grph.ID] = grph;
			return grph;
		}

		private static void Test()
		{
			HashSet<Guid> ids = new HashSet<Guid>();
			HashSet<string> names = new HashSet<string>();
			HashSet<string> fnames = new HashSet<string>();

			foreach (var w in WORLDS.Select(w => w.Value))
			{
				foreach (var n in w.Nodes)
				{
					if (!LEVELS.ContainsKey(n.LevelID))
					{
						SAMLog.Error("ResourceTest", $"Could not find level with ID: {n.LevelID}");
					}
					else
					{
						if (!ids.Add(n.LevelID)) SAMLog.Error("ResourceTest", $"Duplicate LevelID: {n.LevelID}");
						if (!names.Add(LEVELS[n.LevelID].Name)) SAMLog.Error("ResourceTest", $"Duplicate LevelName: {LEVELS[n.LevelID].Name}");
						if (!fnames.Add(LEVELS[n.LevelID].FullName)) SAMLog.Error("ResourceTest", $"Duplicate LevelFullName: {LEVELS[n.LevelID].FullName}");

						foreach (var p in n.OutgoingPipes)
						{
							if (!LEVELS.ContainsKey(p.Target) && !WORLDS.ContainsKey(p.Target)) SAMLog.Error("ResourceTest", $"Could not find level with ID: {p.Target}");
						}
					}
				}

				foreach (var n in w.WarpNodes)
				{
					if (!WORLDS.ContainsKey(n.TargetWorld))
					{
						SAMLog.Error("ResourceTest", $"Could not find world with ID: {n.TargetWorld}");
					}
				}
			}
		}
	}
}
