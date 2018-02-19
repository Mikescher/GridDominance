using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using Microsoft.Xna.Framework.Content;
using MonoSAMFramework.Portable.LogProtocol;

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

		public static Dictionary<Guid, GraphBlueprint> WORLDS;
		public static Dictionary<Guid, LevelBlueprint> LEVELS;
		public static Dictionary<Guid, Guid> MAP_LEVELS_WORLDS; // LEVEL_ID --> WORLD_ID
		public static Dictionary<Guid, int> WORLD_NAMES;
		public static Dictionary<Guid, int> WORLD_NUMBERS;

		public static GraphBlueprint[] WORLDS_MULTIPLAYER;
		public static GraphBlueprint[] WORLDS_BY_NUMBER;

		public static Guid WORLD_ID_NONE        = new Guid("{d34db335-0001-4000-7711-000000100002}");
		public static Guid WORLD_ID_TUTORIAL    = new Guid("{d34db335-0001-4000-7711-000000100001}");
		public static Guid WORLD_ID_MULTIPLAYER = new Guid("{d34db335-0001-4000-7711-000000300001}");
		public static Guid WORLD_ID_GAMEEND     = new Guid("{d34db335-0001-4000-7711-000000100003}");
		public static Guid WORLD_ID_ONLINE      = new Guid("{d34db335-0001-4000-7711-000000300002}");

		public static Guid LEVELID_1_1 = new Guid("{b16b00b5-0001-4000-0000-000001000001}");
		public static Guid LEVELID_1_2 = new Guid("{b16b00b5-0001-4000-0000-000001000002}");
		public static Guid LEVELID_1_3 = new Guid("{b16b00b5-0001-4000-0000-000001000003}");

		public static void LoadContent(ContentManager content)
		{
			LEVELS            = new Dictionary<Guid, LevelBlueprint>();
			WORLDS            = new Dictionary<Guid, GraphBlueprint>();
			MAP_LEVELS_WORLDS = new Dictionary<Guid, Guid>();
			WORLD_NAMES       = new Dictionary<Guid, int>();
			WORLD_NUMBERS     = new Dictionary<Guid, int>();

			LEVEL_DBG      = LoadLevel(content, "levels/lvl_debug");
			LEVEL_TUTORIAL = LoadLevel(content, "levels/lvl_tutorial");

			LoadLevel(content, "levels/level_1-01");
			LoadLevel(content, "levels/level_1-02");
			LoadLevel(content, "levels/level_1-03");
			LoadLevel(content, "levels/level_1-04");
			LoadLevel(content, "levels/level_1-05");
			LoadLevel(content, "levels/level_1-06");
			LoadLevel(content, "levels/level_1-07");
			LoadLevel(content, "levels/level_1-08");
			LoadLevel(content, "levels/level_1-09");
			LoadLevel(content, "levels/level_1-10");
			LoadLevel(content, "levels/level_1-11");
			LoadLevel(content, "levels/level_1-12");
			LoadLevel(content, "levels/level_1-13");
			LoadLevel(content, "levels/level_1-14");

			LoadLevel(content, "levels/level_2-01");
			LoadLevel(content, "levels/level_2-02");
			LoadLevel(content, "levels/level_2-03");
			LoadLevel(content, "levels/level_2-04");
			LoadLevel(content, "levels/level_2-05");
			LoadLevel(content, "levels/level_2-06");
			LoadLevel(content, "levels/level_2-07");
			LoadLevel(content, "levels/level_2-08");
			LoadLevel(content, "levels/level_2-09");
			LoadLevel(content, "levels/level_2-10");
			LoadLevel(content, "levels/level_2-11");
			LoadLevel(content, "levels/level_2-12");
			LoadLevel(content, "levels/level_2-13");
			LoadLevel(content, "levels/level_2-14");
			LoadLevel(content, "levels/level_2-15");
			LoadLevel(content, "levels/level_2-16");
			LoadLevel(content, "levels/level_2-17");
			LoadLevel(content, "levels/level_2-18");
			LoadLevel(content, "levels/level_2-19");
			LoadLevel(content, "levels/level_2-20");
			LoadLevel(content, "levels/level_2-21");
			LoadLevel(content, "levels/level_2-22");
			LoadLevel(content, "levels/level_2-23");
			LoadLevel(content, "levels/level_2-24");
			LoadLevel(content, "levels/level_2-25");
			LoadLevel(content, "levels/level_2-26");
			LoadLevel(content, "levels/level_2-27");
			
			LoadLevel(content, "levels/level_3-01");
			LoadLevel(content, "levels/level_3-02");
			LoadLevel(content, "levels/level_3-03");
			LoadLevel(content, "levels/level_3-04");
			LoadLevel(content, "levels/level_3-05");
			LoadLevel(content, "levels/level_3-06");
			LoadLevel(content, "levels/level_3-07");
			LoadLevel(content, "levels/level_3-08");
			LoadLevel(content, "levels/level_3-09");
			LoadLevel(content, "levels/level_3-10");
			LoadLevel(content, "levels/level_3-11");
			LoadLevel(content, "levels/level_3-12");
			LoadLevel(content, "levels/level_3-13");
			LoadLevel(content, "levels/level_3-14");
			LoadLevel(content, "levels/level_3-15");
			LoadLevel(content, "levels/level_3-16");
			LoadLevel(content, "levels/level_3-17");
			LoadLevel(content, "levels/level_3-18");
			LoadLevel(content, "levels/level_3-19");
			LoadLevel(content, "levels/level_3-20");
			LoadLevel(content, "levels/level_3-21");
			LoadLevel(content, "levels/level_3-22");
			LoadLevel(content, "levels/level_3-23");
			LoadLevel(content, "levels/level_3-24");
			LoadLevel(content, "levels/level_3-25");
			LoadLevel(content, "levels/level_3-26");
			LoadLevel(content, "levels/level_3-27");
			LoadLevel(content, "levels/level_3-28");
			LoadLevel(content, "levels/level_3-29");
			LoadLevel(content, "levels/level_3-30");
			LoadLevel(content, "levels/level_3-31");

			LoadLevel(content, "levels/level_4-01");
			LoadLevel(content, "levels/level_4-02");
			LoadLevel(content, "levels/level_4-03");
			LoadLevel(content, "levels/level_4-04");
			LoadLevel(content, "levels/level_4-05");
			LoadLevel(content, "levels/level_4-06");
			LoadLevel(content, "levels/level_4-07");
			LoadLevel(content, "levels/level_4-08");
			LoadLevel(content, "levels/level_4-09");
			LoadLevel(content, "levels/level_4-10");
			LoadLevel(content, "levels/level_4-11");
			LoadLevel(content, "levels/level_4-12");
			LoadLevel(content, "levels/level_4-13");
			LoadLevel(content, "levels/level_4-14");
			LoadLevel(content, "levels/level_4-15");
			LoadLevel(content, "levels/level_4-16");
			LoadLevel(content, "levels/level_4-17");
			LoadLevel(content, "levels/level_4-18");
			LoadLevel(content, "levels/level_4-19");
			LoadLevel(content, "levels/level_4-20");
			LoadLevel(content, "levels/level_4-21");
			LoadLevel(content, "levels/level_4-22");
			LoadLevel(content, "levels/level_4-23");
			LoadLevel(content, "levels/level_4-24");
			LoadLevel(content, "levels/level_4-25");
			LoadLevel(content, "levels/level_4-26");
			LoadLevel(content, "levels/level_4-27");
			LoadLevel(content, "levels/level_4-28");
			LoadLevel(content, "levels/level_4-29");
			LoadLevel(content, "levels/level_4-30");
			LoadLevel(content, "levels/level_4-31");
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

			WORLDS_MULTIPLAYER = new[] {       WORLD_001, WORLD_002, WORLD_003, WORLD_004 };
			WORLDS_BY_NUMBER   = new[] { null, WORLD_001, WORLD_002, WORLD_003, WORLD_004 };

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

			foreach (var lvl in grph.LevelNodes) MAP_LEVELS_WORLDS[lvl.LevelID] = grph.ID;

			return grph;
		}

		private static void Test()
		{
			HashSet<Guid> ids = new HashSet<Guid>();
			HashSet<string> names = new HashSet<string>();
			HashSet<string> fnames = new HashSet<string>();

			foreach (var w in WORLDS.Select(w => w.Value))
			{
				foreach (var n in w.LevelNodes)
				{
					if (!LEVELS.ContainsKey(n.LevelID))
					{
						SAMLog.Error("ResourceTest-1", $"Could not find level with ID: {n.LevelID}");
					}
					else
					{
						if (!ids.Add(n.LevelID)) SAMLog.Error("ResourceTest-2", $"Duplicate LevelID: {n.LevelID}");
						if (!names.Add(LEVELS[n.LevelID].Name)) SAMLog.Error("ResourceTest-3", $"Duplicate LevelName: {LEVELS[n.LevelID].Name}");
						if (!fnames.Add(LEVELS[n.LevelID].FullName)) SAMLog.Error("ResourceTest-4", $"Duplicate LevelFullName: {LEVELS[n.LevelID].FullName}");

						foreach (var p in n.OutgoingPipes)
						{
							if (!LEVELS.ContainsKey(p.Target) && !WORLDS.ContainsKey(p.Target) && p.Target != WORLD_ID_GAMEEND) SAMLog.Error("ResourceTest-5", $"Could not find level with ID: {p.Target}");
						}
					}
				}

				foreach (var n in w.WarpNodes)
				{
					if (n.TargetWorld == WORLD_ID_GAMEEND) continue;

					if (!WORLDS.ContainsKey(n.TargetWorld))
					{
						SAMLog.Error("ResourceTest-6", $"Could not find world with ID: {n.TargetWorld}");
					}
				}
			}
		}

		public static Guid GetWorldByLevelID(Guid lid)
		{
			Guid wid;
			if (MAP_LEVELS_WORLDS.TryGetValue(lid, out wid)) return wid;
			return Guid.Empty;
		}
	}
}
