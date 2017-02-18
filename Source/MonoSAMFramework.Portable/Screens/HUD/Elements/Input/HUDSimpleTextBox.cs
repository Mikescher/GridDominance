using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Input
{
	public class HUDSimpleTextBox : HUDTextBox
	{
		public Color ColorBorder = Color.Black;
		public int BorderThickness = 2;

		public float BackgoundCornerSize = 16f;
		public HUDBackgroundType BackgoundType = HUDBackgroundType.Simple;

		public HUDSimpleTextBox(int depth) : base(depth)
		{
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			var bg = IsFocused ? ColorBackground : ColorFocused;

			SimpleRenderHelper.DrawHUDBackground(sbatch, BackgoundType, bounds, bg, BackgoundCornerSize);

			if (BackgoundType == HUDBackgroundType.Simple && ColorBorder != Color.Transparent && BorderThickness > 0)
			{
				SimpleRenderHelper.DrawSimpleRectOutline(sbatch, bounds, BorderThickness, ColorBorder);
			}

			if (Text == "" && !IsFocused)
			{
				DrawPlaceholder(sbatch, bounds, Padding, Padding);
			}
			else
			{
				DrawText(sbatch, bounds, Padding, Padding);
			}
		}
	}
}
