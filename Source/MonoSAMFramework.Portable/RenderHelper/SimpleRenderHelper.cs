using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using System;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;

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
				sbatch.DrawCirclePiece(new FPoint(bounds.Right - cornerSize, bounds.Bottom - cornerSize), cornerSize, FloatMath.RAD_POS_000, FloatMath.RAD_POS_090, 8, color, lineWidth);
			}

			if (bl)
			{
				sbatch.DrawCirclePiece(new FPoint(bounds.Left + cornerSize, bounds.Bottom - cornerSize), cornerSize, FloatMath.RAD_POS_090, FloatMath.RAD_POS_180, 8, color, lineWidth);
			}

			if (tl)
			{
				sbatch.DrawCirclePiece(new FPoint(bounds.Left + cornerSize, bounds.Top + cornerSize), cornerSize, FloatMath.RAD_POS_180, FloatMath.RAD_POS_270, 8, color, lineWidth);
			}

			if (tr)
			{
				sbatch.DrawCirclePiece(new FPoint(bounds.Right - cornerSize, bounds.Top + cornerSize), cornerSize, FloatMath.RAD_POS_270, FloatMath.RAD_POS_360, 8, color, lineWidth);
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
				case HUDBackgroundType.SimpleBlur:
					FlatRenderHelper.DrawSimpleBlurPanel(sbatch, bounds, col, cornerSize);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static void DrawAlphaHUDBackground(IBatchRenderer sbatch, HUDBackgroundType type, FRectangle bounds, Color col, float cornerSize, float alpha)
		{
			if (col == Color.Transparent) return;
			if (FloatMath.IsZero(alpha)) return;

			switch (type)
			{
				case HUDBackgroundType.None:
					// Do nothing
					break;
				case HUDBackgroundType.Simple:
					DrawSimpleRect(sbatch, bounds, col * alpha);
					break;
				case HUDBackgroundType.Rounded:
					DrawRoundedRect(sbatch, bounds, col * alpha, cornerSize);
					break;
				case HUDBackgroundType.RoundedBlur:
					FlatRenderHelper.DrawRoundedAlphaBlurPanel(sbatch, bounds, col, cornerSize, alpha);
					break;
				case HUDBackgroundType.SimpleBlur:
					FlatRenderHelper.DrawSimpleAlphaBlurPanel(sbatch, bounds, col, cornerSize, alpha);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static void Draw9Patch(IBatchRenderer sbatch, FRotatedRectangle bounds, Color colEdge, Color colCorner, Color colFill, TextureRegion2D texEdge, TextureRegion2D texCorner, TextureRegion2D texFill, float cornerSize)
		{
			Draw9Patch(sbatch, bounds.WithNoRotation(), colEdge, colCorner, colFill, texEdge, texCorner, texFill, cornerSize, bounds.Rotation);
		}

		public static void Draw9Patch(IBatchRenderer sbatch, FRectangle bounds, Color colEdge, Color colCorner, Color colFill, TextureRegion2D texEdge, TextureRegion2D texCorner, TextureRegion2D texFill, float cornerSize, float rotation)
		{
			rotation = FloatMath.NormalizeAngle(rotation);
			
			if (FloatMath.EpsilonEquals(rotation, FloatMath.RAD_POS_000)) { DrawAligned9Patch(sbatch, bounds.AsRotated(PerpendicularRotation.DEGREE_CW_000), colEdge, colCorner, colFill, texEdge, texCorner, texFill, cornerSize); return; }
			if (FloatMath.EpsilonEquals(rotation, FloatMath.RAD_POS_090)) { DrawAligned9Patch(sbatch, bounds.AsRotated(PerpendicularRotation.DEGREE_CW_090), colEdge, colCorner, colFill, texEdge, texCorner, texFill, cornerSize); return; }
			if (FloatMath.EpsilonEquals(rotation, FloatMath.RAD_POS_180)) { DrawAligned9Patch(sbatch, bounds.AsRotated(PerpendicularRotation.DEGREE_CW_180), colEdge, colCorner, colFill, texEdge, texCorner, texFill, cornerSize); return; }
			if (FloatMath.EpsilonEquals(rotation, FloatMath.RAD_POS_270)) { DrawAligned9Patch(sbatch, bounds.AsRotated(PerpendicularRotation.DEGREE_CW_270), colEdge, colCorner, colFill, texEdge, texCorner, texFill, cornerSize); return; }
			if (FloatMath.EpsilonEquals(rotation, FloatMath.RAD_POS_360)) { DrawAligned9Patch(sbatch, bounds.AsRotated(PerpendicularRotation.DEGREE_CW_360), colEdge, colCorner, colFill, texEdge, texCorner, texFill, cornerSize); return; }

			var ctr = bounds.Center;
			
			var r_tl = new FRectangle(bounds.Left  - cornerSize, bounds.Top    - cornerSize, 2 * cornerSize, 2 * cornerSize);
			var r_tr = new FRectangle(bounds.Right - cornerSize, bounds.Top    - cornerSize, 2 * cornerSize, 2 * cornerSize);
			var r_br = new FRectangle(bounds.Right - cornerSize, bounds.Bottom - cornerSize, 2 * cornerSize, 2 * cornerSize);
			var r_bl = new FRectangle(bounds.Left  - cornerSize, bounds.Bottom - cornerSize, 2 * cornerSize, 2 * cornerSize);

			var r_t  = new FRectangle(bounds.Left  + cornerSize, bounds.Top    - cornerSize, bounds.Width  - 2 * cornerSize, 2 * cornerSize);
			var r_b  = new FRectangle(bounds.Left  + cornerSize, bounds.Bottom - cornerSize, bounds.Width  - 2 * cornerSize, 2 * cornerSize);
			
			var r_r  = FRectangle.CreateByCenter(bounds.Right, bounds.CenterY, bounds.Height - 2 * cornerSize, 2 * cornerSize);
			var r_l  = FRectangle.CreateByCenter(bounds.Left,  bounds.CenterY, bounds.Height - 2 * cornerSize, 2 * cornerSize);

			var r_c = new FRectangle(bounds.Left + cornerSize, bounds.Top + cornerSize, bounds.Width - 2 * cornerSize, bounds.Height - 2 * cornerSize);

			r_tl = r_tl.AsRotateCenterAround(ctr, rotation);
			r_tr = r_tr.AsRotateCenterAround(ctr, rotation);
			r_br = r_br.AsRotateCenterAround(ctr, rotation);
			r_bl = r_bl.AsRotateCenterAround(ctr, rotation);
			
			r_t  = r_t.AsRotateCenterAround(ctr, rotation);
			r_r  = r_r.AsRotateCenterAround(ctr, rotation);
			r_b  = r_b.AsRotateCenterAround(ctr, rotation);
			r_l  = r_l.AsRotateCenterAround(ctr, rotation);

			sbatch.DrawStretched(texEdge, r_t, colEdge, FloatMath.RAD_POS_000 + rotation);
			sbatch.DrawStretched(texEdge, r_r, colEdge, FloatMath.RAD_POS_090 + rotation);
			sbatch.DrawStretched(texEdge, r_b, colEdge, FloatMath.RAD_POS_180 + rotation);
			sbatch.DrawStretched(texEdge, r_l, colEdge, FloatMath.RAD_POS_270 + rotation);

			sbatch.DrawStretched(texCorner, r_tl, colCorner, FloatMath.RAD_POS_000 + rotation);
			sbatch.DrawStretched(texCorner, r_tr, colCorner, FloatMath.RAD_POS_090 + rotation);
			sbatch.DrawStretched(texCorner, r_br, colCorner, FloatMath.RAD_POS_180 + rotation);
			sbatch.DrawStretched(texCorner, r_bl, colCorner, FloatMath.RAD_POS_270 + rotation);
			
			sbatch.DrawStretched(texFill, r_c, colFill, rotation);
		}
		
		public static void DrawAligned9Patch(IBatchRenderer sbatch, FRectangle bounds, Color colEdge, Color colCorner, Color colFill, TextureRegion2D texEdge, TextureRegion2D texCorner, TextureRegion2D texFill, float cornerSize)
		{
			var r_tl = new FRectangle(bounds.Left - cornerSize, bounds.Top - cornerSize, 2 * cornerSize, 2 * cornerSize);
			var r_tr = new FRectangle(bounds.Right - cornerSize, bounds.Top - cornerSize, 2 * cornerSize, 2 * cornerSize);
			var r_br = new FRectangle(bounds.Right - cornerSize, bounds.Bottom - cornerSize, 2 * cornerSize, 2 * cornerSize);
			var r_bl = new FRectangle(bounds.Left - cornerSize, bounds.Bottom - cornerSize, 2 * cornerSize, 2 * cornerSize);

			var r_l = new FRectangle(r_tl.Left, r_tl.Bottom, r_tl.Width, r_bl.Top - r_tl.Bottom);
			var r_t = new FRectangle(r_tl.Right, r_tl.Top, r_tr.Left - r_tl.Right, r_tl.Height);
			var r_r = new FRectangle(r_tr.Left, r_tr.Bottom, r_tr.Width, r_br.Top - r_tr.Bottom);
			var r_b = new FRectangle(r_bl.Right, r_bl.Top, r_br.Left - r_bl.Right, r_bl.Height);

			var r_c = new FRectangle(bounds.Left + cornerSize, bounds.Top + cornerSize, bounds.Width - 2 * cornerSize, bounds.Height - 2 * cornerSize);

			// Top
			sbatch.DrawRot000(texEdge, r_t, colEdge, 0);

			// Right
			sbatch.DrawRot090(texEdge, r_r, colEdge, 0);

			// Bottom
			sbatch.DrawRot180(texEdge, r_b, colEdge, 0);

			// Left
			sbatch.DrawRot270(texEdge, r_l, colEdge, 0);

			// TL
			sbatch.DrawRot000(texCorner, r_tl, colCorner, 0);

			// TR
			sbatch.DrawRot090(texCorner, r_tr, colCorner, 0);

			// BR
			sbatch.DrawRot180(texCorner, r_br, colCorner, 0);

			// BL
			sbatch.DrawRot270(texCorner, r_bl, colCorner, 0);

			// Center
			sbatch.DrawStretched(texFill, r_c, colFill, 0);
		}

		public static void DrawCross(IBatchRenderer sbatch, FRectangle rect, Color color, float thickness)
		{
			sbatch.DrawLine(rect.TopLeft, rect.BottomRight, color, thickness);
			sbatch.DrawLine(rect.TopRight, rect.BottomLeft, color, thickness);
		}
	}
}
