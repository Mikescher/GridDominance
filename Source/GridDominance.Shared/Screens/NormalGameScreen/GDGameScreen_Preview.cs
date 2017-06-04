using System;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;

namespace GridDominance.Shared.Screens.NormalGameScreen
{
	class GDGameScreen_Preview : GDGameScreen
	{
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
	}
}
