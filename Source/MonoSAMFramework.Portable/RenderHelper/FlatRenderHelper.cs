using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.MathHelper;

namespace MonoSAMFramework.Portable.RenderHelper
{
	public static class FlatRenderHelper
	{
		public const int CORNER_SIZE = 16;

		public static readonly Vector2 CORNER_VECTOR_TL = new Vector2(-CORNER_SIZE, -CORNER_SIZE);
		public static readonly Vector2 CORNER_VECTOR_TR = new Vector2(+CORNER_SIZE, -CORNER_SIZE);
		public static readonly Vector2 CORNER_VECTOR_BL = new Vector2(-CORNER_SIZE, +CORNER_SIZE);
		public static readonly Vector2 CORNER_VECTOR_BR = new Vector2(+CORNER_SIZE, +CORNER_SIZE);

		public static void DrawRoundedBlurPanel(IBatchRenderer sbatch, Rectangle bounds, Color color, float scale = 1f)
		{
			StaticTextures.ThrowIfNotInitialized();

			DrawRoundedBlurPanelBackgroundPart(sbatch, bounds, scale);
			DrawRoundedBlurPanelSolidPart(sbatch, bounds, color, scale);
		}

		public static void DrawRoundedBlurPanelSolidPart(IBatchRenderer sbatch, Rectangle bounds, Color color, float scale = 1f)
		{
			StaticTextures.ThrowIfNotInitialized();

			#region Fill Center

			sbatch.Draw(
				StaticTextures.SinglePixel.Texture,
				bounds.AsInflated(CORNER_SIZE * scale, 0),
				StaticTextures.SinglePixel.Bounds,
				color);

			sbatch.Draw(
				StaticTextures.SinglePixel.Texture,
				bounds.AsInflated(0, CORNER_SIZE * scale),
				StaticTextures.SinglePixel.Bounds,
				color);

			#endregion

			#region Corners

			sbatch.Draw(
				StaticTextures.PanelCorner.Texture,
				bounds.VectorTopLeft(),
				StaticTextures.PanelCorner.Bounds,
				color,
				0 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				scale * StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelCorner.Texture,
				bounds.VectorTopRight(),
				StaticTextures.PanelCorner.Bounds,
				color,
				90 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				scale * StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelCorner.Texture,
				bounds.VectorBottomRight(),
				StaticTextures.PanelCorner.Bounds,
				color,
				180 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				scale * StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelCorner.Texture,
				bounds.VectorBottomLeft(),
				StaticTextures.PanelCorner.Bounds,
				color,
				270 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				scale * StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			#endregion
		}

		public static void DrawRoundedBlurPanelBackgroundPart(IBatchRenderer sbatch, Rectangle bounds, float scale = 1f)
		{
			StaticTextures.ThrowIfNotInitialized();

			var cornerSize = (int) (scale * CORNER_SIZE);

			#region Blur Edges

			sbatch.Draw(
				StaticTextures.PanelBlurEdge.Texture,
				new Rectangle(
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
				new Rectangle(
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
				new Rectangle(
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
				new Rectangle(
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
				bounds.VectorTopLeft() + scale * CORNER_VECTOR_TL,
				StaticTextures.PanelBlurCorner.Bounds,
				Color.White,
				0 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				scale * StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelBlurCorner.Texture,
				bounds.VectorTopRight() + scale * CORNER_VECTOR_TR,
				StaticTextures.PanelBlurCorner.Bounds,
				Color.White,
				90 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				scale * StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelBlurCorner.Texture,
				bounds.VectorBottomRight() + scale * CORNER_VECTOR_BR,
				StaticTextures.PanelBlurCorner.Bounds,
				Color.White,
				180 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				scale * StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelBlurCorner.Texture,
				bounds.VectorBottomLeft() + scale * CORNER_VECTOR_BL,
				StaticTextures.PanelBlurCorner.Bounds,
				Color.White,
				270 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				scale * StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			#endregion
		}

		public static void DrawRoundedRect(IBatchRenderer sbatch, Rectangle bounds, Color color, bool tl, bool tr, bool bl, bool br, float cornerScale = 1)
		{
			StaticTextures.ThrowIfNotInitialized();

			if (!tl && !tr && !bl && !br)
			{
				DrawSimpleRect(sbatch, bounds, color);

				return;
			}

			#region Fill Center

			sbatch.Draw(
				StaticTextures.SinglePixel.Texture,
				bounds.AsInflated(CORNER_SIZE * cornerScale, 0),
				StaticTextures.SinglePixel.Bounds,
				color);

			sbatch.Draw(
				StaticTextures.SinglePixel.Texture,
				bounds.AsInflated(0, CORNER_SIZE * cornerScale),
				StaticTextures.SinglePixel.Bounds,
				color);

			#endregion

			var cornerBounds = new Rectangle(0, 0, (int) (CORNER_SIZE * cornerScale) + 2, (int) (CORNER_SIZE * cornerScale) + 2);

			#region Corners

			if (tl)
			{
				sbatch.Draw(
					StaticTextures.PanelCorner.Texture,
					bounds.VectorTopLeft(),
					StaticTextures.PanelCorner.Bounds,
					color,
					0 * FloatMath.DegreesToRadians,
					Vector2.Zero,
					cornerScale * StaticTextures.DEFAULT_TEXTURE_SCALE,
					SpriteEffects.None, 0);
			}
			else
			{
				sbatch.Draw(
					StaticTextures.SinglePixel.Texture,
					cornerBounds.AsOffseted(bounds.VectorTopLeft()),
					StaticTextures.SinglePixel.Bounds,
					color,
					0 * FloatMath.DegreesToRadians,
					Vector2.Zero,
					SpriteEffects.None, 0);
			}

			if (tr)
			{
				sbatch.Draw(
					StaticTextures.PanelCorner.Texture,
					bounds.VectorTopRight(),
					StaticTextures.PanelCorner.Bounds,
					color,
					90 * FloatMath.DegreesToRadians,
					Vector2.Zero,
					cornerScale * StaticTextures.DEFAULT_TEXTURE_SCALE,
					SpriteEffects.None, 0);
			}
			else
			{
				sbatch.Draw(
					StaticTextures.SinglePixel.Texture,
					cornerBounds.AsOffseted(bounds.VectorTopRight()),
					StaticTextures.SinglePixel.Bounds,
					color,
					90 * FloatMath.DegreesToRadians,
					Vector2.Zero,
					SpriteEffects.None, 0);
			}

			if (br)
			{
				sbatch.Draw(
					StaticTextures.PanelCorner.Texture,
					bounds.VectorBottomRight(),
					StaticTextures.PanelCorner.Bounds,
					color,
					180 * FloatMath.DegreesToRadians,
					Vector2.Zero,
					cornerScale * StaticTextures.DEFAULT_TEXTURE_SCALE,
					SpriteEffects.None, 0);
			}
			else
			{
				sbatch.Draw(
					StaticTextures.SinglePixel.Texture,
					cornerBounds.AsOffseted(bounds.VectorBottomRight()),
					StaticTextures.SinglePixel.Bounds,
					color,
					180 * FloatMath.DegreesToRadians,
					Vector2.Zero,
					SpriteEffects.None, 0);
			}

			if (bl)
			{
				sbatch.Draw(
					StaticTextures.PanelCorner.Texture,
					bounds.VectorBottomLeft(),
					StaticTextures.PanelCorner.Bounds,
					color,
					270 * FloatMath.DegreesToRadians,
					Vector2.Zero,
					cornerScale * StaticTextures.DEFAULT_TEXTURE_SCALE,
					SpriteEffects.None, 0);
			}
			else
			{
				sbatch.Draw(
					StaticTextures.SinglePixel.Texture,
					cornerBounds.AsOffseted(bounds.VectorBottomLeft()),
					StaticTextures.SinglePixel.Bounds,
					color,
					270 * FloatMath.DegreesToRadians,
					Vector2.Zero,
					SpriteEffects.None, 0);
			}

			#endregion
		}

		public static void DrawSimpleRect(IBatchRenderer sbatch, Rectangle bounds, Color color)
		{
			StaticTextures.ThrowIfNotInitialized();

			sbatch.Draw(
				StaticTextures.SinglePixel.Texture,
				bounds,
				StaticTextures.SinglePixel.Bounds,
				color);
		}
	}
}