using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;

namespace MonoSAMFramework.Portable.RenderHelper
{
	public static class SimpleRenderHelper
	{
		public static void DrawRoundedRect(IBatchRenderer sbatch, FRectangle bounds, Color color, float cornerSize = 16f)
		{
			DrawRoundedRect(sbatch, bounds, color, true, true, true, true, cornerSize);
		}
		
		public static void DrawRoundedRect(IBatchRenderer sbatch, FRectangle bounds, Color color, bool tl, bool tr, bool bl, bool br, float cornerSize = 16f)
		{
			StaticTextures.ThrowIfNotInitialized();

			if (!tl && !tr && !bl && !br)
			{
				DrawSimpleRect(sbatch, bounds, color);

				return;
			}
			
			#region Fill Center

			if (color.A == 255)
			{
				sbatch.DrawStretched(
					StaticTextures.SinglePixel,
					bounds.AsDeflated(cornerSize, 0),
					color);

				sbatch.DrawStretched(
					StaticTextures.SinglePixel,
					bounds.AsDeflated(0, cornerSize),
					color);
			}
			else
			{
				sbatch.DrawStretched(
					StaticTextures.SinglePixel,
					bounds.AsDeflated(cornerSize, 0),
					color);

				sbatch.DrawStretched(
					StaticTextures.SinglePixel,
					bounds.ToSubRectangle(cornerSize, bounds.Height - 2 * cornerSize, FlatAlign9.WEST),
					color);

				sbatch.DrawStretched(
					StaticTextures.SinglePixel,
					bounds.ToSubRectangle(cornerSize, bounds.Height - 2 * cornerSize, FlatAlign9.EAST),
					color);
			}


			#endregion

			#region Corners
			
			var r_tl = bounds.ToSquare(cornerSize, FlatAlign9.TL);
			var r_tr = bounds.ToSquare(cornerSize, FlatAlign9.TR);
			var r_br = bounds.ToSquare(cornerSize, FlatAlign9.BR);
			var r_bl = bounds.ToSquare(cornerSize, FlatAlign9.BL);

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
