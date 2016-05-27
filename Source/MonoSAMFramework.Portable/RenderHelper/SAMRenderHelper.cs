using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.MathHelper;

namespace MonoSAMFramework.Portable.RenderHelper
{
	public static class SAMRenderHelper
	{
		public const int CORNER_SIZE = 16;

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

			var cornerBounds = new Rectangle(0, 0, (int)(CORNER_SIZE * cornerScale) + 2, (int)(CORNER_SIZE * cornerScale) + 2);

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
