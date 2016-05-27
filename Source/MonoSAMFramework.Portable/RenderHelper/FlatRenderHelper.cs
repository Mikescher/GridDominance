using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.MathHelper;
using MonoSAMFramework.Portable.MathHelper.FloatClasses;

namespace MonoSAMFramework.Portable.RenderHelper
{
	public static class FlatRenderHelper
	{
		public const int CORNER_SIZE = SAMRenderHelper.CORNER_SIZE;

		public static readonly Vector2 CORNER_VECTOR_TL = new Vector2(-CORNER_SIZE, -CORNER_SIZE);
		public static readonly Vector2 CORNER_VECTOR_TR = new Vector2(+CORNER_SIZE, -CORNER_SIZE);
		public static readonly Vector2 CORNER_VECTOR_BL = new Vector2(-CORNER_SIZE, +CORNER_SIZE);
		public static readonly Vector2 CORNER_VECTOR_BR = new Vector2(+CORNER_SIZE, +CORNER_SIZE);

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

			var cornerSize = (int) (scale * CORNER_SIZE);

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
	}
}