using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using GridDominance.Shared.Screens.ScreenGame.Background;
using GridDominance.Shared.Screens.ScreenGame.Entities;
using GridDominance.Shared.Screens.ScreenGame.hud;
using GridDominance.Levelformat.Parser;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.HUD;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Shared.Screens.ScreenGame
{
	class GDGameScreen : GameScreen
	{
		public const float GAMESPEED_SUPERSLOW = 0.25f;
		public const float GAMESPEED_SLOW      = 0.5f;
		public const float GAMESPEED_NORMAL    = 1f;
		public const float GAMESPEED_FAST      = 2f;
		public const float GAMESPEED_SUPERFAST = 4f;

		//-----------------------------------------------------------------
		public MainGame GDOwner => (MainGame)Game;

		public IGDGridBackground GDBackground => (IGDGridBackground) Background;
		public GDEntityManager GDEntities => (GDEntityManager)Entities;
		public GDGameHUD GDGameHUD => (GDGameHUD) GameHUD;
		//-----------------------------------------------------------------

		private bool _isPaused = false;
		public bool IsPaused
		{
			get { return _isPaused; }
			set
			{
				if (value == _isPaused) return;
				_isPaused = value;
				UpdateGameSpeed();
			}			
		}

		private GameSpeedModes _gameSpeedMode = GameSpeedModes.NORMAL;
		public GameSpeedModes GameSpeedMode
		{
			get { return _gameSpeedMode; }
			set
			{
				if (value == _gameSpeedMode) return;
				_gameSpeedMode = value;
				UpdateGameSpeed();
			}
		}

		private Fraction fractionNeutral;
		private Fraction fractionPlayer;
		private Fraction fractionComputer1;
		private Fraction fractionComputer2;
		private Fraction fractionComputer3;

		public readonly LevelFile Blueprint;
		private readonly FractionDifficulty difficulty;

		public bool HasFinished = false;
		public float LevelTime = 0f;

		public GDGameScreen(MainGame game, GraphicsDeviceManager gdm, LevelFile bp, FractionDifficulty diff) : base(game, gdm)
		{
			Blueprint = bp;
			difficulty = diff;

			Initialize();
		}

#if DEBUG
		public Fraction GetPlayerFraction() // Only DEBUG - later there can be games w/o PlayerFraction
		{
			return fractionPlayer;
		}
#endif

		private void Initialize()
		{
			ConvertUnits.SetDisplayUnitToSimUnitRatio(GDSettings.PHYSICS_CONVERSION_FACTOR);

#if DEBUG
			DebugUtils.CreateShortcuts(this);
			DebugDisp = DebugUtils.CreateDisplay(this);
#endif

			//--------------------

			fractionNeutral = Fraction.CreateNeutralFraction();
			fractionPlayer = Fraction.CreatePlayerFraction(fractionNeutral);
			fractionComputer1 = Fraction.CreateComputerFraction(Fraction.COLOR_COMPUTER_01, fractionNeutral, difficulty);
			fractionComputer2 = Fraction.CreateComputerFraction(Fraction.COLOR_COMPUTER_02, fractionNeutral, difficulty);
			fractionComputer3 = Fraction.CreateComputerFraction(Fraction.COLOR_COMPUTER_03, fractionNeutral, difficulty);

			LoadLevelFromBlueprint();
		}


		protected override EntityManager CreateEntityManager() => new GDEntityManager(this);
		protected override GameHUD CreateHUD() => new GDGameHUD(this);
		protected override GameBackground CreateBackground() => MainGame.Inst.Profile.EffectsEnabled ? (GameBackground)new GDGridBackground(this) : new GDStaticGridBackground(this);
		protected override SAMViewportAdapter CreateViewport() => new TolerantBoxingViewportAdapter(Game.Window, Graphics, GDConstants.VIEW_WIDTH, GDConstants.VIEW_HEIGHT);
		protected override DebugMinimap CreateDebugMinimap() => new GDScreenDebugMinimap(this);
		protected override FRectangle CreateMapFullBounds() => new FRectangle(0, 0, GDConstants.VIEW_WIDTH, GDConstants.VIEW_HEIGHT);
		protected override float GetBaseTextureScale() => Textures.DEFAULT_TEXTURE_SCALE_F;

		private void LoadLevelFromBlueprint()
		{
			Fraction[] fracList =
			{
				fractionNeutral, fractionPlayer, fractionComputer1, fractionComputer2, fractionComputer3,
			};

			foreach (var bPrint in Blueprint.BlueprintCannons)
			{
				Entities.AddEntity(new Cannon(this, bPrint, fracList));
			}
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
#if DEBUG
			DebugDisp.IsEnabled = DebugSettings.Get("DebugTextDisplay");
			DebugDisp.Scale = 0.75f;

			if (SAMLog.Entries.Any()) DebugSettings.SetManual("DebugTextDisplay", true);
#endif
			if (!IsPaused && !HasFinished) LevelTime += MonoSAMGame.CurrentTime.ElapsedSeconds;

			TestForGameEndingCondition();
		}

		protected override void OnDrawGame(IBatchRenderer sbatch)
		{
			//
		}

		protected override void OnDrawHUD(IBatchRenderer sbatch)
		{
			//
		}

		private void TestForGameEndingCondition()
		{
			if (HasFinished) return;

			Fraction winningFraction = null;

			foreach (var cannon in Entities.Enumerate().OfType<Cannon>())
			{
				if (cannon.Fraction.IsNeutral) continue;

				if (winningFraction == null)
				{
					winningFraction = cannon.Fraction;
				}
				else if (winningFraction != cannon.Fraction)
				{
					return;
				}
			}

			EndGame(winningFraction);
		}

		private void EndGame(Fraction winner)
		{
			HasFinished = true;

			if (winner.IsPlayer)
			{
				if (GDOwner.Profile.GetLevelData(Blueprint.UniqueID).HasCompleted(difficulty))
				{
					GDGameHUD.ShowScorePanel(Blueprint, GDOwner.Profile, null, true, 0);
				}
				else
				{
					int p = FractionDifficultyHelper.GetScore(difficulty);
					GDOwner.Profile.SetCompleted(Blueprint.UniqueID, difficulty, (int)(LevelTime * 1000), true);
					GDOwner.SaveProfile();
					GDGameHUD.ShowScorePanel(Blueprint, GDOwner.Profile, difficulty, true, p);
				}

				MainGame.Inst.GDSound.PlayEffectGameWon();
			}
			else
			{
				GDGameHUD.ShowScorePanel(Blueprint, GDOwner.Profile, null, false, 0);

				MainGame.Inst.GDSound.PlayEffectGameOver();
			}

			foreach (var cannon in Entities.Enumerate().OfType<Cannon>())
			{
				cannon.ForceUpdateController();
			}

		}

		public World GetPhysicsWorld()
		{
			return GDEntities.PhysicsWorld;
		}

		public override void Resize(int width, int height)
		{
			base.Resize(width, height);

#if DEBUG
			var newQuality = Textures.GetPreferredQuality(Game.GraphicsDevice);
			if (newQuality != Textures.TEXTURE_QUALITY)
			{
				Textures.ChangeQuality(Game.Content, newQuality);
			}
#endif
		}

		private void UpdateGameSpeed()
		{
			if (IsPaused)
			{
				GameSpeed = 0f;
			}
			else
			{
				switch (GameSpeedMode)
				{
					case GameSpeedModes.SUPERSLOW:
						GameSpeed = GAMESPEED_SUPERSLOW;
						break;
					case GameSpeedModes.SLOW:
						GameSpeed = GAMESPEED_SLOW;
						break;
					case GameSpeedModes.NORMAL:
						GameSpeed = GAMESPEED_NORMAL;
						break;
					case GameSpeedModes.FAST:
						GameSpeed = GAMESPEED_FAST;
						break;
					case GameSpeedModes.SUPERFAST:
						GameSpeed = GAMESPEED_SUPERFAST;
						break;
				}
			}
		}

		public void RestartLevel()
		{
			GDOwner.SetLevelScreen(Blueprint, difficulty);
		}

		public void ReplayLevel(FractionDifficulty diff)
		{
			GDOwner.SetLevelScreen(Blueprint, diff);
		}
	}
}
