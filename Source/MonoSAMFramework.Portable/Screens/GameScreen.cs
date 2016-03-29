using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;
using MonoSAMFramework.Portable.DebugDisplay;
using MonoSAMFramework.Portable.External;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.HUD;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.Screens
{
	public abstract class GameScreen : Screen
	{
		public readonly GraphicsDeviceManager Graphics;
		public readonly MonoSAMGame Owner;

#if DEBUG
		protected RealtimeAPSCounter FPSCounter;
		protected RealtimeAPSCounter UPSCounter;
#endif

		public ViewportAdapter Viewport;
		protected IDebugTextDisplay DebugDisp;

		protected InputStateManager InputStateMan;

		protected GameHUD GameHUD;
		protected SpriteBatch MainBatch;
		protected EntityManager Entities;
		protected GameBackground Background;

		protected GameScreen(MonoSAMGame game, GraphicsDeviceManager gdm)
		{
			Graphics = gdm;
			Owner = game;

			Initialize();
		}

		private void Initialize()
		{
			MainBatch = new SpriteBatch(Graphics.GraphicsDevice);
			Viewport = CreateViewport();

			InputStateMan = new InputStateManager(Viewport);
			GameHUD = CreateHUD();
			Background = CreateBackground();

			Entities = CreateEntityManager();
		}
		
		public override void Update(GameTime gameTime)
		{
#if DEBUG
			UPSCounter.Update(gameTime);
#endif

			var state = InputStateMan.GetNewState();
			InputStateMan.TriggerListener();

			if (state.IsExit()) Owner.Exit();

			GameHUD.Update(gameTime, state);
			Background.Update(gameTime, state);
			Entities.Update(gameTime, state);

			DebugDisp.Update(gameTime, state);

			OnUpdate(gameTime, state);
		}


		public override void Draw(GameTime gameTime)
		{
#if DEBUG
			FPSCounter.Update(gameTime);
#endif

			Graphics.GraphicsDevice.Clear(Color.Magenta);

			MainBatch.Begin(transformMatrix: Viewport.GetScaleMatrix());
			{
				Background.Draw(MainBatch);

				Entities.Draw(MainBatch);

				GameHUD.Draw(MainBatch);
			}
			MainBatch.End();

#if DEBUG
			Entities.DrawOuterDebug();

			DebugDisp.Draw();
#endif
		}

		public void PushNotification(string text)
		{
			DebugDisp.AddDecayLine(text);
		}

		public void PushErrorNotification(string text)
		{
			DebugDisp.AddErrorDecayLine(text);
		}

		public IEnumerable<T> GetEntities<T>()
		{
			return Entities.Enumerate().OfType<T>();
		}
		
		protected abstract void OnUpdate(GameTime gameTime, InputState istate);

		protected abstract EntityManager CreateEntityManager();
		protected abstract GameHUD CreateHUD();
		protected abstract GameBackground CreateBackground();
		protected abstract ViewportAdapter CreateViewport();
	}
}
