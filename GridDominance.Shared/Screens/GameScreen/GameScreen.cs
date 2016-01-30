using System;
using System.Collections.Generic;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.GameScreen.Entities;
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
        public const int TILE_WIDTH = 128;

        public const int VIEW_WIDTH  = 8 * TILE_WIDTH; // 1024
        public const int VIEW_HEIGHT = 5 * TILE_WIDTH; // 640

        //-----------------------------------------------------------------

        private ViewportAdapter vpAdapter;

        public SpriteBatch EntityBatch;

        private List<GDEntity> entities = new List<GDEntity>();

        public GameScreen(MainGame game, GraphicsDeviceManager gdm) : base(game, gdm)
        {
            Initialize();
        }

        private void Initialize()
        {
            EntityBatch = new SpriteBatch(graphics.GraphicsDevice);
            vpAdapter = new BoxingViewportAdapter(owner.Window, graphics, VIEW_WIDTH, VIEW_HEIGHT);
            graphics.ApplyChanges();

            //--------------------

            entities.Add(new Cannon(this, 2, 3));
            entities.Add(new Cannon(this, 2, 0));
            entities.Add(new Cannon(this, 5, 1));
            entities.Add(new Cannon(this, 6, 2));
        }

        public override void Update(GameTime gameTime)
        {
#if !__IOS__
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                owner.Exit();
            }
#endif

            foreach (var entity in entities)
            {
                entity.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            EntityBatch.Begin(transformMatrix: vpAdapter.GetScaleMatrix());
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        EntityBatch.Draw(Textures.TexDebugTile, new Rectangle(x * TILE_WIDTH, y * TILE_WIDTH, TILE_WIDTH, TILE_WIDTH), Color.White);
                    }
                }

                foreach (var entity in entities)
                {
                    entity.Draw(gameTime);
                }
            }
            EntityBatch.End();
        }
    }
}
