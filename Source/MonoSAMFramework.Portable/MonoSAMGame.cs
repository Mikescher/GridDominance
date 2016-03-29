using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable
{
	public abstract class MonoSAMGame : Game
	{
		private ScreenManager screens;

		protected readonly GraphicsDeviceManager Graphics;

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
			screens.Draw(gameTime);

			base.Draw(gameTime);
		}

		protected abstract void OnAfterInitialize();
		protected abstract void OnInitialize();
	}
}
