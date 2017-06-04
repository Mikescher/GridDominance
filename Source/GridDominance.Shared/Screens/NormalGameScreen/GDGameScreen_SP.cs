using System;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using GridDominance.Shared.Screens.NormalGameScreen.HUD;

namespace GridDominance.Shared.Screens.NormalGameScreen
{
	class GDGameScreen_SP : GDGameScreen
	{
		public readonly GraphBlueprint WorldBlueprint;

		public GDGameScreen_SP(MainGame game, GraphicsDeviceManager gdm, LevelBlueprint bp, FractionDifficulty diff, GraphBlueprint ws) : base(game, gdm, bp, diff)
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
			HUD.AddModal(new HUDScorePanel(lvl, profile, newDifficulty, playerHasWon, addPoints), false);
		}

		public override void ExitToMap()
		{
			MainGame.Inst.SetWorldMapScreenZoomedOut(WorldBlueprint, Blueprint.UniqueID);
		}
	}
}
