using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

		public static void DrawRoundedBlurPanel(SpriteBatch sbatch, Rectangle bounds, Color color)
		{
			StaticTextures.ThrowIfNotInitialized();

			#region Blur Edges

			sbatch.Draw(
				StaticTextures.PanelBlurEdge.Texture,
				new Rectangle(
					bounds.Left + CORNER_SIZE,
					bounds.Top - CORNER_SIZE,
					bounds.Width - 2 * CORNER_SIZE,
					2 * CORNER_SIZE),
				StaticTextures.PanelBlurEdge.Bounds,
				Color.White,
				0 * FloatMath.DegreesToRadians,
				Vector2.Zero, SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelBlurEdge.Texture,
				new Rectangle(
					bounds.Right - CORNER_SIZE + (2 * CORNER_SIZE),
					bounds.Top + CORNER_SIZE,
					bounds.Height - 2 * CORNER_SIZE,
					2 * CORNER_SIZE
					),
				StaticTextures.PanelBlurEdge.Bounds,
				Color.White,
				90 * FloatMath.DegreesToRadians,
				Vector2.Zero, SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelBlurEdge.Texture,
				new Rectangle(
					bounds.Left + CORNER_SIZE + (bounds.Width - 2 * CORNER_SIZE), 
					bounds.Bottom - CORNER_SIZE + (2 * CORNER_SIZE), 
					bounds.Width - 2 * CORNER_SIZE, 
					2 * CORNER_SIZE),
				StaticTextures.PanelBlurEdge.Bounds,
				Color.White,
				180 * FloatMath.DegreesToRadians,
				Vector2.Zero, SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelBlurEdge.Texture,
				new Rectangle(
					bounds.Left - CORNER_SIZE,
					bounds.Top + CORNER_SIZE + (bounds.Height - 2 * CORNER_SIZE),
					bounds.Height - 2 * CORNER_SIZE,
					2 * CORNER_SIZE),
				StaticTextures.PanelBlurEdge.Bounds,
				Color.White,
				270 * FloatMath.DegreesToRadians,
				Vector2.Zero, SpriteEffects.None, 0);

			#endregion

			#region Blur Corners

			sbatch.Draw(
				StaticTextures.PanelBlurCorner.Texture,
				bounds.VectorTopLeft() + CORNER_VECTOR_TL,
				StaticTextures.PanelBlurCorner.Bounds,
				Color.White,
				0 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelBlurCorner.Texture,
				bounds.VectorTopRight() + CORNER_VECTOR_TR,
				StaticTextures.PanelBlurCorner.Bounds,
				Color.White,
				90 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelBlurCorner.Texture,
				bounds.VectorBottomRight() + CORNER_VECTOR_BR,
				StaticTextures.PanelBlurCorner.Bounds,
				Color.White,
				180 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelBlurCorner.Texture,
				bounds.VectorBottomLeft() + CORNER_VECTOR_BL,
				StaticTextures.PanelBlurCorner.Bounds,
				Color.White,
				270 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			#endregion

			#region Fill Center

			sbatch.Draw(
				StaticTextures.SinglePixel.Texture, 
				bounds.AsInflated(CORNER_SIZE, 0), 
				StaticTextures.SinglePixel.Bounds, 
				color);

			sbatch.Draw(
				StaticTextures.SinglePixel.Texture,
				bounds.AsInflated(0, CORNER_SIZE),
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
				StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelCorner.Texture,
				bounds.VectorTopRight(),
				StaticTextures.PanelCorner.Bounds,
				color,
				90 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelCorner.Texture,
				bounds.VectorBottomRight(),
				StaticTextures.PanelCorner.Bounds,
				color,
				180 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			sbatch.Draw(
				StaticTextures.PanelCorner.Texture,
				bounds.VectorBottomLeft(),
				StaticTextures.PanelCorner.Bounds,
				color,
				270 * FloatMath.DegreesToRadians,
				Vector2.Zero,
				StaticTextures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None, 0);

			#endregion
		}
	}
}
