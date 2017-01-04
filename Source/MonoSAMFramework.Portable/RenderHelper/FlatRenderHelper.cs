using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
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

			var ccornerSize = scale * CROP_CORNER_SIZE;
			var tcornerSize = scale * TEX_CORNER_SIZE;

			#region Fill Center

			sbatch.DrawStretched(
				StaticTextures.SinglePixel,
				bounds.AsDeflated(ccornerSize, 0),
				color);

			sbatch.DrawStretched(
				StaticTextures.SinglePixel,
				bounds.AsDeflated(0, ccornerSize),
				color);

			#endregion

			#region Corners

			// Don't ask my why I need the +0.2f
			var r_tl = new FRectangle(0.2f + bounds.Left,                bounds.Top                 , tcornerSize, tcornerSize);
			var r_tr = new FRectangle(0.0f + bounds.Right - tcornerSize, bounds.Top                 , tcornerSize, tcornerSize);
			var r_br = new FRectangle(0.0f + bounds.Right - tcornerSize, bounds.Bottom - tcornerSize, tcornerSize, tcornerSize);
			var r_bl = new FRectangle(0.2f + bounds.Left,                bounds.Bottom - tcornerSize, tcornerSize, tcornerSize);

			sbatch.DrawRot000(StaticTextures.PanelCorner, r_tl, color, 0);
			sbatch.DrawRot090(StaticTextures.PanelCorner, r_tr, color, 0);
			sbatch.DrawRot180(StaticTextures.PanelCorner, r_br, color, 0);
			sbatch.DrawRot270(StaticTextures.PanelCorner, r_bl, color, 0);

			#endregion
		}

		public static void DrawRoundedBlurPanelBackgroundPart(IBatchRenderer sbatch, FRectangle bounds, float scale = 1f)
		{
			StaticTextures.ThrowIfNotInitialized();

			var cornerSize = scale * CROP_CORNER_SIZE;

			var r_tl = new FRectangle(bounds.Left  - cornerSize, bounds.Top    - cornerSize, 2 * cornerSize, 2 * cornerSize);
			var r_tr = new FRectangle(bounds.Right - cornerSize, bounds.Top    - cornerSize, 2 * cornerSize, 2 * cornerSize);
			var r_br = new FRectangle(bounds.Right - cornerSize, bounds.Bottom - cornerSize, 2 * cornerSize, 2 * cornerSize);
			var r_bl = new FRectangle(bounds.Left  - cornerSize, bounds.Bottom - cornerSize, 2 * cornerSize, 2 * cornerSize);

			var r_l = new FRectangle(r_tl.Left,  r_tl.Bottom, r_tl.Width,             r_bl.Top - r_tl.Bottom);
			var r_t = new FRectangle(r_tl.Right, r_tl.Top,    r_tr.Left - r_tl.Right, r_tl.Height);
			var r_r = new FRectangle(r_tr.Left,  r_tr.Bottom, r_tr.Width,             r_br.Top - r_tr.Bottom);
			var r_b = new FRectangle(r_bl.Right, r_bl.Top,    r_br.Left - r_bl.Right, r_bl.Height);

			// Top
			sbatch.DrawRot000(StaticTextures.PanelBlurEdge, r_t, Color.White, 0);

			// Right
			sbatch.DrawRot090(StaticTextures.PanelBlurEdge, r_r, Color.White, 0);

			// Bottom
			sbatch.DrawRot180(StaticTextures.PanelBlurEdge, r_b, Color.White, 0);

			// Left
			sbatch.DrawRot270(StaticTextures.PanelBlurEdge, r_l, Color.White, 0);

			// TL
			sbatch.DrawRot000(StaticTextures.PanelBlurCorner, r_tl, Color.White, 0);

			// TR
			sbatch.DrawRot090(StaticTextures.PanelBlurCorner, r_tr, Color.White, 0);

			// BR
			sbatch.DrawRot180(StaticTextures.PanelBlurCorner, r_br, Color.White, 0);

			// BL
			sbatch.DrawRot270(StaticTextures.PanelBlurCorner, r_bl, Color.White, 0);
		}
		
		public static void DrawDropShadow(IBatchRenderer sbatch, FRectangle bounds, float sOutset, float sInset)
		{
			StaticTextures.ThrowIfNotInitialized();

			var r_tl = new FRectangle(
				bounds.Left - sOutset,
				bounds.Top - sOutset,
				sOutset + sInset,
				sOutset + sInset);

			var r_tr = new FRectangle(
				bounds.Right - sInset,
				bounds.Top - sOutset,
				sOutset + sInset,
				sOutset + sInset);

			var r_br = new FRectangle(
				bounds.Right - sInset,
				bounds.Bottom - sInset,
				sOutset + sInset,
				sOutset + sInset);

			var r_bl = new FRectangle(
				bounds.Left - sOutset,
				bounds.Bottom - sInset,
				sOutset + sInset,
				sOutset + sInset);

			var r_l = new FRectangle(r_tl.Left, r_tl.Bottom, r_tl.Width, r_bl.Top - r_tl.Bottom);

			var r_t = new FRectangle(r_tl.Right, r_tl.Top, r_tr.Left - r_tl.Right, r_tl.Height);

			var r_r = new FRectangle(r_tr.Left, r_tr.Bottom, r_tr.Width, r_br.Top - r_tr.Bottom);

			var r_b = new FRectangle(r_bl.Right, r_bl.Top, r_br.Left - r_bl.Right, r_bl.Height);
			
			// Top
			sbatch.DrawRot000(StaticTextures.PanelBlurEdge, r_t, Color.White, 0);

			// Right
			sbatch.DrawRot090(StaticTextures.PanelBlurEdge, r_r, Color.White, 0);

			// Bottom
			sbatch.DrawRot180(StaticTextures.PanelBlurEdge, r_b, Color.White, 0);

			// Left
			sbatch.DrawRot270(StaticTextures.PanelBlurEdge, r_l, Color.White, 0);
			
			// TL
			sbatch.DrawRot000(StaticTextures.PanelBlurCorner, r_tl, Color.White, 0);

			// TR
			sbatch.DrawRot090(StaticTextures.PanelBlurCorner, r_tr, Color.White, 0);

			// BR
			sbatch.DrawRot180(StaticTextures.PanelBlurCorner, r_br, Color.White, 0);

			// BL
			sbatch.DrawRot270(StaticTextures.PanelBlurCorner, r_bl, Color.White, 0);
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