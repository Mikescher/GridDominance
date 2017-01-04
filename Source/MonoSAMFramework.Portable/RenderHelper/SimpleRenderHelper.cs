using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

			var cornerBounds = new FRectangle(0, 0, TEX_CORNER_SIZE * cornerScale, TEX_CORNER_SIZE * cornerScale);

			#region Corners

			if (tl)
			{
				sbatch.DrawRaw(
					StaticTextures.PanelCorner.Texture,
					bounds.VectorTopLeft,
					StaticTextures.PanelCorner.Bounds,
					color,
					0 * FloatMath.DegreesToRadians,
					Vector2.Zero,
					cornerScale * StaticTextures.DEFAULT_TEXTURE_SCALE,
					SpriteEffects.None, 0);
			}
			else
			{
				sbatch.DrawRaw(
					StaticTextures.SinglePixel.Texture,
					cornerBounds.AsTranslated(bounds.VectorTopLeft),
					StaticTextures.SinglePixel.Bounds,
					color,
					0 * FloatMath.DegreesToRadians,
					Vector2.Zero,
					SpriteEffects.None, 0);
			}

			if (tr)
			{
				sbatch.DrawRaw(
					StaticTextures.PanelCorner.Texture,
					bounds.VectorTopRight,
					StaticTextures.PanelCorner.Bounds,
					color,
					90 * FloatMath.DegreesToRadians,
					Vector2.Zero,
					cornerScale * StaticTextures.DEFAULT_TEXTURE_SCALE,
					SpriteEffects.None, 0);
			}
			else
			{
				sbatch.DrawRaw(
					StaticTextures.SinglePixel.Texture,
					cornerBounds.AsTranslated(bounds.VectorTopRight),
					StaticTextures.SinglePixel.Bounds,
					color,
					90 * FloatMath.DegreesToRadians,
					Vector2.Zero,
					SpriteEffects.None, 0);
			}

			if (br)
			{
				sbatch.DrawRaw(
					StaticTextures.PanelCorner.Texture,
					bounds.VectorBottomRight,
					StaticTextures.PanelCorner.Bounds,
					color,
					180 * FloatMath.DegreesToRadians,
					Vector2.Zero,
					cornerScale * StaticTextures.DEFAULT_TEXTURE_SCALE,
					SpriteEffects.None, 0);
			}
			else
			{
				sbatch.DrawRaw(
					StaticTextures.SinglePixel.Texture,
					cornerBounds.AsTranslated(bounds.VectorBottomRight),
					StaticTextures.SinglePixel.Bounds,
					color,
					180 * FloatMath.DegreesToRadians,
					Vector2.Zero,
					SpriteEffects.None, 0);
			}

			if (bl)
			{
				sbatch.DrawRaw(
					StaticTextures.PanelCorner.Texture,
					bounds.VectorBottomLeft,
					StaticTextures.PanelCorner.Bounds,
					color,
					270 * FloatMath.DegreesToRadians,
					Vector2.Zero,
					cornerScale * StaticTextures.DEFAULT_TEXTURE_SCALE,
					SpriteEffects.None, 0);
			}
			else
			{
				sbatch.DrawRaw(
					StaticTextures.SinglePixel.Texture,
					cornerBounds.AsTranslated(bounds.VectorBottomLeft),
					StaticTextures.SinglePixel.Bounds,
					color,
					270 * FloatMath.DegreesToRadians,
					Vector2.Zero,
					SpriteEffects.None, 0);
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
				new FRectangle(bounds.Left, FloatMath.Round(bounds.Top), bounds.Width, borderSize),
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
