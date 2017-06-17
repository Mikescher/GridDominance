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
using GridDominance.Shared.Screens.OverworldScreen.HUD;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.NormalGameScreen
{
	class GDGameScreen_Preview : GDGameScreen
	{
		private const float PREVIEW_TIME = 7f;

		protected override GameHUD CreateHUD() => new GDPreviewHUD(this);

		private readonly LevelBlueprint[] _blueprints;
		private readonly int _blueprintIndex;
		private readonly WorldPreviewPanel _parent;

		private float _runtime = 0;

		public GDGameScreen_Preview(MainGame game, GraphicsDeviceManager gdm, WorldPreviewPanel pnl, LevelBlueprint[] bps, int idx) : base(game, gdm, bps[idx], FractionDifficulty.DIFF_3, true)
		{
			GameSpeed = GAMESPEED_SEMIFAST;

			_blueprints     = bps;
			_blueprintIndex = idx;
			_parent         = pnl;
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

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			base.OnUpdate(gameTime, istate);

			if ((_runtime += gameTime.RealtimeElapsedSeconds) > PREVIEW_TIME && FloatMath.PercSin(gameTime.TotalElapsedSeconds*2) > 0.85f)
			{
				_parent.SetNextScreen((_blueprintIndex + 1) % _blueprints.Length);
			}
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
					return new StandardKIController(this, cannon, f, cannon is LaserCannon);

				case FractionType.NeutralFraction:
					return new NeutralKIController(this, cannon, f);

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
