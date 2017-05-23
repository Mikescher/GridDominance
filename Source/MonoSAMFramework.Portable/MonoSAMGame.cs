using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.Language;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Sound;
using System;
using System.Threading.Tasks;

namespace MonoSAMFramework.Portable
{
	public abstract class MonoSAMGame : Game, ILifetimeObject
	{
		private ScreenManager screens;
		private readonly CustomDispatcher gameDispatcher = new CustomDispatcher();

		protected readonly GraphicsDeviceManager Graphics;

		public static ulong GameCycleCounter { get; private set; }
		public static SAMTime CurrentTime { get; private set; }

		public static MonoSAMGame CurrentInst { get; private set; }

		public bool Alive { get; private set; } = true;

		public readonly IOperatingSystemBridge Bridge;

		public abstract SAMSoundPlayer Sound { get; }

		public IDebugTextDisplay DebugDisplay => (screens?.CurrentScreen as GameScreen)?.DebugDisp;

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

		public Screen GetCurrentScreen()
		{
			return screens.CurrentScreen;
		}

		protected override void Update(GameTime gameTime)
		{
			try
			{
				var time = new SAMTime(gameTime);

				OnUpdate(time);

				screens.Update(time);

				Sound.Update(time);

				gameDispatcher.Work();

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

				GC.Collect(0); // small collect after every tick
			}
			catch (Exception e)
			{
				SAMLog.FatalError("Game::Draw", e);
			}
		}

		public void DispatchBeginInvoke(Action a)
		{
			gameDispatcher.BeginInvoke(a);
		}

		public async Task DispatchInvoke(Action a)
		{
			await gameDispatcher.Invoke(a);
		}

		protected abstract void OnAfterInitialize();
		protected abstract void OnInitialize();
		protected virtual void OnUpdate(SAMTime gameTime) { /* override */}
	}
}
