#if DEBUG
#define DEBUG_GAMESCREEN
#define DEBUG_SHORTCUTS
#endif

using FarseerPhysics;
using GridDominance.Shared.Screens.GameScreen.Background;
using GridDominance.Shared.Screens.GameScreen.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.InputListeners;
using FarseerPhysics.Dynamics;
using GridDominance.Levelformat.Parser;
using GridDominance.Shared.Resources;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Shared.Screens.GameScreen.hud;
using Microsoft.Xna.Framework.Input;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.DebugDisplay;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.GameScreen
{
	class GameScreen : GDScreen
	{
		public const int TILE_WIDTH = 64;

		public const int GRID_WIDTH = 16;
		public const int GRID_HEIGHT = 10;

		public const int VIEW_WIDTH  = GRID_WIDTH * TILE_WIDTH; // 1024
		public const int VIEW_HEIGHT = GRID_HEIGHT * TILE_WIDTH; // 640

		//-----------------------------------------------------------------

#if DEBUG
		private RealtimeAPSCounter fpsCounter;
		private RealtimeAPSCounter upsCounter;
#endif

		public TolerantBoxingViewportAdapter Viewport;
		private IDebugTextDisplay debugDisp;

		private InputStateManager inputStateMan;
		private InputListenerManager inputs;
		private MouseListener mouseListener;
		private TouchListener touchListener;

		private GDGameHUD gameHUD;
		private SpriteBatch mainBatch;
		private GDEntityManager entities;
		
		public GameGridBackground Background;

		private Fraction fractionNeutral;
		private Fraction fractionPlayer;
		private Fraction fractionComputer1;
		private Fraction fractionComputer2;
		private Fraction fractionComputer3;

		private readonly LevelFile blueprint;

		public GameScreen(MainGame game, GraphicsDeviceManager gdm, LevelFile bp) 
			: base(game, gdm)
		{
			blueprint = bp;

			Initialize();
		}

		private void Initialize()
		{
			mainBatch = new SpriteBatch(Graphics.GraphicsDevice);
			Viewport = new TolerantBoxingViewportAdapter(Owner.Window, Graphics, VIEW_WIDTH, VIEW_HEIGHT);
			inputs = new InputListenerManager(Viewport);
			inputStateMan = new InputStateManager(Viewport);
			Background = new GameGridBackground(Graphics.GraphicsDevice, Viewport);
			entities = new GDEntityManager(this);
			gameHUD = new GDGameHUD(this);

			ConvertUnits.SetDisplayUnitToSimUnitRatio(GDSettings.PHYSICS_CONVERSION_FACTOR);

			mouseListener = inputs.AddListener(new MouseListenerSettings());
			touchListener = inputs.AddListener(new TouchListenerSettings());


#if DEBUG_GAMESCREEN
			fpsCounter = new RealtimeAPSCounter();
			upsCounter = new RealtimeAPSCounter();

			debugDisp = new DebugTextDisplay(Graphics.GraphicsDevice, Textures.DebugFont);
			{
				debugDisp.AddLine(() => $"FPS = {fpsCounter.AverageAPS:0000.0} (current = {fpsCounter.CurrentAPS:0000.0} | delta = {fpsCounter.AverageDelta*1000:000.00} | min = {fpsCounter.MinimumAPS:0000.0} | total = {fpsCounter.TotalActions:000000})");
				debugDisp.AddLine(() => $"UPS = {upsCounter.AverageAPS:0000.0} (current = {upsCounter.CurrentAPS:0000.0} | delta = {upsCounter.AverageDelta*1000:000.00} | min = {upsCounter.MinimumAPS:0000.0} | total = {upsCounter.TotalActions:000000})");
				debugDisp.AddLine(() => $"Quality = {Textures.TEXTURE_QUALITY} | Texture.Scale={1f/Textures.DEFAULT_TEXTURE_SCALE.X:#.00} | Pixel.Scale={Textures.GetDeviceTextureScaling(Owner.GraphicsDevice):#.00} | Viewport=[{Owner.GraphicsDevice.Viewport.Width}|{Owner.GraphicsDevice.Viewport.Height}]");
				debugDisp.AddLine(() => $"Entities = {entities.Count()}");
				debugDisp.AddLine(() => $"Particles = {Background.Particles.Count()}");
				debugDisp.AddLine(() => $"Pointer = ({inputStateMan.GetCurrentState().PointerPosition.X:000.0}|{inputStateMan.GetCurrentState().PointerPosition.Y:000.0})");
			}

			mouseListener.MouseDown += (o, a) => debugDisp.AddDecayLine($"Mouse::OnDown({a.Position.X:0000}|{a.Position.Y:0000})", 0.75f, 0.5f, 0.25f);
			mouseListener.MouseUp += (o, a) => debugDisp.AddDecayLine($"Mouse::OnUp({a.Position.X:0000}|{a.Position.Y:0000})", 0.75f, 0.5f, 0.25f);
			touchListener.TouchStarted += (o, a) => debugDisp.AddDecayLine($"TouchPad::OnDown({a.Location.Position.X:0000}|{a.Location.Position.Y:0000})", 0.75f, 0.5f, 0.25f);
			touchListener.TouchEnded += (o, a) => debugDisp.AddDecayLine($"TouchPad::OnUp({a.Location.Position.X:0000}|{a.Location.Position.Y:0000})", 0.75f, 0.5f, 0.25f);
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

		public override void Update(GameTime gameTime)
		{
#if DEBUG
			upsCounter.Update(gameTime);
#endif

			var state = inputStateMan.GetNewState();
			
			if (state.IsExit()) Owner.Exit();
			
			inputs.Update(gameTime);

			gameHUD.Update(gameTime, state);

			Background.Update(gameTime, state);

			entities.Update(gameTime, state);

			debugDisp.Update(gameTime, state);

#if DEBUG_SHORTCUTS
			UpdateDebugShortcuts(state);
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

		public override void Draw(GameTime gameTime)
		{
#if DEBUG
			fpsCounter.Update(gameTime);
#endif
			
			Graphics.GraphicsDevice.Clear(Color.Magenta);
			
			mainBatch.Begin(transformMatrix: Viewport.GetScaleMatrix());
			{
				Background.Draw(mainBatch);
			
				entities.Draw(mainBatch);

				gameHUD.Draw(mainBatch);
			}
			mainBatch.End();
			
#if DEBUG
			entities.DrawRest();

			debugDisp.Draw(gameTime);
#endif
		}

		public void PushNotification(string text)
		{
			debugDisp.AddDecayLine(text);
		}

		public void PushErrorNotification(string text)
		{
			debugDisp.AddErrorDecayLine(text);
		}

		public IEnumerable<T> GetEntities<T>()
		{
			return entities.Enumerate().OfType<T>();
		}

		public World GetPhysicsWorld()
		{
			return entities.PhysicsWorld;
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
