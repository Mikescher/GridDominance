using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.Entities.Cannons;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.RenderHelper;

namespace GridDominance.Shared.Screens.NormalGameScreen.Background
{
	class GDStaticGridBackground : GameBackground, IGDGridBackground
	{
		private const int TILE_WIDTH = GDConstants.TILE_WIDTH;

		private readonly GameWrapMode _wrapMode;
		private readonly GDGameScreen _gdowner;

		public GDStaticGridBackground(GDGameScreen scrn, GameWrapMode mode) : base(scrn)
		{
			_wrapMode = mode;
			_gdowner = scrn;
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

		public override void DrawOverlay(IBatchRenderer sbatch)
		{
			int extensionX = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetX / TILE_WIDTH) + 2;
			int extensionY = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetY / TILE_WIDTH) + 2;

			if (_wrapMode == GameWrapMode.Donut || _wrapMode == GameWrapMode.Reflect)
			{
				var fextx = extensionX * TILE_WIDTH;
				var fexty = extensionY * TILE_WIDTH;

				var rn = new FRectangle(-fextx, -fexty, Owner.MapFullBounds.Width + 2 * fextx, fexty);
				var re = new FRectangle(Owner.MapFullBounds.Width, -fexty, fextx, Owner.MapFullBounds.Height + 2 * fexty);
				var rs = new FRectangle(-fextx, Owner.MapFullBounds.Height, Owner.MapFullBounds.Width + 2 * fextx, fexty);
				var rw = new FRectangle(-fextx, -fexty, fextx, Owner.MapFullBounds.Height + 2 * fexty);

				sbatch.DrawStretched(Textures.TexPixel, rn, FlatColors.Clouds);
				sbatch.DrawStretched(Textures.TexPixel, re, FlatColors.Clouds);
				sbatch.DrawStretched(Textures.TexPixel, rs, FlatColors.Clouds);
				sbatch.DrawStretched(Textures.TexPixel, rw, FlatColors.Clouds);

				FlatRenderHelper.DrawForegroundDropShadow(sbatch, Owner.MapFullBounds, GDConstants.TILE_WIDTH / 2f, GDConstants.TILE_WIDTH / 2f);

				sbatch.DrawRectangle(Owner.MapFullBounds, Color.White, 3f);
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

		public void RegisterBlockedLine(FPoint start, FPoint end)
		{
			// NOTHING
		}

		public void RegisterBlockedCircle(FCircle c)
		{
			// NOTHING
		}

		public void RegisterBlockedBlock(FRectangle block, float rotation)
		{
			// NOTHING
		}
	}
}
