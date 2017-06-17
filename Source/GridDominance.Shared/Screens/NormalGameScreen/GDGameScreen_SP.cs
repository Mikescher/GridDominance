using System;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using GridDominance.Shared.Screens.NormalGameScreen.HUD;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.NormalGameScreen
{
	class GDGameScreen_SP : GDGameScreen
	{
		protected override GameHUD CreateHUD() => new GDGameHUD(this);

		public readonly GraphBlueprint WorldBlueprint;

		public GDGameScreen_SP(MainGame game, GraphicsDeviceManager gdm, LevelBlueprint bp, FractionDifficulty diff, GraphBlueprint ws) : base(game, gdm, bp, diff, false)
		{
			WorldBlueprint = ws;
		}

		public override void RestartLevel()
		{
			GDOwner.SetLevelScreen(Blueprint, Difficulty, WorldBlueprint);
		}

		public override void ReplayLevel(FractionDifficulty diff)
		{
			GDOwner.SetLevelScreen(Blueprint, diff, WorldBlueprint);
		}

		public override void ShowScorePanel(LevelBlueprint lvl, PlayerProfile profile, FractionDifficulty? newDifficulty, bool playerHasWon, int addPoints)
		{
			((GDGameHUD)HUD).BtnPause.IsEnabled = false;
			((GDGameHUD)HUD).BtnSpeed.IsEnabled = false;

			GameSpeedMode = GameSpeedModes.NORMAL;

			HUD.AddModal(new HUDScorePanel(lvl, profile, newDifficulty, playerHasWon, addPoints), false);
		}

		public override void ExitToMap()
		{
			MainGame.Inst.SetWorldMapScreenZoomedOut(WorldBlueprint, Blueprint.UniqueID);
		}

		public override AbstractFractionController CreateController(Fraction f, Cannon cannon)
		{
			if (HasFinished)
				return new EndGameAutoPlayerController(this, cannon, f); //TODO only rotate when player won
			
			switch (f.Type)
			{
				case FractionType.PlayerFraction:
					return new PlayerController(this, cannon, f);

				case FractionType.ComputerFraction:
					return new StandardKIController(this, cannon, f, cannon is LaserCannon);

				case FractionType.NeutralFraction:
					return new NeutralKIController(this, cannon, f);

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
