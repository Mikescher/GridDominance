using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame;
using GridDominance.Shared.Screens.WorldMapScreen.Agents;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
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
			int offX = TILE_WIDTH * (int)(Owner.MapOffsetX / TILE_WIDTH);
			int offY = TILE_WIDTH * (int)(Owner.MapOffsetY / TILE_WIDTH);

			int extensionX = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetX / TILE_WIDTH) + 2;
			int extensionY = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetY / TILE_WIDTH) + 2;

			int countX = FloatMath.Ceiling(VAdapter.VirtualGuaranteedWidth / TILE_WIDTH);
			int countY = FloatMath.Ceiling(VAdapter.VirtualGuaranteedHeight / TILE_WIDTH);

			sbatch.Draw(
				Textures.TexPixel, 
				new Rectangle(
					-extensionX * TILE_WIDTH - offX, 
					-extensionY * TILE_WIDTH - offY, 
					(countX + 2*extensionX) * TILE_WIDTH, 
					(countY + 2 * extensionY) * TILE_WIDTH), 
				FlatColors.Background);

			for (int x = -extensionX; x < countX + extensionX; x++)
			{
				for (int y = -extensionY; y < countY + extensionY; y++)
				{
					sbatch.Draw(Textures.TexTileBorder, new Rectangle(x * TILE_WIDTH - offX, y * TILE_WIDTH - offY, TILE_WIDTH, TILE_WIDTH), Color.White);
				}
			}
		}
	}
}