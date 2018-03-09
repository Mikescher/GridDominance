using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using Microsoft.Xna.Framework;
using GridDominance.Shared.Screens.NormalGameScreen.HUD;
using GridDominance.Shared.Screens.NormalGameScreen.Entities.Cannons;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using GridDominance.Shared.Screens.NormalGameScreen.HUD.ScorePanel;
using GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.NormalGameScreen
{
	class GDGameScreen_SCCMPlay : GDGameScreen
	{
		protected override GameHUD CreateHUD() => new GDGameHUD(this);

		public override Fraction LocalPlayerFraction => fractionPlayer;

		public GDGameScreen_SCCMPlay(MainGame game, GraphicsDeviceManager gdm, LevelBlueprint bp, FractionDifficulty diff) 
			: base(game, gdm, bp, diff, false, false, false)
		{
			GameSpeedMode = MainGame.Inst.Profile.SingleplayerGameSpeed;
			UpdateGameSpeed();
		}

		public override void RestartLevel(bool updateSpeed)
		{
			if (updateSpeed)
			{
				MainGame.Inst.Profile.SingleplayerGameSpeed = GameSpeedMode;
				MainGame.Inst.SaveProfile();
			}

			GDOwner.SetCustomLevelScreen(Blueprint, Difficulty);
		}

		public override void ReplayLevel(FractionDifficulty diff)
		{
			GDOwner.SetCustomLevelScreen(Blueprint, diff);
		}

		public override void ShowScorePanel(LevelBlueprint lvl, PlayerProfile profile, HashSet<FractionDifficulty> xnullx, bool playerHasWon, int addPoints, int time)
		{
			((GDGameHUD)HUD).BtnPause.IsEnabled = false;
			((GDGameHUD)HUD).BtnSpeed.IsEnabled = false;

			GameSpeedMode = GameSpeedModes.NORMAL;

			if (playerHasWon)
				HUD.AddModal(new HUDSCCMScorePanel_Transmit(lvl, Difficulty, time), false);
			else
			HUD.AddModal(new HUDSCCMScorePanel_Lost(lvl, Difficulty), false);
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
				if (!GDOwner.Profile.GetOrAddCustomLevelData(Blueprint.CustomMeta_LevelID).HasCompletedExact(Difficulty))
				{
					GDOwner.Profile.SetCustomLevelCompleted(Blueprint.CustomMeta_LevelID, Difficulty, ctime);
				}

				GDOwner.SaveProfile();
				ShowScorePanel(Blueprint, GDOwner.Profile, null, true, 0, ctime);
				MainGame.Inst.GDSound.PlayEffectGameWon();

				EndGameConvert(winner);
			}
			else
			{
				ShowScorePanel(Blueprint, GDOwner.Profile, null, false, 0, ctime);

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

			MainGame.Inst.SetOverworldScreenWithSCCM(SCCMMainPanel.SCCMTab.Hot);
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
			if (Blueprint.CustomMusic != -1) 
				MainGame.Inst.GDSound.PlayMusicLevel(Blueprint.CustomMusic);
			else
				MainGame.Inst.GDSound.StopSong();
		}
	}
}
