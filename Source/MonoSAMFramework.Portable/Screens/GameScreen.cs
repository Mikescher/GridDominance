using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.MathHelper;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.Screens
{
	public abstract class GameScreen : Screen
	{
		public readonly GraphicsDeviceManager Graphics;
		public readonly MonoSAMGame Game;

#if DEBUG
		protected RealtimeAPSCounter FPSCounter;
		protected RealtimeAPSCounter UPSCounter;
#endif

		public SAMViewportAdapter VAdapter;
		protected IDebugTextDisplay DebugDisp;

		protected InputStateManager InputStateMan;

		protected GameHUD GameHUD;
		protected IBatchRenderer MainBatch;
		protected EntityManager Entities;
		protected GameBackground Background;

		public float GameSpeed = 1f;

		protected GameScreen(MonoSAMGame game, GraphicsDeviceManager gdm)
		{
			Graphics = gdm;
			Game = game;
			
			Initialize();
		}

		private void Initialize()
		{
			MainBatch = new StandardSpriteBatchWrapper(Graphics.GraphicsDevice);
			VAdapter = CreateViewport();

			InputStateMan = new InputStateManager(VAdapter);
			GameHUD = CreateHUD();
			Background = CreateBackground();

			Entities = CreateEntityManager();

			DebugDisp = new DummyDebugTextDisplay();

#if DEBUG
			FPSCounter = new RealtimeAPSCounter();
			UPSCounter = new RealtimeAPSCounter();
#endif
		}

		protected override void OnRemove()
		{
			base.OnRemove();

			MainBatch.Dispose();
		}

		public override void Update(GameTime gameTime)
		{
			var state = InputStateMan.GetNewState();

			if (state.IsExit()) Game.Exit();

#if DEBUG
			DebugSettings.Update(state);
#endif

			GameHUD.Update(gameTime, state);
			DebugDisp.Update(gameTime, state);

			if (FloatMath.IsZero(GameSpeed))
			{
				return;
			}
			else if (FloatMath.IsOne(GameSpeed))
			{
				InternalUpdate(gameTime, state);
			}
			else if (GameSpeed < 1f)
			{
				var internalTime = new GameTime(gameTime.TotalGameTime, new TimeSpan((long) (gameTime.ElapsedGameTime.Ticks * GameSpeed)));

				InternalUpdate(internalTime, state);
			}
			else if (GameSpeed > 1f)
			{
				var totalTicks = gameTime.ElapsedGameTime.Ticks * GameSpeed;

				int runCount = (int) GameSpeed;

				long ticksPerRun = (long) (totalTicks / runCount);
				ticksPerRun += (long)((totalTicks - ticksPerRun * runCount) / runCount);

				var time = gameTime.TotalGameTime;

				for (int i = 0; i < runCount; i++)
				{
					var span = new TimeSpan(ticksPerRun);

					InternalUpdate(new GameTime(time, span), state);

					time += span;
				}
			}
			else
			{
				throw new ArgumentException("GameSpeed");
			}
		}

		private void InternalUpdate(GameTime gameTime, InputState state)
		{
#if DEBUG
			UPSCounter.Update(gameTime);
#endif

			InputStateMan.TriggerListener();


			Background.Update(gameTime, state);
			Entities.Update(gameTime, state);

			OnUpdate(gameTime, state);
		}

		public override void Draw(GameTime gameTime)
		{
#if DEBUG
			FPSCounter.Update(gameTime);
#endif

			Graphics.GraphicsDevice.Clear(Color.Magenta);

			MainBatch.Begin(transformMatrix: VAdapter.GetScaleMatrix());
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

		public override void Resize(int width, int height)
		{
			base.Resize(width, height);

			GameHUD.RecalculateAllElementPositions();
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
		protected abstract SAMViewportAdapter CreateViewport();
	}
}
