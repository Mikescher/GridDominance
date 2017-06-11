using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.NormalGameScreen;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen;
using GridDominance.Shared.Screens.NormalGameScreen.Background;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.BatchRenderer;
using System.Collections.Generic;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Agents;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork;

namespace GridDominance.Shared.Screens.ScreenGame
{
	public abstract class GDGameScreen : GameScreen
	{
		public const float GAMESPEED_SUPERSLOW = 0.25f;
		public const float GAMESPEED_SLOW      = 0.5f;
		public const float GAMESPEED_NORMAL    = 1f;
		public const float GAMESPEED_SEMIFAST  = 1.5f;
		public const float GAMESPEED_FAST      = 2f;
		public const float GAMESPEED_SUPERFAST = 4f;

		//-----------------------------------------------------------------

		public MainGame GDOwner => (MainGame)Game;
		public IGDGridBackground GDBackground => (IGDGridBackground) Background;
		public GDEntityManager GDEntities => (GDEntityManager)Entities;

		//-----------------------------------------------------------------

		protected override EntityManager CreateEntityManager() => new GDEntityManager(this);
		protected override GameBackground CreateBackground() => new GDStaticGridBackground(this);
		protected override SAMViewportAdapter CreateViewport() => new TolerantBoxingViewportAdapter(Game.Window, Graphics, GDConstants.VIEW_WIDTH, GDConstants.VIEW_HEIGHT);
		protected override DebugMinimap CreateDebugMinimap() => new StandardDebugMinimapImplementation(this, 192, 32);
		protected override FRectangle CreateMapFullBounds() => new FRectangle(0, 0, 1, 1);
		protected override float GetBaseTextureScale() => Textures.DEFAULT_TEXTURE_SCALE_F;

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

		public readonly LevelBlueprint Blueprint;
		public readonly FractionDifficulty Difficulty;
		public readonly LaserNetwork LaserNetwork;

		public bool HasFinished = false;
		public float LevelTime = 0f;

		public readonly bool IsPreview;
		
		protected GDGameScreen(MainGame game, GraphicsDeviceManager gdm, LevelBlueprint bp, FractionDifficulty diff, bool prev) : base(game, gdm)
		{
			Blueprint = bp;
			Difficulty = diff;
			IsPreview = prev;

			LaserNetwork = new LaserNetwork(GetPhysicsWorld(), this);

			Initialize();
		}

#if DEBUG
		public Fraction GetPlayerFraction() // Only DEBUG - later there can be games w/o PlayerFraction
		{
			return fractionPlayer;
		}
#endif

		public Fraction GetNeutralFraction()
		{
			return fractionNeutral;
		}

		private void Initialize()
		{
			ConvertUnits.SetDisplayUnitToSimUnitRatio(GDConstants.PHYSICS_CONVERSION_FACTOR);

#if DEBUG
			if (!IsPreview)
			{
				DebugUtils.CreateShortcuts(this);
				DebugDisp = DebugUtils.CreateDisplay(this);
			}
#endif

			//--------------------

			fractionNeutral = Fraction.CreateNeutralFraction();
			fractionPlayer = Fraction.CreatePlayerFraction(fractionNeutral);
			fractionComputer1 = Fraction.CreateComputerFraction(Fraction.COLOR_COMPUTER_01, fractionNeutral, Difficulty);
			fractionComputer2 = Fraction.CreateComputerFraction(Fraction.COLOR_COMPUTER_02, fractionNeutral, Difficulty);
			fractionComputer3 = Fraction.CreateComputerFraction(Fraction.COLOR_COMPUTER_03, fractionNeutral, Difficulty);

			LoadLevelFromBlueprint();
		}

