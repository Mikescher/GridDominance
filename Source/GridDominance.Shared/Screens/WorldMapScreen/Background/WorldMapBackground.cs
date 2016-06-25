using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.MathHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;

namespace GridDominance.Shared.Screens.WorldMapScreen.Background
{
	public class WorldMapBackground : GameBackground
	{
		private const int TILE_WIDTH = GDGameScreen.TILE_WIDTH;

		public WorldMapBackground(GameScreen scrn) : base(scrn)
		{
		}

		public override void Update(GameTime gameTime, InputState istate)
		{
			//
		}

		public override void Draw(IBatchRenderer sbatch)
		{
			int extensionX = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetX / TILE_WIDTH);
			int extensionY = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetY / TILE_WIDTH);

			int countX = FloatMath.Ceiling(VAdapter.VirtualGuaranteedWidth / TILE_WIDTH);
			int countY = FloatMath.Ceiling(VAdapter.VirtualGuaranteedHeight / TILE_WIDTH);

			for (int x = -extensionX; x < countX + extensionX; x++)
			{
				for (int y = -extensionY; y < countY + extensionY; y++)
				{
					var color = FlatColors.Background;

					sbatch.Draw(Textures.TexPixel, new Rectangle(x * TILE_WIDTH, y * TILE_WIDTH, TILE_WIDTH, TILE_WIDTH), color);
					sbatch.Draw(Textures.TexTileBorder, new Rectangle(x * TILE_WIDTH, y * TILE_WIDTH, TILE_WIDTH, TILE_WIDTH), Color.White);
				}
			}
		}
	}
}