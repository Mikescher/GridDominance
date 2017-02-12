using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;
using System;
using System.Collections.Concurrent;

namespace MonoSAMFramework.Portable
{
	public abstract class MonoSAMGame : Game, ILifetimeObject
	{
		private ScreenManager screens;

		protected readonly GraphicsDeviceManager Graphics;
		private ConcurrentQueue<Action> dispatchQueue = new ConcurrentQueue<Action>();

		public static ulong GameCycleCounter { get; private set; }
		public static SAMTime CurrentTime { get; private set; }

		public static MonoSAMGame CurrentInst { get; private set; }

		public bool Alive { get; private set; } = true;

		public readonly IOperatingSystemBridge Bridge;

		protected MonoSAMGame(IOperatingSystemBridge bridge)
		{
			try
			{
				FileHelper.RegisterSystemSecificHandler(bridge.FileHelper);

				Bridge = bridge;
				CurrentInst = this;

				CurrentTime = new SAMTime();

				Graphics = new GraphicsDeviceManager(this);
				Content.RootDirectory = "Content";
			}
			catch (Exception e)
			{
				SAMLog.FatalError("Game::Constructor", e);
			}
		}

		protected override void Initialize()
		{
			try
			{
				base.Initialize();

				OnInitialize();

				screens = new ScreenManager(this);

				OnAfterInitialize();
			}
			catch (Exception e)
			{
				SAMLog.FatalError("Game::Initialize", e);
			}
		}

		protected override void OnExiting(object sender, EventArgs args)
		{
			base.OnExiting(sender, args);

			Alive = false;
		}

		protected void SetCurrentScreen(GameScreen gdGameScreen)
		{
			screens.CurrentScreen = gdGameScreen;
		}

		protected override void Update(GameTime gameTime)
		{
			try
			{
				var time = new SAMTime(gameTime);

				screens.Update(time);

				Action dispatchAction;
				while (dispatchQueue.TryDequeue(out dispatchAction)) dispatchAction();

				base.Update(gameTime);
			}
			catch (Exception e)
			{
				SAMLog.FatalError("Game::Update", e);
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			try
			{
				var time = new SAMTime(gameTime);

				GameCycleCounter++;
				CurrentTime = time;

				screens.Draw(time);

				base.Draw(gameTime);
			}
			catch (Exception e)
			{
				SAMLog.FatalError("Game::Draw", e);
			}
		}

		public void DispatchBeginInvoke(Action a)
		{
			dispatchQueue.Enqueue(a);
		}

		protected abstract void OnAfterInitialize();
		protected abstract void OnInitialize();
	}
}
