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
using MonoSAMFramework.Portable.Screens.Entities.Particles;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.Extensions;
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
		private float levelTime = 0f;

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
			DebugSettings.AddSwitch(null, "DBG", this, KCL.C(SKeys.D, SKeys.AndroidMenu), true);

			DebugSettings.AddTrigger("DBG", "SetQuality_1", this, SKeys.D1, KeyModifier.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.FD));
			DebugSettings.AddTrigger("DBG", "SetQuality_2", this, SKeys.D2, KeyModifier.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.BD));
			DebugSettings.AddTrigger("DBG", "SetQuality_3", this, SKeys.D3, KeyModifier.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.LD));
			DebugSettings.AddTrigger("DBG", "SetQuality_4", this, SKeys.D4, KeyModifier.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.MD));
			DebugSettings.AddTrigger("DBG", "SetQuality_5", this, SKeys.D5, KeyModifier.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.HD));

			DebugSettings.AddSwitch("DBG", "PhysicsDebugView",      this, SKeys.F1, KeyModifier.None, false);
			DebugSettings.AddSwitch("DBG", "DebugTextDisplay",      this, SKeys.F2, KeyModifier.None, true);
			DebugSettings.AddSwitch("DBG", "DebugBackground",       this, SKeys.F3, KeyModifier.None, false);
			DebugSettings.AddSwitch("DBG", "DebugHUDBorders",       this, SKeys.F4, KeyModifier.None, false);
			DebugSettings.AddSwitch("DBG", "DebugCannonView",       this, SKeys.F5, KeyModifier.None, true);
			DebugSettings.AddSwitch("DBG", "ShowMatrixTextInfos",   this, SKeys.F6, KeyModifier.None, false);
			DebugSettings.AddSwitch("DBG", "ShowDebugMiniMap",      this, SKeys.F7, KeyModifier.None, false);
			DebugSettings.AddSwitch("DBG", "DebugEntityBoundaries", this, SKeys.F8, KeyModifier.None, false);
			DebugSettings.AddSwitch("DBG", "DebugEntityMouseAreas", this, SKeys.F9, KeyModifier.None, false);

			DebugSettings.AddPush("DBG", "ShowDebugShortcuts",      this, SKeys.Tab, KeyModifier.None);
			DebugSettings.AddPush("DBG", "ShowSerializedProfile",   this, SKeys.O, KeyModifier.None);

			DebugSettings.AddPush("DBG", "AssimilateCannon",        this, SKeys.A, KeyModifier.None);
			DebugSettings.AddPush("DBG", "AbandonCannon",           this, SKeys.S, KeyModifier.None);
#endif

#if DEBUG
			DebugDisp = new DebugTextDisplay(Graphics.GraphicsDevice, Textures.DebugFont);
			{
				DebugDisp.AddLine(() => $"FPS = {FPSCounter.AverageAPS:0000.0} (current = {FPSCounter.CurrentAPS:0000.0} | delta = {FPSCounter.AverageDelta * 1000:000.00} | min = {FPSCounter.MinimumAPS:0000.0} | total = {FPSCounter.TotalActions:000000})");
				DebugDisp.AddLine(() => $"UPS = {UPSCounter.AverageAPS:0000.0} (current = {UPSCounter.CurrentAPS:0000.0} | delta = {UPSCounter.AverageDelta * 1000:000.00} | min = {UPSCounter.MinimumAPS:0000.0} | total = {UPSCounter.TotalActions:000000}) | Speed={GameSpeed:00.00}|{GameSpeedMode}");
				DebugDisp.AddLine(() => $"Quality = {Textures.TEXTURE_QUALITY} | Texture.Scale={1f / Textures.DEFAULT_TEXTURE_SCALE.X:#.00} | Pixel.Scale={Textures.GetDeviceTextureScaling(Game.GraphicsDevice):#.00} | Viewport=[{Game.GraphicsDevice.Viewport.Width}|{Game.GraphicsDevice.Viewport.Height}]");
				DebugDisp.AddLine(() => $"Entities = {Entities.Count(),3} | Particles = {Entities.Enumerate().OfType<IParticleOwner>().Sum(p => p.ParticleCount),3} (Visible: {Entities.Enumerate().Where(p => p.IsInViewport).OfType<IParticleOwner>().Sum(p => p.ParticleCount),3})"); DebugDisp.AddLine(() => $"HUD.Size=(T:{GameHUD.Top}|L:{GameHUD.Left}|R:{GameHUD.Right}|B:{GameHUD.Bottom}) | HUD.AllElements={GameHUD.DeepCount()} | HUD.RootElements={GameHUD.FlatCount()} | Background.Particles={GDBackground.ParticleCount,3}");
				DebugDisp.AddLine(() => $"Pointer = ({InputStateMan.GetCurrentState().PointerPosition.X:000.0}|{InputStateMan.GetCurrentState().PointerPosition.Y:000.0}) | PointerOnMap = ({InputStateMan.GetCurrentState().PointerPositionOnMap.X:000.0}|{InputStateMan.GetCurrentState().PointerPositionOnMap.Y:000.0})");
				DebugDisp.AddLine(() => $"OGL Sprites = {LastReleaseRenderSpriteCount:0000} (+ {LastDebugRenderSpriteCount:0000}); OGL Text = {LastReleaseRenderTextCount:0000} (+ {LastDebugRenderTextCount:0000})");
				DebugDisp.AddLine(() => $"LevelTime = {levelTime:000.000} (finished={HasFinished})");

				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"GraphicsDevice.Viewport=[{Game.GraphicsDevice.Viewport.Width}|{Game.GraphicsDevice.Viewport.Height}]");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.VirtualGuaranteedSize={VAdapter.VirtualGuaranteedSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.RealGuaranteedSize={VAdapter.RealGuaranteedSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.VirtualTotalSize={VAdapter.VirtualTotalSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.RealTotalSize={VAdapter.RealTotalSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.VirtualOffset={VAdapter.VirtualGuaranteedBoundingsOffset}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.RealOffset={VAdapter.RealGuaranteedBoundingsOffset}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.Scale={VAdapter.Scale}");

				DebugDisp.AddLine("ShowDebugShortcuts", DebugSettings.GetSummary);

				DebugDisp.AddLogLines(SAMLogLevel.DEBUG);

				DebugDisp.AddLine("ShowSerializedProfile", () => MainGame.Inst.Profile.SerializeToString(128));

				DebugDisp.AddLine("FALSE", () => InputStateMan.GetCurrentState().GetFullDebugSummary());
			}

			//InputStateMan.PointerDown += (o, a) => DebugDisp.AddDecayLine($"Mouse::OnDown({a.X:0000}|{a.Y:0000})", 0.75f, 0.5f, 0.25f);
			//InputStateMan.PointerUp += (o, a) => DebugDisp.AddDecayLine($"Mouse::OnUp({a.X:0000}|{a.Y:0000})", 0.75f, 0.5f, 0.25f);
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
			if (!IsPaused && !HasFinished) levelTime += MonoSAMGame.CurrentTime.ElapsedSeconds;

			TestForGameEndingCondition();
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
					GDOwner.Profile.SetCompleted(Blueprint.UniqueID, difficulty, (int)(levelTime * 1000), true);
					GDOwner.SaveProfile();
					GDGameHUD.ShowScorePanel(Blueprint, GDOwner.Profile, difficulty, true, p);
				}

			}
			else
			{
				GDGameHUD.ShowScorePanel(Blueprint, GDOwner.Profile, null, false, 0);
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
