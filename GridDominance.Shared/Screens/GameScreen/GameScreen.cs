using System;
using System.Collections.Generic;
using GridDominance.Shared.Framework;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.GameScreen.Background;
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

        private TolerantBoxingViewportAdapter vpAdapter;

        public SpriteBatch EntityBatch;

	    private GDEntityManager entities;
		private GameGridBackground background;

        public GameScreen(MainGame game, GraphicsDeviceManager gdm) : base(game, gdm)
        {
            Initialize();
        }

        private void Initialize()
        {
            EntityBatch = new SpriteBatch(Graphics.GraphicsDevice);
            vpAdapter = new TolerantBoxingViewportAdapter(Owner.Window, Graphics, VIEW_WIDTH, VIEW_HEIGHT);
            background = new GameGridBackground(Graphics.GraphicsDevice, vpAdapter);
			entities = new GDEntityManager();
			//--------------------

			entities.AddEntity(new Cannon(2, 3));
            entities.AddEntity(new Cannon(2, 0));
            entities.AddEntity(new Cannon(5, 1));
            entities.AddEntity(new Cannon(6, 2));
        }

        public override void Update(GameTime gameTime)
        {
#if !__IOS__
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Owner.Exit();
            }
#endif

			entities.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Graphics.GraphicsDevice.Clear(Color.AliceBlue);

            EntityBatch.Begin(transformMatrix: vpAdapter.GetScaleMatrix());
            {
                background.Draw(EntityBatch);

                entities.Draw(EntityBatch);
            }
            EntityBatch.End();
        }
    }
}
