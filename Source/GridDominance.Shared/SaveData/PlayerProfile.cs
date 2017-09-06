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
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen;

namespace GridDominance.Shared.SaveData
{
	public class PlayerProfile : RootDataFile
	{
		protected override SemVersion ArchiveVersion => SemVersion.VERSION_1_0_1;

		public int TotalPoints => LevelData.Sum(p => p.Value.TotalPoints);
		public int TotalTime => LevelData.Sum(p => p.Value.TotalTime);
		public int MultiplayerPoints;
		public bool HasMultiplayerGames;

		public Dictionary<Guid, LevelData> LevelData;
		public HashSet<Guid> PurchasedWorlds;
		public string StrPurchasedWorlds => String.Join("\n", PurchasedWorlds.Select(g => $"{g:B}").OrderBy(p => p));


		public AccountType AccountType;
		public int OnlineUserID;
		public string OnlineUsername;
		public string OnlinePasswordHash;
		public int OnlineRevisionID;
		public bool NeedsReupload;

		public bool SoundsEnabled;
		public bool EffectsEnabled;
		public bool MusicEnabled;

		public Guid LastMultiplayerHostedLevel;
		public GameSpeedModes LastMultiplayerHostedSpeed;

		public bool SkipTutorial;
		public bool AccountReminderShown;

		public int Language;

		public PlayerProfile()
		{
			InitEmpty();
		}

		public void InitEmpty()
		{
			LevelData = new Dictionary<Guid, LevelData>();
			PurchasedWorlds = new HashSet<Guid>();

			MultiplayerPoints = 0;

			AccountType = AccountType.Local;
			OnlineUserID = -1;
			OnlineUsername = "anonymous";
			OnlinePasswordHash = string.Empty;
			OnlineRevisionID = -1;
			NeedsReupload = false;

			SoundsEnabled = true;
			EffectsEnabled = true;
			MusicEnabled = true;

			LastMultiplayerHostedLevel = Levels.LEVELID_1_3;
			LastMultiplayerHostedSpeed = GameSpeedModes.NORMAL;

			SkipTutorial = false;
			AccountReminderShown = false;

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

		public int GetWorldPoints(GraphBlueprint w)
		{
			return LevelData.Where(d => Levels.GetWorldByLevelID(d.Key) == w.ID).Sum(p => p.Value.TotalPoints);
		}

		public int GetWorldTime(GraphBlueprint w)
		{
			return LevelData.Where(d => Levels.GetWorldByLevelID(d.Key) == w.ID).Sum(p => p.Value.TotalTime);
		}

		public void SetCompleted(Guid levelid, FractionDifficulty d, int time, bool upload)
		{
			GetLevelData(levelid).SetBestTime(d, time);

			if (upload && OnlineUserID >= 0)
			{
				MainGame.Inst.Backend.SetScore(this, levelid, d, time).EnsureNoError();
			}
		}

		public int IncMultiplayerScore(int add, bool upload)
		{
			if (add == 0) return 0;

			MultiplayerPoints += add;
			HasMultiplayerGames = true;

			if (upload)
			{
				MainGame.Inst.Backend.SetScoreAndTime(this).EnsureNoError();
			}

			return add;
		}

		public int DecMultiplayerScore(int sub, bool upload)
		{
			sub = Math.Min(sub, MultiplayerPoints);

			if (sub == 0) return 0;

			MultiplayerPoints -= sub;
			HasMultiplayerGames = true;

			if (upload)
			{
				MainGame.Inst.Backend.SetScoreAndTime(this).EnsureNoError();
			}

			return -sub;
		}

		public void SetNotCompleted(Guid levelid, FractionDifficulty d)
		{
			GetLevelData(levelid).SetBestTime(d, null);
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new PlayerProfile());

			RegisterProperty<PlayerProfile, AccountType>(SemVersion.VERSION_1_0_0, "type",    o => o.AccountType,       (o, v) => o.AccountType = v);

			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "userid",    o => o.OnlineUserID,               (o, v) => o.OnlineUserID       = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "user",      o => o.OnlineUsername,             (o, v) => o.OnlineUsername     = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "pass",      o => o.OnlinePasswordHash,         (o, v) => o.OnlinePasswordHash = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "revid",     o => o.OnlineRevisionID,           (o, v) => o.OnlineRevisionID   = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "uploaderr", o => o.NeedsReupload,              (o, v) => o.NeedsReupload      = v);

			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "sounds",    o => o.SoundsEnabled,              (o, v) => o.SoundsEnabled      = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "effect",    o => o.EffectsEnabled,             (o, v) => o.EffectsEnabled     = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "lang",      o => o.Language,                   (o, v) => o.Language           = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "music",     o => o.MusicEnabled,               (o, v) => o.MusicEnabled       = v);

			RegisterProperty<PlayerProfile, GameSpeedModes>(SemVersion.VERSION_1_0_0, "mp_speed",  o => o.LastMultiplayerHostedSpeed, (o, v) => o.LastMultiplayerHostedSpeed = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "mp_level",  o => o.LastMultiplayerHostedLevel, (o, v) => o.LastMultiplayerHostedLevel = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "mp_score",  o => o.MultiplayerPoints,          (o, v) => o.MultiplayerPoints = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "mp_alive",  o => o.HasMultiplayerGames,        (o, v) => o.HasMultiplayerGames = v);

			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_0, "skiptut",   o => o.SkipTutorial,               (o, v) => o.SkipTutorial         = v);
			RegisterProperty<PlayerProfile>(SemVersion.VERSION_1_0_1, "reminder1", o => o.AccountReminderShown,       (o, v) => o.AccountReminderShown = v);

			RegisterPropertyGuidDictionary<PlayerProfile, LevelData>(SemVersion.VERSION_1_0_0, "progress", () => new LevelData(),  o => o.LevelData, (o, v) => o.LevelData = v);
			RegisterPropertyGuidSet<PlayerProfile>(SemVersion.VERSION_1_0_0, "purchases",  o => o.PurchasedWorlds, (o, v) => o.PurchasedWorlds = v);
		}

		protected override string GetTypeName()
		{
			return "PLAYER_PROFILE_DATA";
		}
	}
}
