using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.RenderHelper
{
	public static class FlatRenderHelper
	{
		public static void DrawRoundedBlurPanel(IBatchRenderer sbatch, FRectangle bounds, Color color, float cornerSize = 16f)
		{
			StaticTextures.ThrowIfNotInitialized();

			DrawRoundedBlurPanelBackgroundPart(sbatch, bounds, cornerSize);
			SimpleRenderHelper.DrawRoundedRect(sbatch, bounds, color, cornerSize);
		}

		public static void DrawRoundedAlphaBlurPanel(IBatchRenderer sbatch, FRectangle bounds, Color color, float cornerSize, float alpha)
		{
			StaticTextures.ThrowIfNotInitialized();

			DrawRoundedBlurPanelBackgroundPart(sbatch, bounds, cornerSize, alpha);
			SimpleRenderHelper.DrawRoundedRect(sbatch, bounds, color * alpha, cornerSize);
		}

		public static void DrawSimpleBlurPanel(IBatchRenderer sbatch, FRectangle bounds, Color color, float cornerSize = 16f)
		{
			StaticTextures.ThrowIfNotInitialized();

			DrawRoundedBlurPanelBackgroundPart(sbatch, bounds, cornerSize);
			SimpleRenderHelper.DrawSimpleRect(sbatch, bounds, color);
		}

		public static void DrawSimpleAlphaBlurPanel(IBatchRenderer sbatch, FRectangle bounds, Color color, float cornerSize, float alpha)
		{
			StaticTextures.ThrowIfNotInitialized();

			DrawRoundedBlurPanelBackgroundPart(sbatch, bounds, cornerSize, alpha);
			SimpleRenderHelper.DrawSimpleRect(sbatch, bounds, color * alpha);
		}

		public static void DrawRoundedBlurPanelBackgroundPart(IBatchRenderer sbatch, FRectangle bounds, float cornerSize = 16f)
		{
			StaticTextures.ThrowIfNotInitialized();
			
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

		public static void DrawRoundedBlurPanelBackgroundPart(IBatchRenderer sbatch, FRectangle bounds, float cornerSize = 16f, float alpha = 1f)
		{
			StaticTextures.ThrowIfNotInitialized();

			var alphaWhite = Color.White * alpha;
			var alphaBlack = Color.Black * alpha;

			var r_tl = new FRectangle(bounds.Left - cornerSize, bounds.Top - cornerSize, 2 * cornerSize, 2 * cornerSize);
			var r_tr = new FRectangle(bounds.Right - cornerSize, bounds.Top - cornerSize, 2 * cornerSize, 2 * cornerSize);
			var r_br = new FRectangle(bounds.Right - cornerSize, bounds.Bottom - cornerSize, 2 * cornerSize, 2 * cornerSize);
			var r_bl = new FRectangle(bounds.Left - cornerSize, bounds.Bottom - cornerSize, 2 * cornerSize, 2 * cornerSize);

			var r_l = new FRectangle(r_tl.Left, r_tl.Bottom, r_tl.Width, r_bl.Top - r_tl.Bottom);
			var r_t = new FRectangle(r_tl.Right, r_tl.Top, r_tr.Left - r_tl.Right, r_tl.Height);
			var r_r = new FRectangle(r_tr.Left, r_tr.Bottom, r_tr.Width, r_br.Top - r_tr.Bottom);
			var r_b = new FRectangle(r_bl.Right, r_bl.Top, r_br.Left - r_bl.Right, r_bl.Height);

			// Top
			sbatch.DrawRot000(StaticTextures.PanelBlurEdge, r_t, alphaWhite, 0);

			// Right
			sbatch.DrawRot090(StaticTextures.PanelBlurEdge, r_r, alphaWhite, 0);

			// Bottom
			sbatch.DrawRot180(StaticTextures.PanelBlurEdge, r_b, alphaWhite, 0);

			// Left
			sbatch.DrawRot270(StaticTextures.PanelBlurEdge, r_l, alphaWhite, 0);

			// TL
			sbatch.DrawRot000(StaticTextures.PanelBlurCorner, r_tl, alphaWhite, 0);

			// TR
			sbatch.DrawRot090(StaticTextures.PanelBlurCorner, r_tr, alphaWhite, 0);

			// BR
			sbatch.DrawRot180(StaticTextures.PanelBlurCorner, r_br, alphaWhite, 0);

			// BL
			sbatch.DrawRot270(StaticTextures.PanelBlurCorner, r_bl, alphaWhite, 0);

			sbatch.FillRectangle(r_tl.BottomRight, new FSize(bounds.Width - 2 * cornerSize, bounds.Height - 2 * cornerSize), alphaBlack);
		}

		public static void DrawDropShadow(IBatchRenderer sbatch, FRectangle bounds, float sOutset, float sInset)
		{
			StaticTextures.ThrowIfNotInitialized();

			var r_tl = new FRectangle(bounds.Left  - sOutset, bounds.Top    - sOutset, sOutset + sInset, sOutset + sInset);
			var r_tr = new FRectangle(bounds.Right - sInset,  bounds.Top    - sOutset, sOutset + sInset, sOutset + sInset);
			var r_br = new FRectangle(bounds.Right - sInset,  bounds.Bottom - sInset,  sOutset + sInset, sOutset + sInset);
			var r_bl = new FRectangle(bounds.Left  - sOutset, bounds.Bottom - sInset,  sOutset + sInset, sOutset + sInset);

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

		public static void DrawOutlinesBlurRectangle(IBatchRenderer sbatch, FRectangle bounds, float borderWidth, Color cInner, Color cBorder, float blurOuterWidth, float blurInset)
		{
			StaticTextures.ThrowIfNotInitialized();

			DrawDropShadow(sbatch, bounds, blurOuterWidth, blurInset);

			SimpleRenderHelper.DrawSimpleRect(sbatch, bounds.AsDeflated(borderWidth / 2f, borderWidth / 2f), cInner);
			
			SimpleRenderHelper.DrawSimpleRectOutline(sbatch, bounds, borderWidth, cBorder);
		}
	}
}