﻿using GridDominance.Shared.Framework;
using GridDominance.Shared.Framework.DebugDisplay;
using GridDominance.Shared.Screens.GameScreen.Background;
using GridDominance.Shared.Screens.GameScreen.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.InputListeners;
using System.Threading;

namespace GridDominance.Shared.Screens.GameScreen
{
    class GameScreen : GDScreen
    {
        public const int TILE_WIDTH = 128;
        
        public const int VIEW_WIDTH  = 8 * TILE_WIDTH; // 1024
        public const int VIEW_HEIGHT = 5 * TILE_WIDTH; // 640

		//-----------------------------------------------------------------

		private RealtimeAPSCounter fpsCounter;
		private RealtimeAPSCounter upsCounter;

		private TolerantBoxingViewportAdapter viewport;
	    private GDEntityManager entities;
	    private IDebugTextDisplay debugDisp;

		private InputStateManager inputStateMan;
		private InputListenerManager inputs;
	    private MouseListener mouseListener;
	    private TouchListener touchListener;

		private SpriteBatch mainBatch;

		private GameGridBackground background;

        public GameScreen(MainGame game, GraphicsDeviceManager gdm) 
			: base(game, gdm)
        {
            Initialize();
        }

        private void Initialize()
        {
			mainBatch = new SpriteBatch(Graphics.GraphicsDevice);
            viewport = new TolerantBoxingViewportAdapter(Owner.Window, Graphics, VIEW_WIDTH, VIEW_HEIGHT);
			inputs = new InputListenerManager(viewport);
			inputStateMan = new InputStateManager(viewport);
			background = new GameGridBackground(Graphics.GraphicsDevice, viewport);
			entities = new GDEntityManager();
			fpsCounter = new RealtimeAPSCounter();
			upsCounter = new RealtimeAPSCounter();

			mouseListener = inputs.AddListener(new MouseListenerSettings());
			touchListener = inputs.AddListener(new TouchListenerSettings());

#if DEBUG
			debugDisp = new DebugTextDisplay(Graphics.GraphicsDevice);
	        {
				debugDisp.AddLine(() => $"FPS = {fpsCounter.AverageAPS:0000.0} (current = {fpsCounter.CurrentAPS:0000.0} | delta = {fpsCounter.AverageDelta*1000:000.00} | min = {fpsCounter.MinimumAPS:0000.0} | total = {fpsCounter.TotalActions:000000})");
				debugDisp.AddLine(() => $"UPS = {upsCounter.AverageAPS:0000.0} (current = {upsCounter.CurrentAPS:0000.0} | delta = {upsCounter.AverageDelta*1000:000.00} | min = {upsCounter.MinimumAPS:0000.0} | total = {upsCounter.TotalActions:000000})");
				debugDisp.AddLine(() => $"Entities = {entities.Count()}");
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

			entities.AddEntity(new Cannon(this, 2 * 128 + 64, 3 * 128 + 64));
            entities.AddEntity(new Cannon(this, 2 * 128 + 64, 0 * 128 + 64));
            entities.AddEntity(new Cannon(this, 5 * 128 + 64, 1 * 128 + 64));
            entities.AddEntity(new Cannon(this, 6 * 128 + 64, 2 * 128 + 64));
        }

        public override void Update(GameTime gameTime)
		{
			upsCounter.Update(gameTime);

			var state = inputStateMan.GetNewState();
			
			if (state.IsExit()) Owner.Exit();
			
			inputs.Update(gameTime);

			entities.Update(gameTime, state);

			debugDisp.Update(gameTime, state);
		}

        public override void Draw(GameTime gameTime)
		{
			fpsCounter.Update(gameTime);

			Graphics.GraphicsDevice.Clear(Color.OrangeRed);

            mainBatch.Begin(transformMatrix: viewport.GetScaleMatrix());
            {
                background.Draw(mainBatch);

                entities.Draw(mainBatch);
			}
            mainBatch.End();
			
			debugDisp.Draw(gameTime);
		}

	    public void PushNotification(string text)
	    {
		    debugDisp.AddDecayLine(text);
	    }
    }
}
