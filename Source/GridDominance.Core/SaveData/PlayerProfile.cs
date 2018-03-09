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
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.Network.Backend.QueryResult;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen;
using GridDominance.Shared.Screens.OverworldScreen;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Shared.SaveData
{
	public class PlayerProfile : RootDataFile
	{
		protected override SemVersion ArchiveVersion => SemVersion.VERSION_1_0_7;

		public int TotalPoints => LevelData.Sum(p => p.Value.TotalPoints);
		public int HighscoreTime => LevelData.Sum(p => p.Value.HighscoreTime);
		public int MultiplayerPoints;
		public bool HasMultiplayerGames;

		public Dictionary<Guid, LevelData> LevelData;
		public HashSet<Guid> PurchasedWorlds;
		public string StrPurchasedWorlds => String.Join("\n", PurchasedWorlds.Select(g => $"{g:B}").OrderBy(p => p));
		public List<CustomLevelData> CustomLevelData;

#if DEBUG
		public bool NoAfterSerializeFixes = false;
#endif

		public AccountType AccountType;
		public int OnlineUserID;
		public string OnlineUsername;
		public string OnlinePasswordHash;
		public int OnlineRevisionID;
		public bool NeedsReupload;

		public bool UnacknowledgedAuthError;
		public string BackupOnlineUsername;

		public bool SoundsEnabled;
		public bool EffectsEnabled;
		public bool MusicEnabled;
		public bool ColorblindMode;

		public Guid LastMultiplayerHostedLevel;
		public GameSpeedModes LastMultiplayerHostedSpeed;

		public bool SkipTutorial;
		public bool AccountReminderShown;

		public int Language;
		public GameSpeedModes SingleplayerGameSpeed;
		
		public int ScoreStars;
		public int ScoreSCCM;
		public bool HasCreatedLevels;

		public PlayerProfile()
		{
			InitEmpty();
		}

		public void InitEmpty()
		{
			LevelData = new Dictionary<Guid, LevelData>();
			PurchasedWorlds = new HashSet<Guid>();
			CustomLevelData = new List<CustomLevelData>();

			MultiplayerPoints = 0;
			ScoreStars        = 0;
			ScoreSCCM         = 0;
			HasCreatedLevels  = false;

			AccountType = AccountType.Local;
			OnlineUserID = -1;
			OnlineUsername = "anonymous";
			OnlinePasswordHash = string.Empty;
			OnlineRevisionID = -1;
			NeedsReupload = false;

			UnacknowledgedAuthError = false;
			BackupOnlineUsername = string.Empty;

			SoundsEnabled = true;
			EffectsEnabled = true;
			MusicEnabled = true;
			ColorblindMode = false;

			LastMultiplayerHostedLevel = Levels.LEVELID_1_3;
			LastMultiplayerHostedSpeed = GameSpeedModes.NORMAL;
			
			SkipTutorial = false;
			AccountReminderShown = false;

			Language = L10N.LANG_EN_US;
			SingleplayerGameSpeed = GameSpeedModes.NORMAL;
		}
		
		protected override void Configure()
		{
			RegisterConstructor(() => new PlayerProfile());

			RegisterProperty<PlayerProfile, AccountType>(            SemVersion.VERSION_1_0_0, "type",                                        o => o.AccountType,                (o, v) => o.AccountType                = v);

			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_0, "userid",                                      o => o.OnlineUserID,               (o, v) => o.OnlineUserID               = v);
			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_0, "user",                                        o => o.OnlineUsername,             (o, v) => o.OnlineUsername             = v);
			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_0, "pass",                                        o => o.OnlinePasswordHash,         (o, v) => o.OnlinePasswordHash         = v);
			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_0, "revid",                                       o => o.OnlineRevisionID,           (o, v) => o.OnlineRevisionID           = v);
			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_0, "uploaderr",                                   o => o.NeedsReupload,              (o, v) => o.NeedsReupload              = v);

			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_4, "autherror",                                   o => o.UnacknowledgedAuthError,    (o, v) => o.UnacknowledgedAuthError    = v);
			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_4, "backupusername",                              o => o.BackupOnlineUsername,       (o, v) => o.BackupOnlineUsername       = v);

			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_0, "sounds",                                      o => o.SoundsEnabled,              (o, v) => o.SoundsEnabled              = v);
			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_0, "effect",                                      o => o.EffectsEnabled,             (o, v) => o.EffectsEnabled             = v);
			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_0, "lang",                                        o => o.Language,                   (o, v) => o.Language                   = v);
			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_0, "music",                                       o => o.MusicEnabled,               (o, v) => o.MusicEnabled               = v);
			RegisterProperty<PlayerProfile, GameSpeedModes>(         SemVersion.VERSION_1_0_2, "gamespeed",                                   o => o.SingleplayerGameSpeed,      (o, v) => o.SingleplayerGameSpeed      = v);
			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_3, "colormode",                                   o => o.ColorblindMode,             (o, v) => o.ColorblindMode             = v);

			RegisterProperty<PlayerProfile, GameSpeedModes>(         SemVersion.VERSION_1_0_0, "mp_speed",                                    o => o.LastMultiplayerHostedSpeed, (o, v) => o.LastMultiplayerHostedSpeed = v);
			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_0, "mp_level",                                    o => o.LastMultiplayerHostedLevel, (o, v) => o.LastMultiplayerHostedLevel = v);
			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_0, "mp_score",                                    o => o.MultiplayerPoints,          (o, v) => o.MultiplayerPoints          = v);
			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_0, "mp_alive",                                    o => o.HasMultiplayerGames,        (o, v) => o.HasMultiplayerGames        = v);

			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_6, "sccm_stars",                                  o => o.ScoreStars,                 (o, v) => o.ScoreStars                 = v);
			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_6, "sccm_score",                                  o => o.ScoreSCCM,                  (o, v) => o.ScoreSCCM                  = v);
			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_7, "sccm_creator",                                o => o.HasCreatedLevels,           (o, v) => o.HasCreatedLevels           = v);

			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_0, "skiptut",                                     o => o.SkipTutorial,               (o, v) => o.SkipTutorial               = v);
			RegisterProperty<PlayerProfile>(                         SemVersion.VERSION_1_0_1, "reminder1",                                   o => o.AccountReminderShown,       (o, v) => o.AccountReminderShown       = v);

			RegisterPropertyGuidDictionary<PlayerProfile, LevelData>(SemVersion.VERSION_1_0_0, "progress",       () => new LevelData(),       o => o.LevelData,                  (o, v) => o.LevelData                  = v);
			RegisterPropertyList<PlayerProfile, CustomLevelData>(    SemVersion.VERSION_1_0_5, "sccm_progress",  () => new CustomLevelData(), o => o.CustomLevelData,            (o, v) => o.CustomLevelData            = v);

			RegisterPropertyGuidSet<PlayerProfile>(                  SemVersion.VERSION_1_0_0, "purchases",                                   o => o.PurchasedWorlds,            (o, v) => o.PurchasedWorlds            = v);
		}

		protected override void OnAfterDeserialize()
		{
#if DEBUG
			if (NoAfterSerializeFixes) return;
#endif

			// In v1.0.1 there was a bug where an INVLOGIN would result in AccountType=Anonymous but UserID=-1
			if (AccountType == AccountType.Local || OnlineUserID == -1)
			{
				OnlineUserID = -1;
				OnlineUsername = "anonymous";
				AccountType = AccountType.Local;
				OnlinePasswordHash = "";
			}

			SingleplayerGameSpeed = GameSpeedModes.NORMAL; // reset on each Gamestart
		}

		protected override string GetTypeName()
		{
			return "PLAYER_PROFILE_DATA";
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

		public int GetWorldHighscoreTime(GraphBlueprint w)
		{
			return LevelData.Where(d => Levels.GetWorldByLevelID(d.Key) == w.ID).Sum(p => p.Value.HighscoreTime);
		}

		public void SetCompleted(Guid levelid, FractionDifficulty d, int time, bool upload)
		{
			GetLevelData(levelid).SetBestTime(d, time);

			if (upload && OnlineUserID >= 0)
			{
				MainGame.Inst.Backend.SetScore(this, levelid, d, time).EnsureNoError();
			}
		}

		public void ResetUserOnError()
		{
			var err = (AccountType == AccountType.Full);

			if (err)
			{
				UnacknowledgedAuthError = true;
				BackupOnlineUsername = OnlineUsername;
			}

			OnlineUserID = -1;
			OnlineUsername = "anonymous";
			AccountType = AccountType.Local;
			OnlinePasswordHash = "";


			if (err)
			{
				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					if (MainGame.Inst.GetCurrentScreen() is GDOverworldScreen scr) scr.TryShowAuthErrorPanel();
				});
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

		public CustomLevelData GetCustomLevelData(long oid)
		{
			return CustomLevelData.FirstOrDefault(d => d.id == oid);
		}

		public CustomLevelData GetOrAddCustomLevelData(long oid) // no save (!)
		{
			var data = CustomLevelData.FirstOrDefault(d => d.id == oid);
			if (data != null) return data;

			data = new CustomLevelData { id = oid };
			CustomLevelData.Add(data);
			return data;
		}

		public FractionDifficulty? GetCustomLevelPB(long oid)
		{
			var dat = GetCustomLevelData(oid);
			if (dat == null) return null;

			if (dat.Diff3_HasCompleted) return FractionDifficulty.DIFF_3;
			if (dat.Diff2_HasCompleted) return FractionDifficulty.DIFF_2;
			if (dat.Diff1_HasCompleted) return FractionDifficulty.DIFF_1;
			if (dat.Diff0_HasCompleted) return FractionDifficulty.DIFF_0;

			return null;
		}

		public bool HasCustomLevelBeaten(long onlineID, FractionDifficulty diff)
		{
			var dat = GetCustomLevelData(onlineID);
			if (dat == null) return false;

			switch (diff)
			{
				case FractionDifficulty.DIFF_0: return dat.Diff0_HasCompleted;
				case FractionDifficulty.DIFF_1: return dat.Diff1_HasCompleted;
				case FractionDifficulty.DIFF_2: return dat.Diff2_HasCompleted;
				case FractionDifficulty.DIFF_3: return dat.Diff3_HasCompleted;
				case FractionDifficulty.NEUTRAL:
				case FractionDifficulty.PLAYER:
				default:
					SAMLog.Error("PP::EnumSwitch_HCLB", "diff: " + diff);
					return false;
			}
		}

		public bool HasCustomLevelBeaten(long onlineID)
		{
			var dat = GetCustomLevelData(onlineID);
			if (dat == null) return false;

			return dat.Diff0_HasCompleted || dat.Diff1_HasCompleted || dat.Diff2_HasCompleted || dat.Diff3_HasCompleted;
		}

		public string GetCustomLevelTimeString(long onlineID, FractionDifficulty diff)
		{
			var dat = GetCustomLevelData(onlineID);
			if (dat == null) return string.Empty;
			
			switch (diff)
			{
				case FractionDifficulty.DIFF_0: return dat.Diff0_HasCompleted ? TimeExtension.FormatMilliseconds(dat.Diff0_BestTime, true) : string.Empty;
				case FractionDifficulty.DIFF_1: return dat.Diff1_HasCompleted ? TimeExtension.FormatMilliseconds(dat.Diff1_BestTime, true) : string.Empty;
				case FractionDifficulty.DIFF_2: return dat.Diff2_HasCompleted ? TimeExtension.FormatMilliseconds(dat.Diff2_BestTime, true) : string.Empty;
				case FractionDifficulty.DIFF_3: return dat.Diff3_HasCompleted ? TimeExtension.FormatMilliseconds(dat.Diff3_BestTime, true) : string.Empty;
				case FractionDifficulty.NEUTRAL:
				case FractionDifficulty.PLAYER:
				default:
					SAMLog.Error("PP::EnumSwitch_GCLTS", "diff: " + diff);
					return "?";
			}
			
		}

		public bool HasCustomLevelStarred(long onlineID)
		{
			var dat = GetCustomLevelData(onlineID);
			if (dat == null) return false;

			return dat.starred;
		}

		public bool UpdateSCCMData(QueryResultUserData usr)
		{
			var changed = false;

			if (ScoreStars != usr.ScoreStars) { ScoreStars = usr.ScoreStars; changed = true; }
			if (ScoreSCCM  != usr.ScoreSCCM)  { ScoreSCCM  = usr.ScoreSCCM;  changed = true; }

			return changed;
		}

		public void SetCustomLevelCompleted(long oid, FractionDifficulty diff, int time) // no save (!)
		{
			var dat = GetOrAddCustomLevelData(oid);

			switch (diff)
			{
				case FractionDifficulty.DIFF_0:
					dat.Diff0_BestTime = time;
					dat.Diff0_HasCompleted = true;
					break;

				case FractionDifficulty.DIFF_1:
					dat.Diff1_BestTime = time;
					dat.Diff1_HasCompleted = true;
					break;

				case FractionDifficulty.DIFF_2:
					dat.Diff2_BestTime = time;
					dat.Diff2_HasCompleted = true;
					break;

				case FractionDifficulty.DIFF_3:
					dat.Diff3_BestTime = time;
					dat.Diff3_HasCompleted = true;
					break;

				case FractionDifficulty.NEUTRAL:
				case FractionDifficulty.PLAYER:
				default:
					SAMLog.Error("PP::EnumSwitch_SCLC", "diff: " + diff);
					break;
			}
		}

		internal void SetCustomLevelStarred(long oid, bool value)
		{
			var dat = GetOrAddCustomLevelData(oid);
			dat.starred = value;
		}

		public int?[] GetCustomLevelTimes(long oid)
		{
			var dat = GetOrAddCustomLevelData(oid);
			return new []
			{
				dat.Diff0_HasCompleted ? (int?)dat.Diff0_BestTime : null,
				dat.Diff1_HasCompleted ? (int?)dat.Diff1_BestTime : null,
				dat.Diff2_HasCompleted ? (int?)dat.Diff2_BestTime : null,
				dat.Diff3_HasCompleted ? (int?)dat.Diff3_BestTime : null,
			};
		}
	}
}
