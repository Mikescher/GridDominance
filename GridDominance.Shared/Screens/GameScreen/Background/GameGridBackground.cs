using GridDominance.Shared.Framework;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

namespace GridDominance.Shared.Screens.GameScreen.Background
{
    class GameGridBackground : IDraw
    {
        private const int TILE_WIDTH = GameScreen.TILE_WIDTH;

        private const int TILE_COUNT_X = 8;
        private const int TILE_COUNT_Y = 5;

        protected readonly GameScreen Owner;
        protected readonly GraphicsDevice Graphics;
        protected readonly TolerantBoxingViewportAdapter Adapter;

        public GameGridBackground(GameScreen screen, GraphicsDevice graphicsDevice, TolerantBoxingViewportAdapter adapter)
        {
            Owner = screen;
            Graphics = graphicsDevice;
            Adapter = adapter;
        }

        public void Draw(GameTime gameTime)
        {
            int extensionX = FloatMath.Ceiling((Graphics.Viewport.Width - Adapter.RealWidth) / (TILE_WIDTH * 2f));
            int extensionY = FloatMath.Ceiling((Graphics.Viewport.Height - Adapter.RealHeight) / (TILE_WIDTH * 2f));

            for (int x = -extensionX; x < TILE_COUNT_X + extensionX; x++)
            {
                for (int y = -extensionY; y < TILE_COUNT_Y + extensionY; y++)
                {
                    Owner.EntityBatch.Draw(Textures.TexDebugTile, new Rectangle(x * TILE_WIDTH, y * TILE_WIDTH, TILE_WIDTH, TILE_WIDTH), Color.White);
                }
            }
        }
    }
}
