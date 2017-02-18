using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.RenderHelper;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Input
{
	public class HUDIconTextBox : HUDTextBox
	{
		public Color ColorPadLeft = Color.LightGray;
		public float WidthPadLeft = 0;

		public Color IconColor = Color.Transparent;
		public TextureRegion2D Icon = null;
		public FSize IconSize = FSize.Empty;

		public float BackgoundCornerSize = 16f;

		public HUDIconTextBox(int depth) : base(depth)
		{
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			var bg = IsFocused ? ColorBackground : ColorFocused;

			SimpleRenderHelper.DrawRoundedRect(sbatch, bounds, bg, BackgoundCornerSize);
			
			if (Text == "" && !IsFocused)
			{
				DrawPlaceholder(sbatch, bounds, Padding + WidthPadLeft, Padding);
			}
			else
			{
				DrawText(sbatch, bounds, Padding + WidthPadLeft, Padding);
			}

			SimpleRenderHelper.DrawRoundedRect(sbatch, bounds.ToSubRectangleWest(WidthPadLeft), ColorPadLeft, true, false, true, false, BackgoundCornerSize);

			if (Icon != null && !IconSize.IsEmpty)
			{
				//TODO NOT CENTERED !!!!!!!!!!!!!!
				sbatch.DrawCentered(Icon, new Vector2(bounds.Top + bounds.Height/2, bounds.Left + WidthPadLeft/2), IconSize.Width, IconSize.Height, IconColor);
			}
		}
	}
}
