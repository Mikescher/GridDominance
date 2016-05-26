using FarseerPhysics;
using FarseerPhysics.Dynamics;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame.Background;
using GridDominance.Shared.Screens.ScreenGame.Entities;
using GridDominance.Shared.Screens.ScreenGame.hud;
using GridDominance.Levelformat.Parser;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.ViewportAdapters;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.ScreenGame
{
	class GDGameScreen : GameScreen
	{
		public const int TILE_WIDTH = 64;

		public const int GRID_WIDTH = 16;
		public const int GRID_HEIGHT = 10;

		public const int VIEW_WIDTH  = GRID_WIDTH * TILE_WIDTH; // 1024
		public const int VIEW_HEIGHT = GRID_HEIGHT * TILE_WIDTH; // 640

		public const float GAMESPEED_SUPERSLOW = 0.25f;
		public const float GAMESPEED_SLOW      = 0.5f;
		public const float GAMESPEED_NORMAL    = 1f;
		public const float GAMESPEED_FAST      = 2f;
		public const float GAMESPEED_SUPERFAST = 4f;

		//-----------------------------------------------------------------
		public MainGame GDOwner => (MainGame)Owner;

		public GDGridBackground GDBackground => (GDGridBackground) Background;
		public GDEntityManager GDEntities => (GDEntityManager) Entities;
		public TolerantBoxingViewportAdapter GDViewport => (TolerantBoxingViewportAdapter) Viewport;

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

		private readonly LevelFile blueprint;

		public GDGameScreen(MainGame game, GraphicsDeviceManager gdm, LevelFile bp) : base(game, gdm)
		{
			blueprint = bp;

			Initialize();

#if DEBUG
			DebugSettings.AddTrigger("SetQuality_1", this, Keys.D1, KeyboardModifiers.Control, x => Textures.ChangeQuality(Owner.Content, TextureQuality.FD));
			DebugSettings.AddTrigger("SetQuality_2", this, Keys.D2, KeyboardModifiers.Control, x => Textures.ChangeQuality(Owner.Content, TextureQuality.BD));
			DebugSettings.AddTrigger("SetQuality_3", this, Keys.D3, KeyboardModifiers.Control, x => Textures.ChangeQuality(Owner.Content, TextureQuality.LD));
			DebugSettings.AddTrigger("SetQuality_4", this, Keys.D4, KeyboardModifiers.Control, x => Textures.ChangeQuality(Owner.Content, TextureQuality.MD));
			DebugSettings.AddTrigger("SetQuality_5", this, Keys.D5, KeyboardModifiers.Control, x => Textures.ChangeQuality(Owner.Content, TextureQuality.HD));

			DebugSettings.AddSwitch("PhysicsDebugView", this, Keys.F1, KeyboardModifiers.None, false);
			DebugSettings.AddSwitch("DebugTextDisplay", this, Keys.F2, KeyboardModifiers.None, true);
			DebugSettings.AddSwitch("DebugBackground", this,  Keys.F3, KeyboardModifiers.None, false);
			DebugSettings.AddSwitch("DebugHUDBorders", this,  Keys.F4, KeyboardModifiers.None, false);
			DebugSettings.AddSwitch("DebugCannonView", this,  Keys.F5, KeyboardModifiers.None, true);

			DebugSettings.AddPush("ShowDebugShortcuts", this, Keys.Tab, KeyboardModifiers.None);
#endif
		}

		private void Initialize()
		{
			ConvertUnits.SetDisplayUnitToSimUnitRatio(GDSettings.PHYSICS_CONVERSION_FACTOR);

#if DEBUG
			FPSCounter = new RealtimeAPSCounter();
			UPSCounter = new RealtimeAPSCounter();

			DebugDisp = new DebugTextDisplay(Graphics.GraphicsDevice, Textures.DebugFont);
			{
				DebugDisp.AddLine(() => $"FPS = {FPSCounter.AverageAPS:0000.0} (current = {FPSCounter.CurrentAPS:0000.0} | delta = {FPSCounter.AverageDelta * 1000:000.00} | min = {FPSCounter.MinimumAPS:0000.0} | total = {FPSCounter.TotalActions:000000})");
				DebugDisp.AddLine(() => $"UPS = {UPSCounter.AverageAPS:0000.0} (current = {UPSCounter.CurrentAPS:0000.0} | delta = {UPSCounter.AverageDelta * 1000:000.00} | min = {UPSCounter.MinimumAPS:0000.0} | total = {UPSCounter.TotalActions:000000})");
				DebugDisp.AddLine(() => $"Quality = {Textures.TEXTURE_QUALITY} | Texture.Scale={1f / Textures.DEFAULT_TEXTURE_SCALE.X:#.00} | Pixel.Scale={Textures.GetDeviceTextureScaling(Owner.GraphicsDevice):#.00} | Viewport=[{Owner.GraphicsDevice.Viewport.Width}|{Owner.GraphicsDevice.Viewport.Height}]");
				DebugDisp.AddLine(() => $"Entities = {Entities.Count(),3} | Particles = {GDBackground.Particles.Count,3} | Bodies = {GDEntities.PhysicsWorld.BodyList.Count,3}");
				DebugDisp.AddLine(() => $"HUD.Size=(T:{GameHUD.Top}|L:{GameHUD.Left}|R:{GameHUD.Right}|B:{GameHUD.Bottom}) | HUD.Elements={GameHUD.Count()}");
				DebugDisp.AddLine(() => $"Pointer = ({InputStateMan.GetCurrentState().PointerPosition.X:000.0}|{InputStateMan.GetCurrentState().PointerPosition.Y:000.0})");
				DebugDisp.AddLine(() => $"OGL Sprites = {((IDebugBatchRenderer)MainBatch).LastRenderSpriteCount:0000}; OGL Text = {((IDebugBatchRenderer)MainBatch).LastRenderTextCount:0000}");
				
				DebugDisp.AddLine(new DebugTextDisplayLine(DebugSettings.GetSummary, () => DebugSettings.Get("ShowDebugShortcuts")));
			}

			//InputStateMan.PointerDown += (o, a) => DebugDisp.AddDecayLine($"Mouse::OnDown({a.X:0000}|{a.Y:0000})", 0.75f, 0.5f, 0.25f);
			//InputStateMan.PointerUp += (o, a) => DebugDisp.AddDecayLine($"Mouse::OnUp({a.X:0000}|{a.Y:0000})", 0.75f, 0.5f, 0.25f);
#else
			DebugDisp = new DummyDebugTextDisplay();
#endif


			//--------------------

			fractionNeutral = Fraction.CreateNeutralFraction();
			fractionPlayer = Fraction.CreatePlayerFraction(fractionNeutral);
			fractionComputer1 = Fraction.CreateComputerFraction(Fraction.COLOR_COMPUTER_01, fractionNeutral, Fraction.MULTIPLICATOR_COMPUTER_0);
			fractionComputer2 = Fraction.CreateComputerFraction(Fraction.COLOR_COMPUTER_02, fractionNeutral, Fraction.MULTIPLICATOR_COMPUTER_0);
			fractionComputer3 = Fraction.CreateComputerFraction(Fraction.COLOR_COMPUTER_03, fractionNeutral, Fraction.MULTIPLICATOR_COMPUTER_0);

			LoadLevelFromBlueprint();
		}


		protected override EntityManager CreateEntityManager() => new GDEntityManager(this);
		protected override GameHUD CreateHUD() => new GDGameHUD(this);
		protected override GameBackground CreateBackground() => new GDGridBackground(this);
		protected override ViewportAdapter CreateViewport() => new TolerantBoxingViewportAdapter(Owner.Window, Graphics, VIEW_WIDTH, VIEW_HEIGHT);

		private void LoadLevelFromBlueprint()
		{
			Fraction[] fracList =
			{
				fractionNeutral, fractionPlayer, fractionComputer1, fractionComputer2, fractionComputer3,
			};

			foreach (var bPrint in blueprint.BlueprintCannons)
			{
				Entities.AddEntity(new Cannon(this, bPrint, fracList));
			}
		}

		protected override void OnUpdate(GameTime gameTime, InputState istate)
		{
#if DEBUG
			DebugDisp.IsEnabled = DebugSettings.Get("DebugTextDisplay");
#endif
			// NOP
		}

		public World GetPhysicsWorld()
		{
			return GDEntities.PhysicsWorld;
		}

		public override void Resize(int width, int height)
		{
			GameHUD.RecalculateAllElementPositions();

#if DEBUG
			var newQuality = Textures.GetPreferredQuality(Owner.GraphicsDevice);
			if (newQuality != Textures.TEXTURE_QUALITY)
			{
				Textures.ChangeQuality(Owner.Content, newQuality);
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
			GDOwner.SetLevelScreen(blueprint);
		}
	}
}
