using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using Microsoft.Xna.Framework;
using GridDominance.Shared.Screens.NormalGameScreen.HUD;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.Entities.Cannons;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using GridDominance.Shared.Screens.NormalGameScreen.HUD.ScorePanel;
using GridDominance.Shared.SCCM;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.NormalGameScreen
{
	class GDGameScreen_SCCMUpload : GDGameScreen
	{
		protected override GameHUD CreateHUD() => new GDGameHUD(this);

		public readonly SCCMLevelData SCCMData;

		public override Fraction LocalPlayerFraction => fractionPlayer;

		private GameSpeedModes _lastSpeed;
		private readonly bool _toast;

		public GDGameScreen_SCCMUpload(MainGame game, GraphicsDeviceManager gdm, LevelBlueprint bp, SCCMLevelData dat, GameSpeedModes speed, bool toast) 
			: base(game, gdm, bp, FractionDifficulty.DIFF_3, false, false, false)
		{
			SCCMData = dat;
			_toast = toast;

			GameSpeedMode = _lastSpeed = speed;
			UpdateGameSpeed();
		}

		public override void RestartLevel(bool updateSpeed)
		{
			GDOwner.SetEditorUploadLevel(Blueprint, SCCMData, false, updateSpeed ? GameSpeedMode : _lastSpeed);
		}

		public override void ReplayLevel(FractionDifficulty diff)
		{
			GDOwner.SetEditorUploadLevel(Blueprint, SCCMData, false, GameSpeedMode);
		}

		public override void ShowScorePanel(LevelBlueprint lvl, PlayerProfile profile, HashSet<FractionDifficulty> newDifficulties, bool playerHasWon, int addPoints, int time)
		{
			((GDGameHUD)HUD).BtnPause.IsEnabled = false;
			((GDGameHUD)HUD).BtnSpeed.IsEnabled = false;

			GameSpeedMode = GameSpeedModes.NORMAL;

			HUD.AddModal(new HUDSCCMUploadScorePanel(lvl, SCCMData, Difficulty, _lastSpeed, playerHasWon, time), false);
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
			_lastSpeed = GameSpeedMode;

			HasFinished = true;
			PlayerWon = playerWon;

			var ctime = (int)(LevelTime * 1000);

			if (playerWon)
			{
				ShowScorePanel(Blueprint, GDOwner.Profile, new HashSet<FractionDifficulty>(), true, 0, ctime);
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
			MainGame.Inst.SetLevelEditorScreen(SCCMData);
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

			if (_toast) HUD.ShowToast("SCCMUpload:Notif", L10N.T(L10NImpl.STR_SCCM_UPLOADINFO), 40, FlatColors.Silver, FlatColors.Foreground, 4f);
		}
	}
}
