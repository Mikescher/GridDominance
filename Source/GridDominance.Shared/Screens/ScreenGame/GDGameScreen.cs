using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using GridDominance.Shared.Screens.ScreenGame.Background;
using GridDominance.Shared.Screens.ScreenGame.Entities;
using GridDominance.Shared.Screens.ScreenGame.hud;
using GridDominance.Levelformat.Parser;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.InputListeners;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.HUD;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.Entities.Particles;
using MonoSAMFramework.Portable.Screens.Entities.Particles.GPUParticles;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;
using GridDominance.Shared.Resources;

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

		public GDGridBackground GDBackground => (GDGridBackground) Background;
		public GDEntityManager GDEntities => (GDEntityManager) Entities;
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

		private readonly LevelFile blueprint;
		private readonly FractionDifficulty difficulty;
		public readonly string LevelName = "NAL"; //TODO Better smth guid in levelfile (somehow force unique, by compiler ??)

		public bool HasFinished = false;

		public GDGameScreen(MainGame game, GraphicsDeviceManager gdm, LevelFile bp, FractionDifficulty diff) : base(game, gdm)
		{
			blueprint = bp;
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
			DebugSettings.AddTrigger("SetQuality_1", this, Keys.D1, KeyboardModifiers.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.FD));
			DebugSettings.AddTrigger("SetQuality_2", this, Keys.D2, KeyboardModifiers.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.BD));
			DebugSettings.AddTrigger("SetQuality_3", this, Keys.D3, KeyboardModifiers.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.LD));
			DebugSettings.AddTrigger("SetQuality_4", this, Keys.D4, KeyboardModifiers.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.MD));
			DebugSettings.AddTrigger("SetQuality_5", this, Keys.D5, KeyboardModifiers.Control, x => Textures.ChangeQuality(Game.Content, TextureQuality.HD));

			DebugSettings.AddSwitch("PhysicsDebugView", this, Keys.F1, KeyboardModifiers.None, false);
			DebugSettings.AddSwitch("DebugTextDisplay", this, Keys.F2, KeyboardModifiers.None, true);
			DebugSettings.AddSwitch("DebugBackground", this, Keys.F3, KeyboardModifiers.None, false);
			DebugSettings.AddSwitch("DebugHUDBorders", this, Keys.F4, KeyboardModifiers.None, false);
			DebugSettings.AddSwitch("DebugCannonView", this, Keys.F5, KeyboardModifiers.None, true);
			DebugSettings.AddSwitch("ShowMatrixTextInfos", this, Keys.F6, KeyboardModifiers.None, false);
			DebugSettings.AddSwitch("ShowDebugMiniMap", this, Keys.F7, KeyboardModifiers.None, false);
			DebugSettings.AddSwitch("DebugEntityBoundaries", this, Keys.F8, KeyboardModifiers.None, false);

			DebugSettings.AddPush("ShowDebugShortcuts", this, Keys.Tab, KeyboardModifiers.None);

			DebugSettings.AddPush("AssimilateCannon", this, Keys.A, KeyboardModifiers.None);
			DebugSettings.AddPush("AbandonCannon", this, Keys.S, KeyboardModifiers.None);
#endif

#if DEBUG
			DebugDisp = new DebugTextDisplay(Graphics.GraphicsDevice, Textures.DebugFont);
			{
				DebugDisp.AddLine(() => $"FPS = {FPSCounter.AverageAPS:0000.0} (current = {FPSCounter.CurrentAPS:0000.0} | delta = {FPSCounter.AverageDelta * 1000:000.00} | min = {FPSCounter.MinimumAPS:0000.0} | total = {FPSCounter.TotalActions:000000})");
				DebugDisp.AddLine(() => $"UPS = {UPSCounter.AverageAPS:0000.0} (current = {UPSCounter.CurrentAPS:0000.0} | delta = {UPSCounter.AverageDelta * 1000:000.00} | min = {UPSCounter.MinimumAPS:0000.0} | total = {UPSCounter.TotalActions:000000})");
				DebugDisp.AddLine(() => $"Quality = {Textures.TEXTURE_QUALITY} | Texture.Scale={1f / Textures.DEFAULT_TEXTURE_SCALE.X:#.00} | Pixel.Scale={Textures.GetDeviceTextureScaling(Game.GraphicsDevice):#.00} | Viewport=[{Game.GraphicsDevice.Viewport.Width}|{Game.GraphicsDevice.Viewport.Height}]");
				DebugDisp.AddLine(() => $"Entities = {Entities.Count(),3} | Particles = {Entities.Enumerate().OfType<IParticleOwner>().Sum(p => p.ParticleCount),3} (Visible: {Entities.Enumerate().Where(p => p.IsInViewport).OfType<IParticleOwner>().Sum(p => p.ParticleCount),3})"); DebugDisp.AddLine(() => $"HUD.Size=(T:{GameHUD.Top}|L:{GameHUD.Left}|R:{GameHUD.Right}|B:{GameHUD.Bottom}) | HUD.AllElements={GameHUD.DeepCount()} | HUD.RootElements={GameHUD.FlatCount()} | Background.Particles={GDBackground.Particles.Count,3}");
				DebugDisp.AddLine(() => $"Pointer = ({InputStateMan.GetCurrentState().PointerPosition.X:000.0}|{InputStateMan.GetCurrentState().PointerPosition.Y:000.0})");
				DebugDisp.AddLine(() => $"OGL Sprites = {LastReleaseRenderSpriteCount:0000} (+ {LastDebugRenderSpriteCount:0000}); OGL Text = {LastReleaseRenderTextCount:0000} (+ {LastDebugRenderTextCount:0000})");

				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"GraphicsDevice.Viewport=[{Game.GraphicsDevice.Viewport.Width}|{Game.GraphicsDevice.Viewport.Height}]");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.VirtualGuaranteedSize={VAdapter.VirtualGuaranteedSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.RealGuaranteedSize={VAdapter.RealGuaranteedSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.VirtualTotalSize={VAdapter.VirtualTotalSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.RealTotalSize={VAdapter.RealTotalSize}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.VirtualOffset={VAdapter.VirtualGuaranteedBoundingsOffset}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.RealOffset={VAdapter.RealGuaranteedBoundingsOffset}");
				DebugDisp.AddLine("ShowMatrixTextInfos", () => $"Adapter.Scale={VAdapter.Scale}");

				DebugDisp.AddLine("ShowDebugShortcuts", DebugSettings.GetSummary);
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
		protected override GameBackground CreateBackground() => new GDGridBackground(this);
		protected override SAMViewportAdapter CreateViewport() => new TolerantBoxingViewportAdapter(Game.Window, Graphics, GDConstants.VIEW_WIDTH, GDConstants.VIEW_HEIGHT);
		protected override DebugMinimap CreateDebugMinimap() => new GDScreenDebugMinimap(this);
		protected override FRectangle CreateMapFullBounds() => new FRectangle(0, 0, GDConstants.VIEW_WIDTH, GDConstants.VIEW_HEIGHT);

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
				if (GDOwner.Profile.GetLevelData(LevelName).HasCompleted(difficulty))
				{
					GDGameHUD.ShowScorePanel(GDOwner.Profile, null, true);
				}
				else
				{
					GDOwner.Profile.GetLevelData(LevelName).SetCompleted(difficulty);
					GDOwner.SaveProfile();
					GDGameHUD.ShowScorePanel(GDOwner.Profile, difficulty, true);
				}

			}
			else
			{
				GDGameHUD.ShowScorePanel(GDOwner.Profile, null, false);
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
			GDOwner.SetLevelScreen(blueprint);
		}
	}
}
