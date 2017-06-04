using System;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Agents;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using GridDominance.Shared.Screens.NormalGameScreen.HUD;

namespace GridDominance.Shared.Screens.NormalGameScreen
{
	class GDGameScreen_Tutorial : GDGameScreen
	{
		public GDGameScreen_Tutorial(MainGame game, GraphicsDeviceManager gdm, LevelBlueprint bp) : base(game, gdm, Levels.LEVEL_TUTORIAL, FractionDifficulty.DIFF_0)
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
			HUD.AddModal(new HUDTutorialScorePanel(profile, addPoints), false);
		}

		public override void ExitToMap()
		{
			MainGame.Inst.SetOverworldScreen();
		}
	}
}
