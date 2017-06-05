using System;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Screens.HUD;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.HUD;

namespace GridDominance.Shared.Screens.NormalGameScreen
{
	class GDGameScreen_Preview : GDGameScreen
	{
		protected override GameHUD CreateHUD() => new GDPreviewHUD(this);

		public GDGameScreen_Preview(MainGame game, GraphicsDeviceManager gdm, LevelBlueprint bp) : base(game, gdm, bp, FractionDifficulty.DIFF_3)
		{
			//
		}

		public override void ExitToMap()
		{
			//
		}

		public override void ReplayLevel(FractionDifficulty diff)
		{
			//
		}

		public override void RestartLevel()
		{
			//
		}

		public override void ShowScorePanel(LevelBlueprint lvl, PlayerProfile profile, FractionDifficulty? newDifficulty, bool playerHasWon, int addPoints)
		{
			//
		}

		protected override void TestForGameEndingCondition()
		{
			// the ride never ends
		}

		public override AbstractFractionController CreateController(Fraction f, Cannon cannon)
		{
			switch (f.Type)
			{
				case FractionType.PlayerFraction:
				case FractionType.ComputerFraction:
					return new StandardKIController(this, cannon, f);

				case FractionType.NeutralFraction:
					return new NeutralKIController(this, cannon, f);

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
