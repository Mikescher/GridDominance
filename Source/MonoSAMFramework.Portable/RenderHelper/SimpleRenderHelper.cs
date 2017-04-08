using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using System;

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

		public static void DrawRoundedRectOutline(IBatchRenderer sbatch, FRectangle bounds, Color color, int sides = 8, float lineWidth = 1f, float cornerSize = 16f)
		{
			DrawRoundedRectOutline(sbatch, bounds, color, true, true, true, true, sides, lineWidth, cornerSize);
		}

		public static void DrawRoundedRectOutline(IBatchRenderer sbatch, FRectangle bounds, Color color, bool tl, bool tr, bool bl, bool br, int sides = 8, float lineWidth = 1f, float cornerSize = 16f)
		{
			StaticTextures.ThrowIfNotInitialized();
			
			// LEFT
			sbatch.DrawStretched(
				StaticTextures.SinglePixel,
				new FRectangle(bounds.Left, bounds.Top + (tl ? cornerSize : 0), lineWidth, bounds.Height - (tl ? cornerSize : 0) - (bl ? cornerSize : 0)),
				color);

			// TOP
			sbatch.DrawStretched(
				StaticTextures.SinglePixel,
				new FRectangle(bounds.Left + (tl ? cornerSize : 0), bounds.Top, bounds.Width - (tl ? cornerSize : 0) - (tr ? cornerSize : 0), lineWidth),
				color);

			// RIGHT
			sbatch.DrawStretched(
				StaticTextures.SinglePixel,
				new FRectangle(bounds.Right - lineWidth, bounds.Top + (tr ? cornerSize : 0), lineWidth, bounds.Height - (tr ? cornerSize : 0) - (br ? cornerSize : 0)),
				color);

			// BOTTOM
			sbatch.DrawStretched(
				StaticTextures.SinglePixel,
				new FRectangle(bounds.Left + (bl ? cornerSize : 0), bounds.Bottom - lineWidth, bounds.Width - (bl ? cornerSize : 0) - (br ? cornerSize : 0), lineWidth),
				color);

			if (br)
			{
				sbatch.DrawCirclePiece(new Vector2(bounds.Right - cornerSize, bounds.Bottom - cornerSize), cornerSize, FloatMath.RAD_POS_000, FloatMath.RAD_POS_090, 8, color, lineWidth);
			}

			if (bl)
			{
				sbatch.DrawCirclePiece(new Vector2(bounds.Left + cornerSize, bounds.Bottom - cornerSize), cornerSize, FloatMath.RAD_POS_090, FloatMath.RAD_POS_180, 8, color, lineWidth);
			}

			if (tl)
			{
				sbatch.DrawCirclePiece(new Vector2(bounds.Left + cornerSize, bounds.Top + cornerSize), cornerSize, FloatMath.RAD_POS_180, FloatMath.RAD_POS_270, 8, color, lineWidth);
			}

			if (tr)
			{
				sbatch.DrawCirclePiece(new Vector2(bounds.Right - cornerSize, bounds.Top + cornerSize), cornerSize, FloatMath.RAD_POS_270, FloatMath.RAD_POS_360, 8, color, lineWidth);
			}
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
			
			// LEFT
			sbatch.DrawStretched(
				StaticTextures.SinglePixel,
				new FRectangle(bounds.Left, bounds.Top, bsize, bounds.Height),
				color);

			// TOP
			sbatch.DrawStretched(
				StaticTextures.SinglePixel,
				new FRectangle(bounds.Left, bounds.Top, bounds.Width, bsize),
				color);

			// RIGHT
			sbatch.DrawStretched(
				StaticTextures.SinglePixel,
				new FRectangle(bounds.Right - bsize, bounds.Top, bsize, bounds.Height),
				color);

			// BOTTOM
			sbatch.DrawStretched(
				StaticTextures.SinglePixel,
				new FRectangle(bounds.Left, bounds.Bottom - bsize, bounds.Width, bsize),
				color);
		}

		public static void DrawHUDBackground(IBatchRenderer sbatch, HUDBackgroundType type, FRectangle bounds, Color col, float cornerSize)
		{
			if (col == Color.Transparent) return;

			switch (type)
			{
				case HUDBackgroundType.None:
					// Do nothing
					break;
				case HUDBackgroundType.Simple:
					DrawSimpleRect(sbatch, bounds, col);
					break;
				case HUDBackgroundType.Rounded:
					DrawRoundedRect(sbatch, bounds, col, cornerSize);
					break;
				case HUDBackgroundType.RoundedBlur:
					FlatRenderHelper.DrawRoundedBlurPanel(sbatch, bounds, col, cornerSize);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