		private void LoadLevelFromBlueprint()
		{
			Fraction[] fracList = { fractionNeutral, fractionPlayer, fractionComputer1, fractionComputer2, fractionComputer3 };

			//----------------------------------------------------------------

			MapFullBounds = new FRectangle(0, 0, Blueprint.LevelWidth, Blueprint.LevelHeight);
			MapViewportCenterX = Blueprint.LevelViewX;
			MapViewportCenterY = Blueprint.LevelViewY;

			if (MainGame.Inst.Profile.EffectsEnabled) Background = new GDCellularBackground(this, Blueprint);

			//----------------------------------------------------------------

			var cannonList = new List<Cannon>();
			var portalList = new List<Portal>();

			foreach (var bPrint in Blueprint.BlueprintCannons)
			{
				var e = new BulletCannon(this, bPrint, fracList);
				Entities.AddEntity(e);
				cannonList.Add(e);
			}

			foreach (var bPrint in Blueprint.BlueprintVoidWalls)
			{
				var e = new VoidWall(this, bPrint);
				Entities.AddEntity(e);
			}

			foreach (var bPrint in Blueprint.BlueprintVoidCircles)
			{
				var e = new VoidCircle(this, bPrint);
				Entities.AddEntity(e);
			}

			foreach (var bPrint in Blueprint.BlueprintGlassBlocks)
			{
				var e = new GlassBlock(this, bPrint);
				Entities.AddEntity(e);
			}

			foreach (var bPrint in Blueprint.BlueprintBlackHoles)
			{
				var e = new BlackHole(this, bPrint);
				Entities.AddEntity(e);
			}

			foreach (var bPrint in Blueprint.BlueprintPortals)
			{
				var e = new Portal(this, bPrint);
				Entities.AddEntity(e);
				portalList.Add(e);
			}

			foreach (var bPrint in Blueprint.BlueprintLaserCannons)
			{
				var e = new LaserCannon(this, bPrint, fracList);
				Entities.AddEntity(e);
				cannonList.Add(e);
			}

			foreach (var bPrint in Blueprint.BlueprintMirrorBlocks)
			{
				var e = new MirrorBlock(this, bPrint);
				Entities.AddEntity(e);
			}

			foreach (var bPrint in Blueprint.BlueprintMirrorCircles)
			{
				var e = new MirrorCircle(this, bPrint);
				Entities.AddEntity(e);
			}

			//----------------------------------------------------------------

			foreach (var cannon in cannonList)
				cannon.OnAfterLevelLoad();


			foreach (var portal in portalList)
				portal.OnAfterLevelLoad(portalList);

			//----------------------------------------------------------------

			if (!IsPreview) AddAgent(new GameDragAgent(this));
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
#if DEBUG
			DebugDisp.IsEnabled = DebugSettings.Get("DebugTextDisplay");
			DebugDisp.Scale = 0.75f;
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

		protected virtual void TestForGameEndingCondition()
		{
			if (HasFinished) return;

			bool hasPlayer = false;
			bool hasComputer = false;
			
			foreach (var cannon in Entities.Enumerate().OfType<Cannon>())
			{
				if (cannon.Fraction.IsPlayer) hasPlayer = true;
				if (cannon.Fraction.IsComputer) hasComputer = true;
			}

			if (hasPlayer && !hasComputer) EndGame(true);
			if (!hasPlayer && hasComputer) EndGame(false);
		}

		private void EndGame(bool playerWon)
		{
			HasFinished = true;

			if (playerWon)
			{
				if (GDOwner.Profile.GetLevelData(Blueprint.UniqueID).HasCompleted(Difficulty))
				{
					ShowScorePanel(Blueprint, GDOwner.Profile, null, true, 0);
				}
				else
				{
					var ctime = (int) (LevelTime * 1000);

					int p = FractionDifficultyHelper.GetScore(Difficulty);
					GDOwner.Profile.SetCompleted(Blueprint.UniqueID, Difficulty, ctime, true);

					// Fake the online data until next sync
					var localdata = GDOwner.Profile.LevelData[Blueprint.UniqueID].Data[Difficulty];
					localdata.GlobalCompletionCount++;
					if (ctime < localdata.BestTime) {localdata.GlobalBestTime = ctime; localdata.GlobalBestUserID = GDOwner.Profile.OnlineUserID; }

					GDOwner.SaveProfile();
					ShowScorePanel(Blueprint, GDOwner.Profile, Difficulty, true, p);
				}

				MainGame.Inst.GDSound.PlayEffectGameWon();
			}
			else
			{
				ShowScorePanel(Blueprint, GDOwner.Profile, null, false, 0);

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

		public abstract void RestartLevel();
		public abstract void ReplayLevel(FractionDifficulty diff);
		public abstract void ShowScorePanel(LevelBlueprint lvl, PlayerProfile profile, FractionDifficulty? newDifficulty, bool playerHasWon, int addPoints);
		public abstract void ExitToMap();
		public abstract AbstractFractionController CreateController(Fraction f, Cannon cannon);
	}
}
