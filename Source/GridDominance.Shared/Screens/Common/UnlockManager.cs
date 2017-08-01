using System;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.WorldMapScreen;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Shared.Screens.Common
{
	public enum WorldUnlockState { FullyLocked, NeedsAction, Unlocked }

	public static class UnlockManager
	{
		public static WorldUnlockState IsUnlocked(GraphBlueprint w, bool showToast) => IsUnlocked(w.ID, showToast);

		public static WorldUnlockState IsUnlocked(Guid id, bool showToast)
		{
			if (id == Levels.WORLD_ID_TUTORIAL)
			{
				return WorldUnlockState.Unlocked;
			}

			if (id == Levels.WORLD_001.ID)
			{
				if (MainGame.Inst.Profile.SkipTutorial) return WorldUnlockState.Unlocked;
				if (MainGame.Inst.Profile.GetLevelData(Levels.LEVEL_TUTORIAL).HasAnyCompleted()) return WorldUnlockState.Unlocked;

				return WorldUnlockState.NeedsAction;
			}

			if (id == Levels.WORLD_002.ID)
			{
				if (!BlueprintAnalyzer.IsWorldReachable(Levels.WORLD_001, Levels.WORLD_002.ID)) return WorldUnlockState.FullyLocked;

				if (!GDConstants.USE_IAB) return WorldUnlockState.Unlocked;

				int neededPoints = PointsForUnlock(id);

				if (MainGame.Inst.Profile.TotalPoints >= neededPoints) return WorldUnlockState.Unlocked;

				return GetIABState(GDConstants.IAB_WORLD2, Levels.WORLD_002.ID, showToast);
			}

			if (id == Levels.WORLD_003.ID)
			{
				if (!BlueprintAnalyzer.IsWorldReachable(Levels.WORLD_002, Levels.WORLD_003.ID)) return WorldUnlockState.FullyLocked;

				if (!GDConstants.USE_IAB) return WorldUnlockState.Unlocked;

				int neededPoints = PointsForUnlock(id);

				if (MainGame.Inst.Profile.TotalPoints >= neededPoints) return WorldUnlockState.Unlocked;

				return GetIABState(GDConstants.IAB_WORLD3, Levels.WORLD_003.ID, showToast);
			}

			if (id == Levels.WORLD_004.ID)
			{
				if (!BlueprintAnalyzer.IsWorldReachable(Levels.WORLD_003, Levels.WORLD_004.ID)) return WorldUnlockState.FullyLocked;

				if (!GDConstants.USE_IAB) return WorldUnlockState.Unlocked;

				int neededPoints = PointsForUnlock(id);

				if (MainGame.Inst.Profile.TotalPoints >= neededPoints) return WorldUnlockState.Unlocked;

				return GetIABState(GDConstants.IAB_WORLD4, Levels.WORLD_004.ID, showToast);
			}

			if (id == Levels.WORLD_ID_GAMEEND)
			{
				if (!BlueprintAnalyzer.IsWorldReachable(Levels.WORLD_004, Levels.WORLD_ID_GAMEEND)) return WorldUnlockState.FullyLocked;

				return WorldUnlockState.FullyLocked;
			}

			if (id == Levels.WORLD_ID_MULTIPLAYER)
			{
				if (!GDConstants.USE_IAB) return WorldUnlockState.Unlocked;

				return GetIABState(GDConstants.IAB_MULTIPLAYER, Levels.WORLD_ID_MULTIPLAYER, showToast);
			}

			SAMLog.Error("UNLCK::NID", $"UnlockManager: ID not found {id} ({showToast})");
			return WorldUnlockState.FullyLocked;
		}

		private static WorldUnlockState GetIABState(string iabCode, Guid id, bool toast)
		{
			var ip = MainGame.Inst.Bridge.IAB.IsPurchased(iabCode);

			if (ip == PurchaseQueryResult.Refunded)
			{
				if (MainGame.Inst.Profile.PurchasedWorlds.Contains(id))
				{
					SAMLog.Debug("Level refunded: " + id);
					MainGame.Inst.Profile.PurchasedWorlds.Remove(id);
					MainGame.Inst.SaveProfile();
				}
				return WorldUnlockState.NeedsAction;
			}

			if (MainGame.Inst.Profile.PurchasedWorlds.Contains(id)) return WorldUnlockState.Unlocked;

			switch (ip)
			{
				case PurchaseQueryResult.Purchased:
					MainGame.Inst.Profile.PurchasedWorlds.Add(id);
					MainGame.Inst.SaveProfile();
					return WorldUnlockState.Unlocked;

				case PurchaseQueryResult.NotPurchased:
				case PurchaseQueryResult.Cancelled:
					return WorldUnlockState.NeedsAction;

				case PurchaseQueryResult.Error:
					if (toast) MainGame.Inst.ShowToast("UNLCK::E1", L10N.T(L10NImpl.STR_IAB_TESTERR), 40, FlatColors.Pomegranate, FlatColors.Foreground, 2.5f);
					return WorldUnlockState.NeedsAction;

				case PurchaseQueryResult.Refunded:
					if (MainGame.Inst.Profile.PurchasedWorlds.Contains(id))
					{
						SAMLog.Debug("Level refunded: " + id);
						MainGame.Inst.Profile.PurchasedWorlds.Remove(id);
						MainGame.Inst.SaveProfile();
					}
					return WorldUnlockState.NeedsAction;

				case PurchaseQueryResult.NotConnected:
					if (toast) MainGame.Inst.ShowToast("UNLCK::E2", L10N.T(L10NImpl.STR_IAB_TESTNOCONN), 40, FlatColors.Pomegranate, FlatColors.Foreground, 2.5f);
					return WorldUnlockState.NeedsAction;

				case PurchaseQueryResult.CurrentlyInitializing:
					if (toast) MainGame.Inst.ShowToast("UNLCK::E3", L10N.T(L10NImpl.STR_IAB_TESTINPROGRESS), 40, FlatColors.Pomegranate, FlatColors.Foreground, 2.5f);
					return WorldUnlockState.NeedsAction;

				default:
					SAMLog.Error("EnumSwitch_IU", "IsUnlocked()", "MainGame.Inst.Bridge.IAB.IsPurchased(MainGame.IAB_WORLD " + id + ")) -> " + ip);
					return WorldUnlockState.NeedsAction;
			}
		}

		public static int PointsForUnlock(Guid id)
		{
			if (id == Levels.WORLD_001.ID)
				return 0;

			if (id == Levels.WORLD_002.ID)
				return BlueprintAnalyzer.MaxPointCount(Levels.WORLD_001) -
				       1 * GDConstants.FREE_SCORE_PER_WORLD -
				       GDConstants.SCORE_DIFF_0;

			if (id == Levels.WORLD_003.ID)
				return BlueprintAnalyzer.MaxPointCount(Levels.WORLD_001) +
				       BlueprintAnalyzer.MaxPointCount(Levels.WORLD_002) -
				       2 * GDConstants.FREE_SCORE_PER_WORLD -
				       GDConstants.SCORE_DIFF_0;

			if (id == Levels.WORLD_004.ID)
				return BlueprintAnalyzer.MaxPointCount(Levels.WORLD_001) +
				       BlueprintAnalyzer.MaxPointCount(Levels.WORLD_002) +
				       BlueprintAnalyzer.MaxPointCount(Levels.WORLD_003) -
				       3 * GDConstants.FREE_SCORE_PER_WORLD -
				       GDConstants.SCORE_DIFF_0;

			SAMLog.Error("UNLCK::PFU", $"UnlockManager: WorldID not found {id}");
			return 99999;
		}
	}

}
