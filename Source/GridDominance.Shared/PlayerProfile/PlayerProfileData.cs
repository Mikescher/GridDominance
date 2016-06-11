using System;
using System.Collections.Generic;
using System.Text;
using MonoSAMFramework.Portable.FileHelper.DataFile;

namespace GridDominance.Shared.PlayerProfile
{
	public class PlayerProfileData : BaseDataFile
	{
		private Guid identifier;

		private Dictionary<string, PlayerProfileLevelData> levelData;

		private string onlineUsername;
		private string onlinePasswordHash;

		public PlayerProfileData()
		{
			identifier = Guid.NewGuid();
			levelData = new Dictionary<string, PlayerProfileLevelData>();

			onlineUsername     = string.Empty;
			onlinePasswordHash = string.Empty;
		}

		public PlayerProfileLevelData GetLevelData(string levelid)
		{
			if (!levelData.ContainsKey(levelid))
				levelData[levelid] = new PlayerProfileLevelData();

			return levelData[levelid];
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new PlayerProfileData());

			RegisterProperty<PlayerProfileData>("id", o => o.identifier, (o, v) => o.identifier = v);
			RegisterProperty<PlayerProfileData>("user", o => o.onlineUsername, (o, v) => o.onlineUsername = v);
			RegisterProperty<PlayerProfileData>("pass", o => o.onlinePasswordHash, (o, v) => o.onlinePasswordHash = v);

			RegisterPropertyStringDictionary<PlayerProfileData, PlayerProfileLevelData>("progress", () => new PlayerProfileLevelData(),  o => o.levelData, (o, v) => o.levelData = v);
		}

		protected override string GetTypeName()
		{
			return "PLAYER_PROFILE_DATA";
		}
	}
}
