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
	public enum WorldUnlockState
	{
		OpenAndUnlocked,                 // World can be entered w/o problems
		ReachableButMustBePreviewed,     // World is reachable but not enough points, show preview
		UnreachableButCanBePreviewed,    // World is unreachable - but still show preview (for IAB / unlock infos)
		UnreachableAndFullyLocked        // World is locked, only show error
	}

	public static class UnlockManager
	{
		public static WorldUnlockState IsUnlocked(GraphBlueprint w, bool showToast) => IsUnlocked(w.ID, showToast);

		public static WorldUnlockState IsUnlocked(Guid id, bool showToast)
		{
			if (id == Levels.WORLD_ID_TUTORIAL)
			{
				return WorldUnlockState.OpenAndUnlocked;
			}

			if (id == Levels.WORLD_001.ID)
			{
				if (MainGame.Inst.Profile.SkipTutorial) return WorldUnlockState.OpenAndUnlocked;
				if (MainGame.Inst.Profile.GetLevelData(Levels.LEVEL_TUTORIAL).HasAnyCompleted()) return WorldUnlockState.OpenAndUnlocked;

				return WorldUnlockState.UnreachableButCanBePreviewed;
			}

			if (id == Levels.WORLD_002.ID)
			{
				switch (GDConstants.FLAVOR)
				{
					case GDFlavor.FREE:
					{
						bool reachable = BlueprintAnalyzer.IsWorldReachable(Levels.WORLD_001, Levels.WORLD_002.ID);
						int neededPoints = PointsForUnlock(id);

						if (reachable && MainGame.Inst.Profile.TotalPoints >= neededPoints) return WorldUnlockState.OpenAndUnlocked;

						return WorldUnlockState.UnreachableButCanBePreviewed;
						}
					case GDFlavor.IAB:
					{
						bool reachable = BlueprintAnalyzer.IsWorldReachable(Levels.WORLD_001, Levels.WORLD_002.ID);
						int neededPoints = PointsForUnlock(id);

						if (reachable && MainGame.Inst.Profile.TotalPoints >= neededPoints) return WorldUnlockState.OpenAndUnlocked;

						if (GetIABState(GDConstants.IAB_WORLD2, Levels.WORLD_002.ID, showToast)) return WorldUnlockState.OpenAndUnlocked;

						return reachable ? WorldUnlockState.ReachableButMustBePreviewed : WorldUnlockState.UnreachableButCanBePreviewed;
					}

					case GDFlavor.FULL:
					{
						bool reachable = BlueprintAnalyzer.IsWorldReachable(Levels.WORLD_001, Levels.WORLD_002.ID);

						return reachable ? WorldUnlockState.OpenAndUnlocked : WorldUnlockState.UnreachableAndFullyLocked;
					}

					default:
					{
						SAMLog.Error("UNLCK::EnumSwitch_IU_1", "GDConstants.FLAVOR = " + GDConstants.FLAVOR);
					}
				}
			}

			if (id == Levels.WORLD_003.ID)
			{
				switch (GDConstants.FLAVOR)
				{
					case GDFlavor.FREE:
					{
						bool reachable = BlueprintAnalyzer.IsWorldReachable(Levels.WORLD_002, Levels.WORLD_003.ID);
						int neededPoints = PointsForUnlock(id);

						if (reachable && MainGame.Inst.Profile.TotalPoints >= neededPoints) return WorldUnlockState.OpenAndUnlocked;

						return WorldUnlockState.UnreachableButCanBePreviewed;
					}
					case GDFlavor.IAB:
					{
						bool reachable = BlueprintAnalyzer.IsWorldReachable(Levels.WORLD_002, Levels.WORLD_003.ID);
						int neededPoints = PointsForUnlock(id);

						if (reachable && MainGame.Inst.Profile.TotalPoints >= neededPoints) return WorldUnlockState.OpenAndUnlocked;

						if (GetIABState(GDConstants.IAB_WORLD3, Levels.WORLD_003.ID, showToast)) return WorldUnlockState.OpenAndUnlocked;

						return reachable ? WorldUnlockState.ReachableButMustBePreviewed : WorldUnlockState.UnreachableButCanBePreviewed;
					}

					case GDFlavor.FULL:
					{
						bool reachable = BlueprintAnalyzer.IsWorldReachable(Levels.WORLD_002, Levels.WORLD_003.ID);

						return reachable ? WorldUnlockState.OpenAndUnlocked : WorldUnlockState.UnreachableAndFullyLocked;
						}

					default:
					{
						SAMLog.Error("UNLCK::EnumSwitch_IU_2", "GDConstants.FLAVOR = " + GDConstants.FLAVOR);
					}
				}
			}

			if (id == Levels.WORLD_004.ID)
			{
				switch (GDConstants.FLAVOR)
				{
					case GDFlavor.FREE:
					{
						bool reachable = BlueprintAnalyzer.IsWorldReachable(Levels.WORLD_003, Levels.WORLD_004.ID);
						int neededPoints = PointsForUnlock(id);

						if (reachable && MainGame.Inst.Profile.TotalPoints >= neededPoints) return WorldUnlockState.OpenAndUnlocked;

						return WorldUnlockState.UnreachableButCanBePreviewed;
					}
					case GDFlavor.IAB:
					{
						bool reachable = BlueprintAnalyzer.IsWorldReachable(Levels.WORLD_003, Levels.WORLD_004.ID);
						int neededPoints = PointsForUnlock(id);

						if (reachable && MainGame.Inst.Profile.TotalPoints >= neededPoints) return WorldUnlockState.OpenAndUnlocked;

						if (GetIABState(GDConstants.IAB_WORLD4, Levels.WORLD_004.ID, showToast)) return WorldUnlockState.OpenAndUnlocked;

						return reachable ? WorldUnlockState.ReachableButMustBePreviewed : WorldUnlockState.UnreachableButCanBePreviewed;
						}

					case GDFlavor.FULL:
					{
						bool reachable = BlueprintAnalyzer.IsWorldReachable(Levels.WORLD_003, Levels.WORLD_004.ID);

						return reachable ? WorldUnlockState.OpenAndUnlocked : WorldUnlockState.UnreachableAndFullyLocked;
						}

					default:
					{
						SAMLog.Error("UNLCK::EnumSwitch_IU_3", "GDConstants.FLAVOR = " + GDConstants.FLAVOR);
					}
				}
			}

			if (id == Levels.WORLD_ID_GAMEEND)
			{
				if (!BlueprintAnalyzer.IsWorldReachable(Levels.WORLD_004, Levels.WORLD_ID_GAMEEND)) return WorldUnlockState.OpenAndUnlocked;

				return WorldUnlockState.UnreachableAndFullyLocked;
			}

			if (id == Levels.WORLD_ID_MULTIPLAYER)
			{
				switch (GDConstants.FLAVOR)
				{
					case GDFlavor.FREE:
					{
						return WorldUnlockState.UnreachableAndFullyLocked;
					}
					case GDFlavor.IAB:
					{
						return GetIABState(GDConstants.IAB_MULTIPLAYER, Levels.WORLD_ID_MULTIPLAYER, showToast) ? WorldUnlockState.OpenAndUnlocked : WorldUnlockState.ReachableButMustBePreviewed;
					}

					case GDFlavor.FULL:
					{
						return WorldUnlockState.OpenAndUnlocked;
					}

					default:
					{
						SAMLog.Error("UNLCK::EnumSwitch_IU_4", "GDConstants.FLAVOR = " + GDConstants.FLAVOR);
					}
				}
			}

			SAMLog.Error("UNLCK::NID", $"UnlockManager: ID not found {id} ({showToast})");
			return WorldUnlockState.UnreachableAndFullyLocked;
		}

		private static bool GetIABState(string iabCode, Guid id, bool toast)
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
				return false;
			}

			if (MainGame.Inst.Profile.PurchasedWorlds.Contains(id)) return true;

			switch (ip)
			{
				case PurchaseQueryResult.Purchased:
					MainGame.Inst.Profile.PurchasedWorlds.Add(id);
					MainGame.Inst.SaveProfile();
					return true;

				case PurchaseQueryResult.NotPurchased:
				case PurchaseQueryResult.Cancelled:
					return false;

				case PurchaseQueryResult.Error:
					if (toast) MainGame.Inst.ShowToast("UNLCK::E1", L10N.T(L10NImpl.STR_IAB_TESTERR), 40, FlatColors.Pomegranate, FlatColors.Foreground, 2.5f);
					return false;

				case PurchaseQueryResult.Refunded:
					if (MainGame.Inst.Profile.PurchasedWorlds.Contains(id))
					{
						SAMLog.Debug("Level refunded: " + id);
						MainGame.Inst.Profile.PurchasedWorlds.Remove(id);
						MainGame.Inst.SaveProfile();
					}
					return false;

				case PurchaseQueryResult.NotConnected:
					if (toast) MainGame.Inst.ShowToast("UNLCK::E2", L10N.T(L10NImpl.STR_IAB_TESTNOCONN), 40, FlatColors.Pomegranate, FlatColors.Foreground, 2.5f);
					return false;

				case PurchaseQueryResult.CurrentlyInitializing:
					if (toast) MainGame.Inst.ShowToast("UNLCK::E3", L10N.T(L10NImpl.STR_IAB_TESTINPROGRESS), 40, FlatColors.Pomegranate, FlatColors.Foreground, 2.5f);
					return false;

				default:
					SAMLog.Error("EnumSwitch_IU", "IsUnlocked()", "MainGame.Inst.Bridge.IAB.IsPurchased(MainGame.IAB_WORLD " + id + ")) -> " + ip);
					return false;
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
