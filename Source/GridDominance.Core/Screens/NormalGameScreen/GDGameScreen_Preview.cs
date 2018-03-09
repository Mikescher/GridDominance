using System;
using System.Collections.Generic;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using Microsoft.Xna.Framework;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Screens.HUD;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Entities.Cannons;
using GridDominance.Shared.Screens.NormalGameScreen.HUD;
using GridDominance.Shared.Screens.OverworldScreen.HUD;
using GridDominance.Shared.Screens.OverworldScreen.HUD.Dialogs;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.NormalGameScreen
{
	class GDGameScreen_Preview : GDGameScreen
	{
		private const float PREVIEW_TIME = 7f;

		protected override GameHUD CreateHUD() => new EmptyGameHUD(this, Textures.HUDFontRegular);

		private readonly LevelBlueprint[] _blueprints;
		private readonly int _blueprintIndex;
		private readonly WorldPreviewPanel _parent;

		private float _runtime = 0;

		public override Fraction LocalPlayerFraction => null;

		public GDGameScreen_Preview(MainGame game, GraphicsDeviceManager gdm, WorldPreviewPanel pnl, LevelBlueprint[] bps, int idx, int worldnumber) 
			: base(game, gdm, bps[idx], FractionDifficulty.DIFF_3, true, false, false)
		{
			GameSpeed = GAMESPEED_SEMIFAST;

			_blueprints     = bps;
			_blueprintIndex = idx;
			_parent         = pnl;

			GameHUD = new GDPreviewHUD(this, worldnumber);
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

		public override void ShowScorePanel(LevelBlueprint lvl, PlayerProfile profile, HashSet<FractionDifficulty> newDifficulties, bool playerHasWon, int addPoints, int time)
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
				case FractionType.ComputerFraction:
					return cannon.CreateKIController(this, f);

				case FractionType.NeutralFraction:
					return cannon.CreateNeutralController(this, f);

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
