using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.RenderHelper
{
	public static class SimpleRenderHelper
	{
		public const int CROP_CORNER_SIZE = 16;
		public const int TEX_CORNER_SIZE = 24;

		public static void DrawRoundedRect(IBatchRenderer sbatch, FRectangle bounds, Color color, float cornerScale = 1)
		{
			DrawRoundedRect(sbatch, bounds, color, true, true, true, true, cornerScale);
		}
		
		public static void DrawRoundedRect(IBatchRenderer sbatch, FRectangle bounds, Color color, bool tl, bool tr, bool bl, bool br, float cornerScale = 1)
		{
			StaticTextures.ThrowIfNotInitialized();

			if (!tl && !tr && !bl && !br)
			{
				DrawSimpleRect(sbatch, bounds, color);

				return;
			}

			#region Fill Center

			sbatch.DrawStretched(
				StaticTextures.SinglePixel,
				bounds.AsDeflated(CROP_CORNER_SIZE * cornerScale, 0),
				color);

			sbatch.DrawStretched(
				StaticTextures.SinglePixel,
				bounds.AsDeflated(0, CROP_CORNER_SIZE * cornerScale),
				color);

			#endregion

			#region Corners

			var tcornerSize = TEX_CORNER_SIZE * cornerScale;

			var r_tl = new FRectangle(bounds.Left, bounds.Top, tcornerSize, TEX_CORNER_SIZE * cornerScale);
			var r_tr = new FRectangle(bounds.Right - tcornerSize, bounds.Top, tcornerSize, tcornerSize);
			var r_br = new FRectangle(bounds.Right - tcornerSize, bounds.Bottom - tcornerSize, tcornerSize, tcornerSize);
			var r_bl = new FRectangle(bounds.Left, bounds.Bottom - tcornerSize, tcornerSize, tcornerSize);

			if (tl)
			{
				sbatch.DrawRot000(StaticTextures.PanelCorner, r_tl, color, 0);
			}
			else
			{
				sbatch.DrawRot000(StaticTextures.SinglePixel, r_tl, color, 0);
			}

			if (tr)
			{
				sbatch.DrawRot090(StaticTextures.PanelCorner, r_tr, color, 0);
			}
			else
			{
				sbatch.DrawRot090(StaticTextures.SinglePixel, r_tr, color, 0);
			}

			if (br)
			{
				sbatch.DrawRot180(StaticTextures.PanelCorner, r_br, color, 0);
			}
			else
			{
				sbatch.DrawRot180(StaticTextures.SinglePixel, r_br, color, 0);
			}

			if (bl)
			{
				sbatch.DrawRot270(StaticTextures.PanelCorner, r_bl, color, 0);
			}
			else
			{
				sbatch.DrawRot270(StaticTextures.SinglePixel, r_bl, color, 0);
			}

			#endregion
		}

		public static void DrawSimpleRect(IBatchRenderer sbatch, FRectangle bounds, Color color)
		{
			StaticTextures.ThrowIfNotInitialized();

			sbatch.DrawStretched(
				StaticTextures.SinglePixel,
				bounds,
				color);
		}

		public static void DrawSimpleRectOutline(IBatchRenderer sbatch, FRectangle bounds, float bsize, Color color)
		{
			StaticTextures.ThrowIfNotInitialized();

			int borderSize = FloatMath.Round(bsize);

			// LEFT
			sbatch.DrawStretched(
				StaticTextures.SinglePixel,
				new FRectangle(bounds.Left, bounds.Top, borderSize, bounds.Height),
				color);

			// TOP
			sbatch.DrawStretched(
				StaticTextures.SinglePixel,
				new FRectangle(bounds.Left, bounds.Top, bounds.Width, borderSize),
				color);

			// RIGHT
			sbatch.DrawStretched(
				StaticTextures.SinglePixel,
				new FRectangle(bounds.Right - borderSize, bounds.Top, borderSize, bounds.Height),
				color);

			// BOTTOM
			sbatch.DrawStretched(
				StaticTextures.SinglePixel,
				new FRectangle(bounds.Left, bounds.Bottom - borderSize, bounds.Width, borderSize),
				color);
		}
	}
}
