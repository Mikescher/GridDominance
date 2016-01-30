using GridDominance.Shared.Framework;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

namespace GridDominance.Shared.Screens.GameScreen.Background
{
    class GameGridBackground
    {
        private const int TILE_WIDTH = GameScreen.TILE_WIDTH;

        private const int TILE_COUNT_X = 8;
        private const int TILE_COUNT_Y = 5;
		
        protected readonly GraphicsDevice Graphics;
        protected readonly TolerantBoxingViewportAdapter Adapter;

        public GameGridBackground(GraphicsDevice graphicsDevice, TolerantBoxingViewportAdapter adapter)
        {
            Graphics = graphicsDevice;
            Adapter = adapter;
        }

        public void Draw(SpriteBatch sbatch)
        {
            int extensionX = FloatMath.Ceiling((Graphics.Viewport.Width - Adapter.RealWidth) / (TILE_WIDTH * 2f));
            int extensionY = FloatMath.Ceiling((Graphics.Viewport.Height - Adapter.RealHeight) / (TILE_WIDTH * 2f));

            for (int x = -extensionX; x < TILE_COUNT_X + extensionX; x++)
            {
                for (int y = -extensionY; y < TILE_COUNT_Y + extensionY; y++)
                {
					sbatch.Draw(Textures.TexDebugTile, new Rectangle(x * TILE_WIDTH, y * TILE_WIDTH, TILE_WIDTH, TILE_WIDTH), Color.White);
                }
            }

#if DEBUG
            for (int x = 0; x < TILE_COUNT_X; x++)
                for (int y = 0; y < TILE_COUNT_Y; y++)
					sbatch.DrawRectangle(new Rectangle(x * TILE_WIDTH, y * TILE_WIDTH, TILE_WIDTH, TILE_WIDTH), Color.Magenta);
#endif
        }
    }
}
