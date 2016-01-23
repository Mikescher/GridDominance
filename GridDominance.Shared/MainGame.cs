using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

namespace GridDominance.Shared
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class MainGame : Game
	{
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private ViewportAdapter vpAdapter;
		private TextureAtlas atlas;
		private Texture2D tx;

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

			graphics.PreferredBackBufferWidth = 500;
			graphics.PreferredBackBufferHeight = 500;
			Window.AllowUserResizing = true;
#else
			graphics.IsFullScreen = true;
			graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
#endif
			graphics.ApplyChanges();

			vpAdapter = new BoxingViewportAdapter(Window, graphics, 800, 500);

			graphics.ApplyChanges();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			tx = Content.Load<Texture2D>("textures/spritesheet");
			atlas = Content.Load<TextureAtlas>("textures/spritesheet-sheet");
		}
		
		protected override void UnloadContent()
		{
			// NOP
		}

		protected override void Update(GameTime gameTime)
		{
#if !__IOS__
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}
#endif
			
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.Red);

			spriteBatch.Begin(transformMatrix: vpAdapter.GetScaleMatrix());
			{
//				spriteBatch.Draw(atlas["tile_debug"].Texture, new Rectangle(0, 0, 800, 500), atlas["tile_debug"].Bounds, Color.White);
				

				for (int x = 0; x < 8; x++)
				{
					for (int y = 0; y < 5; y++)
					{
						spriteBatch.Draw(tx, new Rectangle(x*100, y*100, 100, 100), Color.White);
					}	
				}
			}
			spriteBatch.End();


			base.Draw(gameTime);
		}
	}
}

