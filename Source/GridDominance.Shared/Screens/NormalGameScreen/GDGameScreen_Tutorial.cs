using System;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Agents;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using GridDominance.Shared.Screens.NormalGameScreen.HUD;
using MonoSAMFramework.Portable.Screens.HUD;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;

namespace GridDominance.Shared.Screens.NormalGameScreen
{
	class GDGameScreen_Tutorial : GDGameScreen
	{
		protected override GameHUD CreateHUD() => new GDGameHUD(this);

		public GDGameScreen_Tutorial(MainGame game, GraphicsDeviceManager gdm, LevelBlueprint bp) : base(game, gdm, Levels.LEVEL_TUTORIAL, FractionDifficulty.DIFF_0, false)
		{
			AddAgent(new TutorialAgent(this));
		}

		public override void RestartLevel()
		{
			GDOwner.SetTutorialLevelScreen();
		}

		public override void ReplayLevel(FractionDifficulty diff)
		{
			GDOwner.SetTutorialLevelScreen();
		}

		public override void ShowScorePanel(LevelBlueprint lvl, PlayerProfile profile, FractionDifficulty? newDifficulty, bool playerHasWon, int addPoints)
		{
			((GDGameHUD)HUD).BtnPause.IsEnabled = false;
			((GDGameHUD)HUD).BtnSpeed.IsEnabled = false;

			GameSpeedMode = GameSpeedModes.NORMAL;

			HUD.AddModal(new HUDTutorialScorePanel(profile, addPoints), false);
		}

		public override void ExitToMap()
		{
			MainGame.Inst.SetOverworldScreen();
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
					return cannon.CreateKIController(this, f);

				case FractionType.NeutralFraction:
					return new NeutralKIController(this, cannon, f);

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override void OnShow()
		{
			MainGame.Inst.GDSound.PlayMusicTutorial();
		}
	}
}
