using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.External;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable
{
	public abstract class MonoSAMGame : Game
	{
		private ScreenManager screens;

		protected readonly GraphicsDeviceManager Graphics;

		public static ulong GameCycleCounter { get; private set; }
		public static GameTime CurrentTime { get; private set; }

		protected MonoSAMGame()
		{
			Graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			base.Initialize();

			OnInitialize();

			screens = new ScreenManager(this);

			OnAfterInitialize();
		}

		protected void SetCurrentScreen(GameScreen gdGameScreen)
		{
			screens.CurrentScreen = gdGameScreen;
		}

		protected override void Update(GameTime gameTime)
		{
			screens.Update(gameTime);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GameCycleCounter++;
			CurrentTime = gameTime;

			screens.Draw(gameTime);

			base.Draw(gameTime);
		}

		protected abstract void OnAfterInitialize();
		protected abstract void OnInitialize();
	}
}
