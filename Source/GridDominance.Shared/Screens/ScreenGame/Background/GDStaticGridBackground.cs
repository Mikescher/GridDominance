using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens.Background;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens;

namespace GridDominance.Shared.Screens.ScreenGame.Background
{
	class GDStaticGridBackground : GameBackground, IGDGridBackground
	{
		private const int TILE_COUNT_X = GDConstants.GRID_WIDTH;
		private const int TILE_COUNT_Y = GDConstants.GRID_HEIGHT;

		public int ParticleCount => -1;

		public GDStaticGridBackground(GameScreen scrn) : base(scrn)
		{

		}
		
		public override void Draw(IBatchRenderer sbatch)
		{
			int extensionX = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetX / GDConstants.TILE_WIDTH);
			int extensionY = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetY / GDConstants.TILE_WIDTH);

			for (int x = -extensionX; x < TILE_COUNT_X + extensionX; x++)
			{
				for (int y = -extensionY; y < TILE_COUNT_Y + extensionY; y++)
				{
					var color = FlatColors.Background;

					sbatch.DrawStretched(Textures.TexPixel, new FRectangle(x * GDConstants.TILE_WIDTH, y * GDConstants.TILE_WIDTH, GDConstants.TILE_WIDTH, GDConstants.TILE_WIDTH), color);
					sbatch.DrawStretched(Textures.TexTileBorder, new FRectangle(x * GDConstants.TILE_WIDTH, y * GDConstants.TILE_WIDTH, GDConstants.TILE_WIDTH, GDConstants.TILE_WIDTH), Color.White);
				}
			}
		}
		
		public override void Update(SAMTime gameTime, InputState state)
		{
			//
		}

		public void RegisterBlockedSpawn(Cannon cannon, int i, int i1)
		{
			// NOTHING
		}

		public void DeregisterBlockedSpawn(Cannon cannon, int spawnX, int spawnY)
		{
			// NOTHING
		}

		public void SpawnParticles(Fraction fraction, int spawnX, int spawnY)
		{
			// NOTHING
		}
	}
}
