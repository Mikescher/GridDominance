using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;
using MonoSAMFramework.Portable.DebugDisplay;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.HUD;
using System.Collections.Generic;
using System.Linq;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;

namespace MonoSAMFramework.Portable.Screens
{
	public abstract class GameScreen : Screen
	{
		public readonly GraphicsDeviceManager Graphics;
		public readonly MonoSAMGame Owner;

#if DEBUG
		protected RealtimeAPSCounter fpsCounter;
		protected RealtimeAPSCounter upsCounter;
#endif

		public ViewportAdapter Viewport;
		protected IDebugTextDisplay debugDisp;

		protected InputStateManager inputStateMan;

		protected GameHUD gameHUD;
		protected SpriteBatch mainBatch;
		protected EntityManager entities;
		protected GameBackground background;

		public GameScreen(MonoSAMGame game, GraphicsDeviceManager gdm)
		{
			Graphics = gdm;
			Owner = game;

			Initialize();
		}

		private void Initialize()
		{
			mainBatch = new SpriteBatch(Graphics.GraphicsDevice);
			Viewport = CreateViewport();

			inputStateMan = new InputStateManager(Viewport);
			gameHUD = CreateHUD();
			background = CreateBackground();

			entities = CreateEntityManager();
		}
		
		public override void Update(GameTime gameTime)
		{
#if DEBUG
			upsCounter.Update(gameTime);
#endif

			var state = inputStateMan.GetNewState();

			if (state.IsExit()) Owner.Exit();

			gameHUD.Update(gameTime, state);
			background.Update(gameTime, state);
			entities.Update(gameTime, state);

			debugDisp.Update(gameTime, state);

			OnUpdate(gameTime, state);
		}


		public override void Draw(GameTime gameTime)
		{
#if DEBUG
			fpsCounter.Update(gameTime);
#endif

			Graphics.GraphicsDevice.Clear(Color.Magenta);

			mainBatch.Begin(transformMatrix: Viewport.GetScaleMatrix());
			{
				background.Draw(mainBatch);

				entities.Draw(mainBatch);

				gameHUD.Draw(mainBatch);
			}
			mainBatch.End();

#if DEBUG
			entities.DrawOuterDebug();

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
		
		protected abstract void OnUpdate(GameTime gameTime, InputState istate);

		protected abstract EntityManager CreateEntityManager();
		protected abstract GameHUD CreateHUD();
		protected abstract GameBackground CreateBackground();
		protected abstract ViewportAdapter CreateViewport();
	}
}
