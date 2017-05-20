using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens.Background;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens;

namespace GridDominance.Shared.Screens.NormalGameScreen.Background
{
	class GDStaticGridBackground : GameBackground, IGDGridBackground
	{
		private const int TILE_COUNT_X = GDConstants.GRID_WIDTH;
		private const int TILE_COUNT_Y = GDConstants.GRID_HEIGHT;

		public GDStaticGridBackground(GameScreen scrn) : base(scrn)
		{

		}
		
		public override void Draw(IBatchRenderer sbatch)
		{
			int extensionX = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetX / GDConstants.TILE_WIDTH);
			int extensionY = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetY / GDConstants.TILE_WIDTH);

			sbatch.DrawStretched(
				Textures.TexPixel, 
				new FRectangle(
					-extensionX * GDConstants.TILE_WIDTH, 
					-extensionY * GDConstants.TILE_WIDTH, 
					(TILE_COUNT_X + 2* extensionX + 1) * GDConstants.TILE_WIDTH, 
					(TILE_COUNT_Y + 2 * extensionY + 1) * GDConstants.TILE_WIDTH), 
				FlatColors.Background);

			for (int x = -extensionX; x < TILE_COUNT_X + extensionX; x++)
			{
				for (int y = -extensionY; y < TILE_COUNT_Y + extensionY; y++)
				{
					sbatch.DrawStretched(Textures.TexTileBorder, new FRectangle(x * GDConstants.TILE_WIDTH, y * GDConstants.TILE_WIDTH, GDConstants.TILE_WIDTH, GDConstants.TILE_WIDTH), Color.White);
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
