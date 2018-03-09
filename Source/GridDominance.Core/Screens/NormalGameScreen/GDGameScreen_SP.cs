using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.Common;
using GridDominance.Shared.Screens.Common.HUD.Elements;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using Microsoft.Xna.Framework;
using GridDominance.Shared.Screens.NormalGameScreen.HUD;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.Entities.Cannons;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using GridDominance.Shared.Screens.NormalGameScreen.HUD.ScorePanel;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.NormalGameScreen
{
	class GDGameScreen_SP : GDGameScreen
	{
		protected override GameHUD CreateHUD() => new GDGameHUD(this);

		public readonly GraphBlueprint WorldBlueprint;

		public override Fraction LocalPlayerFraction => fractionPlayer;

		private readonly List<Guid> _unlocksBefore;

		public GDGameScreen_SP(MainGame game, GraphicsDeviceManager gdm, LevelBlueprint bp, FractionDifficulty diff, GraphBlueprint ws) 
			: base(game, gdm, bp, diff, false, false, diff == FractionDifficulty.DIFF_0 && bp.UniqueID == Levels.LEVELID_1_1)
		{
			WorldBlueprint = ws;

			GameSpeedMode = MainGame.Inst.Profile.SingleplayerGameSpeed;
			UpdateGameSpeed();

			_unlocksBefore = UnlockManager.GetFullUnlockState().ToList();
		}

		public override void RestartLevel(bool updateSpeed)
		{
			if (updateSpeed)
			{
				MainGame.Inst.Profile.SingleplayerGameSpeed = GameSpeedMode;
				MainGame.Inst.SaveProfile();
			}

			GDOwner.SetLevelScreen(Blueprint, Difficulty, WorldBlueprint);
		}

		public override void ReplayLevel(FractionDifficulty diff)
		{
			GDOwner.SetLevelScreen(Blueprint, diff, WorldBlueprint);
		}

		public override void ShowScorePanel(LevelBlueprint lvl, PlayerProfile profile, HashSet<FractionDifficulty> newDifficulties, bool playerHasWon, int addPoints, int time)
		{
			((GDGameHUD)HUD).BtnPause.IsEnabled = false;
			((GDGameHUD)HUD).BtnSpeed.IsEnabled = false;

			GameSpeedMode = GameSpeedModes.NORMAL;

			HUD.AddModal(new HUDScorePanel(lvl, profile, newDifficulties, Difficulty, playerHasWon, addPoints, time), false);

			if (playerHasWon && MainGame.Flavor==GDFlavor.IAB)
			{
				var diff = UnlockManager.GetFullUnlockState().Except(_unlocksBefore);
				foreach (var d in diff)
				{
					if (d == Levels.WORLD_ID_ONLINE) AchievementPopup.Show(L10N.T(L10NImpl.STR_ACH_UNLOCK_ONLINE));
					if (d == Levels.WORLD_ID_MULTIPLAYER) AchievementPopup.Show(L10N.T(L10NImpl.STR_ACH_UNLOCK_MULTIPLAYER));
					if (d == Levels.WORLD_001.ID) AchievementPopup.Show(L10N.TF(L10NImpl.STR_ACH_UNLOCK_WORLD, 1));
					if (d == Levels.WORLD_002.ID) AchievementPopup.Show(L10N.TF(L10NImpl.STR_ACH_UNLOCK_WORLD, 2));
					if (d == Levels.WORLD_003.ID) AchievementPopup.Show(L10N.TF(L10NImpl.STR_ACH_UNLOCK_WORLD, 3));
					if (d == Levels.WORLD_004.ID) AchievementPopup.Show(L10N.TF(L10NImpl.STR_ACH_UNLOCK_WORLD, 4));
				}
			}
		}

		protected override void TestForGameEndingCondition()
		{
			if (HasFinished) return;

			bool hasPlayer = false;
			bool hasComputer = false;

			foreach (var cannon in Entities.Enumerate().OfType<Cannon>())
			{
				if (cannon is RelayCannon) continue;
				if (cannon is ShieldProjectorCannon) continue;

				if (cannon.Fraction.IsPlayer) hasPlayer = true;
				if (cannon.Fraction.IsComputer) hasComputer = true;
			}

			if (hasPlayer && !hasComputer) EndGame(true, fractionPlayer);
			if (!hasPlayer && hasComputer)
			{
				var winner = Entities
					.Enumerate()
					.OfType<Cannon>()
					.GroupBy(p => p.Fraction)
					.Where(p => !p.Key.IsNeutral)
					.OrderBy(p => p.Count())
					.Last()
					.Key;

				EndGame(false, winner);
			}
		}

		private void EndGame(bool playerWon, Fraction winner)
		{
			MainGame.Inst.Profile.SingleplayerGameSpeed = GameSpeedMode;
			MainGame.Inst.SaveProfile();

			HasFinished = true;
			PlayerWon = playerWon;

			var ctime = (int)(LevelTime * 1000);

			if (playerWon)
			{
				int scoreGain = 0;
				HashSet<FractionDifficulty> gains = new HashSet<FractionDifficulty>();

				for (FractionDifficulty diff = FractionDifficulty.DIFF_0; diff <= Difficulty; diff++)
				{
					if (!GDOwner.Profile.GetLevelData(Blueprint.UniqueID).HasCompletedOrBetter(diff))
					{
						scoreGain += FractionDifficultyHelper.GetScore(diff);
						gains.Add(diff);
					}
				}

				{
					if (!GDOwner.Profile.GetLevelData(Blueprint.UniqueID).HasCompletedExact(Difficulty))
					{
						GDOwner.Profile.SetCompleted(Blueprint.UniqueID, Difficulty, ctime, true);
					}
					var localdata = GDOwner.Profile.LevelData[Blueprint.UniqueID].Data[Difficulty];

					if (ctime < localdata.BestTime)
					{
						// update PB
						GDOwner.Profile.SetCompleted(Blueprint.UniqueID, Difficulty, ctime, true);
					}

					// Fake the online data until next sync
					localdata.GlobalCompletionCount++;
					if (ctime < localdata.GlobalBestTime || localdata.GlobalBestTime == -1)
					{
						if (localdata.GlobalBestTime != -1)
						{
							// WURLD RECARD !!
							// Dispatch is trick to put in infront of score panel ...
							MainGame.Inst.DispatchBeginInvoke(() => {AchievementPopup.Show(L10N.T(L10NImpl.STR_ACH_WORLDRECORD));});
						}

						localdata.GlobalBestTime = ctime;
						localdata.GlobalBestUserID = GDOwner.Profile.OnlineUserID;
					}
				}

				GDOwner.SaveProfile();
				ShowScorePanel(Blueprint, GDOwner.Profile, gains, true, scoreGain, ctime);
				MainGame.Inst.GDSound.PlayEffectGameWon();

				EndGameConvert(winner);
			}
			else
			{
				ShowScorePanel(Blueprint, GDOwner.Profile, new HashSet<FractionDifficulty>(), false, 0, ctime);

				MainGame.Inst.GDSound.PlayEffectGameOver();

				EndGameConvert(winner);
			}

			foreach (var cannon in Entities.Enumerate().OfType<Cannon>())
			{
				cannon.ForceUpdateController();
			}
		}

		public override void ExitToMap(bool updateSpeed)
		{
			if (updateSpeed)
			{
				MainGame.Inst.Profile.SingleplayerGameSpeed = GameSpeedMode;
				MainGame.Inst.SaveProfile();
			}

			MainGame.Inst.SetWorldMapScreenZoomedOut(WorldBlueprint, Blueprint);
		}

		public override AbstractFractionController CreateController(Fraction f, Cannon cannon)
		{
			if (HasFinished)
			{
				if (PlayerWon)
					return new EndGameAutoPlayerController(this, cannon, f);
				else
					return new EndGameAutoComputerController(this, cannon, f);
			}
			
			switch (f.Type)
			{
				case FractionType.PlayerFraction:
					return new PlayerController(this, cannon, f);

				case FractionType.ComputerFraction:
#if DEBUG
					if (DebugSettings.Get("ControlEnemies")) return new PlayerController(this, cannon, f);
#endif
					return cannon.CreateKIController(this, f);

				case FractionType.NeutralFraction:
#if DEBUG
					if (DebugSettings.Get("ControlEnemies")) return new PlayerController(this, cannon, f);
#endif
					return cannon.CreateNeutralController(this, f);

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override void OnShow()
		{
			MainGame.Inst.GDSound.PlayMusicLevel(Levels.WORLD_NUMBERS[WorldBlueprint.ID]);
		}
	}
}
