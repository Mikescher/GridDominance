using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.RenderHelper;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Input
{
	public class HUDSimpleTextBox : HUDTextBox
	{
		public HUDBackgroundDefinition BackgroundNormal = HUDBackgroundDefinition.DUMMY;
		public HUDBackgroundDefinition BackgroundFocused = HUDBackgroundDefinition.DUMMY;

		public HUDSimpleTextBox(int depth = 0) : base(depth)
		{
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			if (IsFocused)
				HUDRenderHelper.DrawBackground(sbatch, bounds, BackgroundFocused);
			else
				HUDRenderHelper.DrawBackground(sbatch, bounds, BackgroundNormal);
			
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
