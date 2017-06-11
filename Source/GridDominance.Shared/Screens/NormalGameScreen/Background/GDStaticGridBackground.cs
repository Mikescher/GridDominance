using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens;

namespace GridDominance.Shared.Screens.NormalGameScreen.Background
{
	class GDStaticGridBackground : GameBackground, IGDGridBackground
	{
		private const int TILE_WIDTH = GDConstants.TILE_WIDTH;

		public GDStaticGridBackground(GameScreen scrn) : base(scrn)
		{

		}
		
		public override void Draw(IBatchRenderer sbatch)
		{
			int offX = TILE_WIDTH * (int)(Owner.MapOffsetX / TILE_WIDTH);
			int offY = TILE_WIDTH * (int)(Owner.MapOffsetY / TILE_WIDTH);

			int extensionX = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetX / TILE_WIDTH) + 2;
			int extensionY = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetY / TILE_WIDTH) + 2;

			int countX = FloatMath.Ceiling(VAdapter.VirtualGuaranteedWidth / TILE_WIDTH);
			int countY = FloatMath.Ceiling(VAdapter.VirtualGuaranteedHeight / TILE_WIDTH);

			var r = new FRectangle(-extensionX * TILE_WIDTH - offX, -extensionY * TILE_WIDTH - offY, (countX + 2 * extensionX) * TILE_WIDTH, (countY + 2 * extensionY) * TILE_WIDTH);

			sbatch.DrawStretched(Textures.TexPixel, r, FlatColors.Background);

			for (int x = -extensionX; x < countX + extensionX; x++)
			{
				for (int y = -extensionY; y < countY + extensionY; y++)
				{
					sbatch.DrawStretched(Textures.TexTileBorder, new FRectangle(x * TILE_WIDTH - offX, y * TILE_WIDTH - offY, TILE_WIDTH, TILE_WIDTH), Color.White);
				}
			}
		}
		
		public override void Update(SAMTime gameTime, InputState state)
		{
			//
		}

		public void RegisterSpawn(Cannon cannon, FCircle circle)
		{
			// NOTHING
		}

		public void RegisterBlockedLine(Vector2 start, Vector2 end)
		{
			// NOTHING
		}

		public void RegisterBlockedCircle(FCircle c)
		{
			// NOTHING
		}

		public void RegisterBlockedBlock(FRectangle block)
		{
			// NOTHING
		}
	}
}
