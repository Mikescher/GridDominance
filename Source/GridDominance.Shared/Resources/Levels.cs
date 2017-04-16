using System;
using System.Collections.Generic;
using GridDominance.Graphfileformat.Parser;
using GridDominance.Levelfileformat.Parser;
using GridDominance.Levelformat.Parser;
using Microsoft.Xna.Framework.Content;
using MonoSAMFramework.Portable.LogProtocol;

// ReSharper disable NotAccessedField.Global
namespace GridDominance.Shared.Resources
{
	public static class Levels
	{
		public static WorldGraphFile WORLD_001;

		public static List<WorldGraphFile> WORLDS;
		public static Dictionary<Guid, LevelFile> LEVELS;

		public static LevelFile LEVEL_DBG;

		public static void LoadContent(ContentManager content)
		{
			LEVELS = new Dictionary<Guid, LevelFile>();
			WORLDS = new List<WorldGraphFile>();

			LEVEL_DBG = LoadLevel(content, "levels/lvl_debug");

			LoadLevel(content, "levels/lvl001");
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

			WORLD_001 = LoadWorld(content, "levels/world_1");

#if DEBUG
			Test();
#endif
		}

		private static LevelFile LoadLevel(ContentManager content, string id)
		{
			var lvl = content.Load<LevelFile>(id);
			LEVELS[lvl.UniqueID] = lvl;
			return lvl;
		}

		private static WorldGraphFile LoadWorld(ContentManager content, string id)
		{
			var grph = content.Load<WorldGraphFile>(id);
			WORLDS.Add(grph);
			return grph;
		}

		private static void Test()
		{
			HashSet<Guid> ids = new HashSet<Guid>();
			HashSet<string> names = new HashSet<string>();
			HashSet<string> fnames = new HashSet<string>();

			foreach (var w in WORLDS)
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
							if (!LEVELS.ContainsKey(p.Target)) SAMLog.Error("ResourceTest", $"Could not find level with ID: {p.Target}");
						}
					}
				}
			}
		}
	}
}
