using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.Persistance;
using MonoSAMFramework.Portable.Persistance.DataFile;
using MonoSAMFramework.Portable.Persistance.DataFile.PrimitiveWrapper;

namespace GridDominance.Shared.SaveData
{
	public class PlayerProfile : RootDataFile
	{
		protected override SemVersion ArchiveVersion => new SemVersion(1, 0, 0);

		public int TotalPoints => LevelData.Sum(p => p.Value.TotalPoints);

		public Dictionary<Guid, PlayerProfileLevelData> LevelData;

		public int OnlineUserID;
		public string OnlineUsername;
		public string OnlinePasswordHash;
		public int OnlineRevisionID;
		public bool NeedsReupload;

		public bool SoundsEnabled;
		public bool EffectsEnabled;

		public PlayerProfile()
		{
			LevelData = new Dictionary<Guid, PlayerProfileLevelData>();

			OnlineUserID       = -1;
			OnlineUsername     = "anonymous";
			OnlinePasswordHash = string.Empty;
			OnlineRevisionID   = -1;
			NeedsReupload      = false;

			SoundsEnabled = true;
			EffectsEnabled = true;
		}

		public PlayerProfileLevelData GetLevelData(Guid levelid)
		{
			if (!LevelData.ContainsKey(levelid))
				LevelData[levelid] = new PlayerProfileLevelData();

			return LevelData[levelid];
		}

		public void SetCompleted(Guid levelid, FractionDifficulty d, int time, bool upload)
		{
			GetLevelData(levelid).SetBestTime(d, time);
			
			if (upload && OnlineUserID >= 0)
			{
				MainGame.Inst.Backend.SetScore(this, levelid, d, time).EnsureNoError();
			}
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new PlayerProfile());

			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "userid",    o => o.OnlineUserID,       (o, v) => o.OnlineUserID       = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "user",      o => o.OnlineUsername,     (o, v) => o.OnlineUsername     = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "pass",      o => o.OnlinePasswordHash, (o, v) => o.OnlinePasswordHash = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "revid",     o => o.OnlineRevisionID,   (o, v) => o.OnlineRevisionID   = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "uploaderr", o => o.NeedsReupload,      (o, v) => o.NeedsReupload      = v);

			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "sounds",    o => o.SoundsEnabled,      (o, v) => o.SoundsEnabled      = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "effect",    o => o.EffectsEnabled,     (o, v) => o.EffectsEnabled     = v);

			RegisterPropertyGuidictionary<PlayerProfile, PlayerProfileLevelData>(SemVersion.VERSION_1_0_0, "progress", () => new PlayerProfileLevelData(),  o => o.LevelData, (o, v) => o.LevelData = v);
		}

		protected override string GetTypeName()
		{
			return "PLAYER_PROFILE_DATA";
		}
	}
}
