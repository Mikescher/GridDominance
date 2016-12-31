using System;
using System.Collections.Generic;
using MonoSAMFramework.Portable.Persistance;
using MonoSAMFramework.Portable.Persistance.DataFile;

namespace GridDominance.Shared.PlayerProfile
{
	public class PlayerProfile : RootDataFile
	{
		protected override SemVersion ArchiveVersion => new SemVersion(1, 0, 0);

		private Guid identifier;

		private Dictionary<string, PlayerProfileLevelData> levelData;

		private string onlineUsername;
		private string onlinePasswordHash;

		public bool SoundsEnabled;
		public bool EffectsEnabled;

		public PlayerProfile()
		{
			identifier = Guid.NewGuid();
			levelData = new Dictionary<string, PlayerProfileLevelData>();

			onlineUsername     = string.Empty;
			onlinePasswordHash = string.Empty;

			SoundsEnabled = true;
			EffectsEnabled = true;
		}

		public PlayerProfileLevelData GetLevelData(string levelid)
		{
			if (!levelData.ContainsKey(levelid))
				levelData[levelid] = new PlayerProfileLevelData();

			return levelData[levelid];
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new PlayerProfile());

			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "id", o => o.identifier, (o, v) => o.identifier = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "user", o => o.onlineUsername, (o, v) => o.onlineUsername = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "pass", o => o.onlinePasswordHash, (o, v) => o.onlinePasswordHash = v);

			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "sounds", o => o.SoundsEnabled, (o, v) => o.SoundsEnabled = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "effect", o => o.EffectsEnabled, (o, v) => o.EffectsEnabled = v);

			RegisterPropertyStringDictionary<PlayerProfile, PlayerProfileLevelData>(SemVersion.VERSION_1_0_0, "progress", () => new PlayerProfileLevelData(),  o => o.levelData, (o, v) => o.levelData = v);
		}

		protected override string GetTypeName()
		{
			return "PLAYER_PROFILE_DATA";
		}
	}
}
