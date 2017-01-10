using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;
using System;

namespace MonoSAMFramework.Portable
{
	public abstract class MonoSAMGame : Game, ILifetimeObject
	{
		private ScreenManager screens;

		protected readonly GraphicsDeviceManager Graphics;

		public static ulong GameCycleCounter { get; private set; }
		public static GameTime CurrentTime { get; private set; }

		public static MonoSAMGame CurrentInst { get; private set; }

		public bool Alive { get; private set; } = true;

		protected MonoSAMGame()
		{
			try
			{
				CurrentInst = this;

				CurrentTime = new GameTime();

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
				screens.Update(gameTime);

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
				GameCycleCounter++;
				CurrentTime = gameTime;

				screens.Draw(gameTime);

				base.Draw(gameTime);
			}
			catch (Exception e)
			{
				SAMLog.FatalError("Game::Draw", e);
			}
		}

		protected abstract void OnAfterInitialize();
		protected abstract void OnInitialize();
	}
}
