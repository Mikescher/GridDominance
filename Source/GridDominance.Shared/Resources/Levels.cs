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

		public static LevelBlueprint LEVEL_TUTORIAL;
		public static LevelBlueprint LEVEL_DBG;
		public static LevelBlueprint LEVEL_1_1;

		public static Dictionary<Guid, GraphBlueprint> WORLDS;
		public static Dictionary<Guid, LevelBlueprint> LEVELS;
		public static Dictionary<Guid, int> WORLD_NAMES;

		public static void LoadContent(ContentManager content)
		{
			LEVELS      = new Dictionary<Guid, LevelBlueprint>();
			WORLDS      = new Dictionary<Guid, GraphBlueprint>();
			WORLD_NAMES = new Dictionary<Guid, int>();

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
			/* [MARK_LOAD_LEVEL] */

			WORLD_001 = LoadWorld(content, "levels/world_1");
			WORLD_002 = LoadWorld(content, "levels/world_2");
			WORLD_003 = LoadWorld(content, "levels/world_3");

			WORLD_NAMES[LEVEL_TUTORIAL.UniqueID] = L10NImpl.STR_WORLD_TUTORIAL;
			WORLD_NAMES[WORLD_001.ID]            = L10NImpl.STR_WORLD_W1;
			WORLD_NAMES[WORLD_002.ID]            = L10NImpl.STR_WORLD_W2;
			WORLD_NAMES[WORLD_003.ID]            = L10NImpl.STR_WORLD_W3;

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
