using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.RenderHelper
{
	public static class FlatRenderHelper
	{
		public const int CROP_CORNER_SIZE = SimpleRenderHelper.CROP_CORNER_SIZE;
		public const int TEX_CORNER_SIZE = SimpleRenderHelper.TEX_CORNER_SIZE;

		public static readonly Vector2 CORNER_VECTOR_TL = new Vector2(-CROP_CORNER_SIZE, -CROP_CORNER_SIZE);
		public static readonly Vector2 CORNER_VECTOR_TR = new Vector2(+CROP_CORNER_SIZE, -CROP_CORNER_SIZE);
		public static readonly Vector2 CORNER_VECTOR_BL = new Vector2(-CROP_CORNER_SIZE, +CROP_CORNER_SIZE);
		public static readonly Vector2 CORNER_VECTOR_BR = new Vector2(+CROP_CORNER_SIZE, +CROP_CORNER_SIZE);

		public static void DrawRoundedBlurPanel(IBatchRenderer sbatch, FRectangle bounds, Color color, float scale = 1f)
		{
			StaticTextures.ThrowIfNotInitialized();

			DrawRoundedBlurPanelBackgroundPart(sbatch, bounds, scale);
			DrawRoundedBlurPanelSolidPart(sbatch, bounds, color, scale);
		}

		public static void DrawRoundedBlurPanelSolidPart(IBatchRenderer sbatch, FRectangle bounds, Color color, float scale = 1f)
		{
			StaticTextures.ThrowIfNotInitialized();

			#region Fill Center

			sbatch.Draw(
				StaticTextures.SinglePixel.Texture,
				bounds.AsDeflated(CROP_CORNER_SIZE * scale, 0),
				StaticTextures.SinglePixel.Bounds,
				color);

			sbatch.Draw(
				StaticTextures.SinglePixel.Texture,
				bounds.AsDeflated(0, CROP_CORNER_SIZE * scale),
				StaticTextures.SinglePixel.Bounds,
				color);

			#endregion

			#region Corners

			sbatch.Draw(
				StaticTextures.PanelCorner.Texture,
				bounds.VectorTopLeft,
				StaticTextures.PanelCorner.Bounds,
				color,
				0 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				scale * StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelCorner.Texture,
				bounds.VectorTopRight,
				StaticTextures.PanelCorner.Bounds,
				color,
				90 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				scale * StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelCorner.Texture,
				bounds.VectorBottomRight,
				StaticTextures.PanelCorner.Bounds,
				color,
				180 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				scale * StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelCorner.Texture,
				bounds.VectorBottomLeft,
				StaticTextures.PanelCorner.Bounds,
				color,
				270 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				scale * StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			#endregion
		}

		public static void DrawRoundedBlurPanelBackgroundPart(IBatchRenderer sbatch, FRectangle bounds, float scale = 1f)
		{
			StaticTextures.ThrowIfNotInitialized();

			var cornerSize = (int) (scale * CROP_CORNER_SIZE);

			#region Blur Edges

			sbatch.Draw(
				StaticTextures.PanelBlurEdge.Texture,
				new FRectangle(
					bounds.Left + cornerSize,
					bounds.Top - cornerSize,
					bounds.Width - 2 * cornerSize,
					2 * cornerSize),
				StaticTextures.PanelBlurEdge.Bounds,
				Color.White,
				0 * FloatMath.DegreesToRadians,
				Vector2.Zero, SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelBlurEdge.Texture,
				new FRectangle(
					bounds.Right - cornerSize + (2 * cornerSize),
					bounds.Top + cornerSize,
					bounds.Height - 2 * cornerSize,
					2 * cornerSize
					),
				StaticTextures.PanelBlurEdge.Bounds,
				Color.White,
				90 * FloatMath.DegreesToRadians,
				Vector2.Zero, SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelBlurEdge.Texture,
				new FRectangle(
					bounds.Left + cornerSize + (bounds.Width - 2 * cornerSize),
					bounds.Bottom - cornerSize + (2 * cornerSize),
					bounds.Width - 2 * cornerSize,
					2 * cornerSize),
				StaticTextures.PanelBlurEdge.Bounds,
				Color.White,
				180 * FloatMath.DegreesToRadians,
				Vector2.Zero, SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelBlurEdge.Texture,
				new FRectangle(
					bounds.Left - cornerSize,
					bounds.Top + cornerSize + (bounds.Height - 2 * cornerSize),
					bounds.Height - 2 * cornerSize,
					2 * cornerSize),
				StaticTextures.PanelBlurEdge.Bounds,
				Color.White,
				270 * FloatMath.DegreesToRadians,
				Vector2.Zero, SpriteEffects.None, 0);

			#endregion

			#region Blur Corners

			sbatch.Draw(
				StaticTextures.PanelBlurCorner.Texture,
				bounds.VectorTopLeft + scale * CORNER_VECTOR_TL,
				StaticTextures.PanelBlurCorner.Bounds,
				Color.White,
				0 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				scale * StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelBlurCorner.Texture,
				bounds.VectorTopRight + scale * CORNER_VECTOR_TR,
				StaticTextures.PanelBlurCorner.Bounds,
				Color.White,
				90 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				scale * StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelBlurCorner.Texture,
				bounds.VectorBottomRight + scale * CORNER_VECTOR_BR,
				StaticTextures.PanelBlurCorner.Bounds,
				Color.White,
				180 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				scale * StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelBlurCorner.Texture,
				bounds.VectorBottomLeft + scale * CORNER_VECTOR_BL,
				StaticTextures.PanelBlurCorner.Bounds,
				Color.White,
				270 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				scale * StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			#endregion
		}

		private static float iii = 0;
		public static void DrawDropShadow(IBatchRenderer sbatch, FRectangle bounds, float sOutset, float sInset)
		{
			StaticTextures.ThrowIfNotInitialized();

			var r_tl = new FRectangle(
				bounds.Left - sOutset,
				bounds.Top - sOutset,
				sOutset + sInset,
				sOutset + sInset).Round();

			var r_tr = new FRectangle(
				bounds.Right - sInset,
				bounds.Top - sOutset,
				sOutset + sInset,
				sOutset + sInset).Round();

			var r_br = new FRectangle(
				bounds.Right - sInset,
				bounds.Bottom - sInset,
				sOutset + sInset,
				sOutset + sInset).Round();

			var r_bl = new FRectangle(
				bounds.Left - sOutset,
				bounds.Bottom - sInset,
				sOutset + sInset,
				sOutset + sInset).Round();

			var r_l = new Rectangle(r_tl.Left, r_tl.Bottom, r_tl.Width, r_bl.Top - r_tl.Bottom);

			var r_t = new Rectangle(r_tl.Right, r_tl.Top, r_tr.Left - r_tl.Right, r_tl.Height);

			var r_r = new Rectangle(r_tr.Left, r_tr.Bottom, r_tr.Width, r_br.Top - r_tr.Bottom);

			var r_b = new Rectangle(r_bl.Right, r_bl.Top, r_br.Left - r_bl.Right, r_bl.Height);

			#region Blur Edges

			// Top
			sbatch.DrawRot000(StaticTextures.PanelBlurEdge.Texture, r_t, StaticTextures.PanelBlurEdge.Bounds, Color.White, 0);

			// Right
			sbatch.DrawRot090(StaticTextures.PanelBlurEdge.Texture, r_r, StaticTextures.PanelBlurEdge.Bounds, Color.White, 0);

			// Bottom
			sbatch.DrawRot180(StaticTextures.PanelBlurEdge.Texture, r_b, StaticTextures.PanelBlurEdge.Bounds, Color.White, 0);

			// Left
			sbatch.DrawRot270(StaticTextures.PanelBlurEdge.Texture, r_l, StaticTextures.PanelBlurEdge.Bounds, Color.White, 0);

			#endregion

			#region Blur Corners

			// TL
			sbatch.Draw(
				StaticTextures.PanelBlurCorner.Texture,
				r_tl,
				StaticTextures.PanelBlurCorner.Bounds,
				Color.White,
				0,
				Vector2.Zero, SpriteEffects.None, 0);

			// TR
			sbatch.Draw(
				StaticTextures.PanelBlurCorner.Texture,
				r_tr,
				StaticTextures.PanelBlurCorner.Bounds,
				Color.White,
				0,
				Vector2.Zero, SpriteEffects.FlipHorizontally, 0);

			// BR
			sbatch.Draw(
				StaticTextures.PanelBlurCorner.Texture,
				r_br,
				StaticTextures.PanelBlurCorner.Bounds,
				Color.White,
				0,
				Vector2.Zero, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0);

			// BL
			sbatch.Draw(
				StaticTextures.PanelBlurCorner.Texture,
				r_bl,
				StaticTextures.PanelBlurCorner.Bounds,
				Color.White,
				0,
				Vector2.Zero, SpriteEffects.FlipVertically, 0);

			#endregion
		}

		public static void DrawOutlinesBlurRectangle(IBatchRenderer sbatch, FRectangle bounds, float borderWidth, Color cInner, Color cBorder, float blurOuterWidth, float blurInset)
		{
			StaticTextures.ThrowIfNotInitialized();

			DrawDropShadow(sbatch, bounds, blurOuterWidth, blurInset);

			SimpleRenderHelper.DrawSimpleRect(sbatch, bounds.AsDeflated(borderWidth / 2f, borderWidth / 2f), cInner);
			
			SimpleRenderHelper.DrawSimpleRectOutline(sbatch, bounds, borderWidth, cBorder);
		}
	}
}