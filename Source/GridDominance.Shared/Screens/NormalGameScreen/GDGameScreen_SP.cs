using System;
using System.Collections.Generic;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using GridDominance.Shared.Screens.NormalGameScreen.HUD;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.NormalGameScreen
{
	class GDGameScreen_SP : GDGameScreen
	{
		protected override GameHUD CreateHUD() => new GDGameHUD(this);

		public readonly GraphBlueprint WorldBlueprint;

		public override Fraction LocalPlayerFraction => fractionPlayer;

		public GDGameScreen_SP(MainGame game, GraphicsDeviceManager gdm, LevelBlueprint bp, FractionDifficulty diff, GraphBlueprint ws) 
			: base(game, gdm, bp, diff, false, false)
		{
			WorldBlueprint = ws;
		}

		public override void RestartLevel()
		{
			GDOwner.SetLevelScreen(Blueprint, Difficulty, WorldBlueprint, GameSpeedMode);
		}

		public override void ReplayLevel(FractionDifficulty diff)
		{
			GDOwner.SetLevelScreen(Blueprint, diff, WorldBlueprint);
		}

		public override void ShowScorePanel(LevelBlueprint lvl, PlayerProfile profile, HashSet<FractionDifficulty> newDifficulties, bool playerHasWon, int addPoints)
		{
			((GDGameHUD)HUD).BtnPause.IsEnabled = false;
			((GDGameHUD)HUD).BtnSpeed.IsEnabled = false;

			GameSpeedMode = GameSpeedModes.NORMAL;

			HUD.AddModal(new HUDScorePanel(lvl, profile, newDifficulties, playerHasWon, addPoints), false);
		}

		public override void ExitToMap()
		{
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
					return new NeutralKIController(this, cannon, f);

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
