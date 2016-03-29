#if DEBUG
#define DEBUG_GAMESCREEN
#define DEBUG_SHORTCUTS
#endif

using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame.Background;
using GridDominance.Shared.Screens.ScreenGame.Entities;
using GridDominance.Shared.Screens.ScreenGame.hud;
using GridDominance.Levelformat.Parser;
using GridDominance.Shared.Screens.GameScreen;
using GridDominance.Shared.Screens.GameScreen.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.ViewportAdapters;
using MonoSAMFramework.Portable.DebugDisplay;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.ScreenGame
{
	class GDGameScreen : MonoSAMFramework.Portable.Screens.GameScreen
	{
		public const int TILE_WIDTH = 64;

		public const int GRID_WIDTH = 16;
		public const int GRID_HEIGHT = 10;

		public const int VIEW_WIDTH  = GRID_WIDTH * TILE_WIDTH; // 1024
		public const int VIEW_HEIGHT = GRID_HEIGHT * TILE_WIDTH; // 640

		//-----------------------------------------------------------------

		public GDGridBackground GDBackground => (GDGridBackground) background;
		public GDEntityManager GDEntities => (GDEntityManager) entities;
		public TolerantBoxingViewportAdapter GDViewport => (TolerantBoxingViewportAdapter) Viewport;

		//-----------------------------------------------------------------

		private Fraction fractionNeutral;
		private Fraction fractionPlayer;
		private Fraction fractionComputer1;
		private Fraction fractionComputer2;
		private Fraction fractionComputer3;

		private readonly LevelFile blueprint;

		public GDGameScreen(MainGame game, GraphicsDeviceManager gdm, LevelFile bp) 
			: base(game, gdm)
		{
			blueprint = bp;

			Initialize();
		}

		private void Initialize()
		{
			ConvertUnits.SetDisplayUnitToSimUnitRatio(GDSettings.PHYSICS_CONVERSION_FACTOR);
			
#if DEBUG_GAMESCREEN
			fpsCounter = new RealtimeAPSCounter();
			upsCounter = new RealtimeAPSCounter();

			debugDisp = new DebugTextDisplay(Graphics.GraphicsDevice, Textures.DebugFont);
			{
				debugDisp.AddLine(() => $"FPS = {fpsCounter.AverageAPS:0000.0} (current = {fpsCounter.CurrentAPS:0000.0} | delta = {fpsCounter.AverageDelta*1000:000.00} | min = {fpsCounter.MinimumAPS:0000.0} | total = {fpsCounter.TotalActions:000000})");
				debugDisp.AddLine(() => $"UPS = {upsCounter.AverageAPS:0000.0} (current = {upsCounter.CurrentAPS:0000.0} | delta = {upsCounter.AverageDelta*1000:000.00} | min = {upsCounter.MinimumAPS:0000.0} | total = {upsCounter.TotalActions:000000})");
				debugDisp.AddLine(() => $"Quality = {Textures.TEXTURE_QUALITY} | Texture.Scale={1f/Textures.DEFAULT_TEXTURE_SCALE.X:#.00} | Pixel.Scale={Textures.GetDeviceTextureScaling(Owner.GraphicsDevice):#.00} | Viewport=[{Owner.GraphicsDevice.Viewport.Width}|{Owner.GraphicsDevice.Viewport.Height}]");
				debugDisp.AddLine(() => $"Entities = {entities.Count()}");
				debugDisp.AddLine(() => $"Particles = {GDBackground.Particles.Count()}");
				debugDisp.AddLine(() => $"Pointer = ({inputStateMan.GetCurrentState().PointerPosition.X:000.0}|{inputStateMan.GetCurrentState().PointerPosition.Y:000.0})");
			}

			// TODO move to inputman
			//mouseListener.MouseDown += (o, a) => debugDisp.AddDecayLine($"Mouse::OnDown({a.Position.X:0000}|{a.Position.Y:0000})", 0.75f, 0.5f, 0.25f);
			//mouseListener.MouseUp += (o, a) => debugDisp.AddDecayLine($"Mouse::OnUp({a.Position.X:0000}|{a.Position.Y:0000})", 0.75f, 0.5f, 0.25f);
			//touchListener.TouchStarted += (o, a) => debugDisp.AddDecayLine($"TouchPad::OnDown({a.Location.Position.X:0000}|{a.Location.Position.Y:0000})", 0.75f, 0.5f, 0.25f);
			//touchListener.TouchEnded += (o, a) => debugDisp.AddDecayLine($"TouchPad::OnUp({a.Location.Position.X:0000}|{a.Location.Position.Y:0000})", 0.75f, 0.5f, 0.25f);
#else
			debugDisp = new DummyDebugTextDisplay();
#endif


			//--------------------

			fractionNeutral   = Fraction.CreateNeutralFraction();
			fractionPlayer    = Fraction.CreatePlayerFraction(fractionNeutral);
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
				fractionNeutral,
				fractionPlayer,
				fractionComputer1,
				fractionComputer2,
				fractionComputer3,
			};

			foreach (var bPrint in blueprint.BlueprintCannons)
			{
				entities.AddEntity(new Cannon(this, bPrint, fracList));
			}
		}

		protected override void OnUpdate(GameTime gameTime, InputState istate)
		{
#if DEBUG_SHORTCUTS
			UpdateDebugShortcuts(istate);
#endif
		}

#if DEBUG
		private void UpdateDebugShortcuts(InputState state)
		{
			if (state.IsShortcutJustPressed(KeyboardModifiers.Control, Keys.D1))
				Textures.ChangeQuality(Owner.Content, TextureQuality.FD);

			if (state.IsShortcutJustPressed(KeyboardModifiers.Control, Keys.D2))
				Textures.ChangeQuality(Owner.Content, TextureQuality.BD);

			if (state.IsShortcutJustPressed(KeyboardModifiers.Control, Keys.D3))
				Textures.ChangeQuality(Owner.Content, TextureQuality.LD);

			if (state.IsShortcutJustPressed(KeyboardModifiers.Control, Keys.D4))
				Textures.ChangeQuality(Owner.Content, TextureQuality.MD);

			if (state.IsShortcutJustPressed(KeyboardModifiers.Control, Keys.D5))
				Textures.ChangeQuality(Owner.Content, TextureQuality.HD);
		}
#endif

		public World GetPhysicsWorld()
		{
			return GDEntities.PhysicsWorld;
		}

#if DEBUG
		public override void Resize(int width, int height)
		{
			var newQuality = Textures.GetPreferredQuality(Owner.GraphicsDevice);
			if (newQuality != Textures.TEXTURE_QUALITY)
			{
				Textures.ChangeQuality(Owner.Content, newQuality);
			}
		}
#endif

	}
}
