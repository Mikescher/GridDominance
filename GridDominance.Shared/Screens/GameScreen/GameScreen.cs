using System;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.TextureAtlases;

namespace GridDominance.Shared.Screens.GameScreen
{
    class GameScreen : GDScreen
    {
        private ViewportAdapter vpAdapter;
        private SpriteBatch mainBatch;

        public GameScreen(MainGame game, GraphicsDeviceManager gdm) : base(game, gdm)
        {
            Initialize();
        }

        private void Initialize()
        {
            mainBatch = new SpriteBatch(graphics.GraphicsDevice);
            vpAdapter = new BoxingViewportAdapter(owner.Window, graphics, 800, 500);

            graphics.ApplyChanges();
        }

        public override void Update(GameTime gameTime)
        {
#if !__IOS__
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                owner.Exit();
            }
#endif
        }

        public override void Draw(GameTime gameTime)
        {
            mainBatch.Begin(transformMatrix: vpAdapter.GetScaleMatrix());
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        mainBatch.Draw(Textures.TexDebugTile, new Rectangle(x * 100, y * 100, 100, 100), Color.White);
                    }
                }
            }
            mainBatch.End();
        }
    }
}
