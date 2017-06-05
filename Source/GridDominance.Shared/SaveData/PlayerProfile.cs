using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.Persistance;
using MonoSAMFramework.Portable.Persistance.DataFile;
using MonoSAMFramework.Portable.Localization;
using GridDominance.Graphfileformat.Blueprint;

namespace GridDominance.Shared.SaveData
{
	public class PlayerProfile : RootDataFile
	{
		protected override SemVersion ArchiveVersion => new SemVersion(1, 0, 3);

		public int TotalPoints => LevelData.Sum(p => p.Value.TotalPoints);

		public Dictionary<Guid, LevelData> LevelData;

		public AccountType AccountType;
		public int OnlineUserID;
		public string OnlineUsername;
		public string OnlinePasswordHash;
		public int OnlineRevisionID;
		public bool NeedsReupload;

		public bool SoundsEnabled;
		public bool EffectsEnabled;

		public bool SkipTutorial;

		public int Language;

		public PlayerProfile()
		{
			InitEmpty();
		}

		public void InitEmpty()
		{
			LevelData = new Dictionary<Guid, LevelData>();

			AccountType = AccountType.Local;
			OnlineUserID = -1;
			OnlineUsername = "anonymous";
			OnlinePasswordHash = string.Empty;
			OnlineRevisionID = -1;
			NeedsReupload = false;

			SoundsEnabled = true;
			EffectsEnabled = true;

			SkipTutorial = false;

			Language = L10N.LANG_EN_US;
		}

		public LevelData GetLevelData(Guid levelid)
		{
			if (!LevelData.ContainsKey(levelid))
				LevelData[levelid] = new LevelData();

			return LevelData[levelid];
		}

		public LevelData GetLevelData(LevelBlueprint bp)
		{
			return GetLevelData(bp.UniqueID);
		}

		public LevelData GetLevelData(NodeBlueprint bp)
		{
			return GetLevelData(bp.LevelID);
		}

		public LevelDiffData GetLevelData(Guid levelid, FractionDifficulty d)
		{
			return GetLevelData(levelid).Data[d];
		}

		public void SetCompleted(Guid levelid, FractionDifficulty d, int time, bool upload)
		{
			GetLevelData(levelid).SetBestTime(d, time);

			if (upload && OnlineUserID >= 0)
			{
				MainGame.Inst.Backend.SetScore(this, levelid, d, time).EnsureNoError();
			}
		}

		public void SetNotCompleted(Guid levelid, FractionDifficulty d)
		{
			GetLevelData(levelid).SetBestTime(d, null);
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new PlayerProfile());

			RegisterProperty<PlayerProfile, AccountType>(SemVersion.VERSION_1_0_0, "type",    o => o.AccountType,       (o, v) => o.AccountType = v);

			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "userid",    o => o.OnlineUserID,       (o, v) => o.OnlineUserID       = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "user",      o => o.OnlineUsername,     (o, v) => o.OnlineUsername     = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "pass",      o => o.OnlinePasswordHash, (o, v) => o.OnlinePasswordHash = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "revid",     o => o.OnlineRevisionID,   (o, v) => o.OnlineRevisionID   = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "uploaderr", o => o.NeedsReupload,      (o, v) => o.NeedsReupload      = v);

			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_1, "sounds",    o => o.SoundsEnabled,      (o, v) => o.SoundsEnabled      = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "effect",    o => o.EffectsEnabled,     (o, v) => o.EffectsEnabled     = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_3, "lang",      o => o.Language,           (o, v) => o.Language = v);

			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_1, "skiptut",   o => o.SkipTutorial,       (o, v) => o.SkipTutorial       = v);

			RegisterPropertyGuidictionary<PlayerProfile, LevelData>(SemVersion.VERSION_1_0_0, "progress", () => new LevelData(),  o => o.LevelData, (o, v) => o.LevelData = v);
		}

		protected override string GetTypeName()
		{
			return "PLAYER_PROFILE_DATA";
		}
	}
}
