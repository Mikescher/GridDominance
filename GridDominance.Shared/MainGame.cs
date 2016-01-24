using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.GameScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

namespace GridDominance.Shared
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class MainGame : Game
	{
	    private ScreenManager screens;

		private readonly GraphicsDeviceManager graphics;

		public MainGame()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			base.Initialize();

#if __DESKTOP__
			IsMouseVisible = true;
			graphics.IsFullScreen = false;

			graphics.PreferredBackBufferWidth = 800;
			graphics.PreferredBackBufferHeight = 600;
			Window.AllowUserResizing = true;
#else
			graphics.IsFullScreen = true;
			graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
#endif
			graphics.ApplyChanges();

		    screens = new ScreenManager(this)
		    {
		        CurrentScreen = new GameScreen(this, graphics)
		    };
		}

		protected override void LoadContent()
		{
            Textures.LoadContent(Content);
		}
		
		protected override void UnloadContent()
		{
			// NOP
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
	}
}

