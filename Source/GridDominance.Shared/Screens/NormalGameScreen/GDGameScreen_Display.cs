using System.Collections.Generic;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Screens.HUD;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Background;

namespace GridDominance.Shared.Screens.NormalGameScreen
{
	class GDGameScreen_Display : GDGameScreen
	{
		protected override GameHUD CreateHUD() => new EmptyGameHUD(this, Textures.HUDFontRegular);

		public override Fraction LocalPlayerFraction => null;

		public GDGameScreen_Display(MainGame game, GraphicsDeviceManager gdm, LevelBlueprint b) 
			: base(game, gdm, b, FractionDifficulty.DIFF_3, true, false, false)
		{
			Background = new GDEmptyGridBackground(this, (GameWrapMode)b.WrapMode);
		}

		public override void ExitToMap(bool updateSpeed)
		{
			//
		}

		public override void ReplayLevel(FractionDifficulty diff)
		{
			//
		}

		public override void RestartLevel(bool updateSpeed)
		{
			//
		}

		public override void ShowScorePanel(LevelBlueprint lvl, PlayerProfile profile, HashSet<FractionDifficulty> newDifficulty, bool playerHasWon, int addPoints, int time)
		{
			//
		}

		protected override void TestForGameEndingCondition()
		{
			//
		}

		public override AbstractFractionController CreateController(Fraction f, Cannon cannon)
		{
			return new EmptyController(this, cannon, f);
		}
	}
}
