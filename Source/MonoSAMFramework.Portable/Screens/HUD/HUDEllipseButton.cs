using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.MathHelper;
using MonoSAMFramework.Portable.MathHelper.FloatClasses;

namespace MonoSAMFramework.Portable.Screens.HUD
{
	public abstract class HUDEllipseButton : HUDButton
	{
		protected FSize OverrideEllipseSize = FSize.Empty;

		protected override bool IsCursorOnButton(InputState istate)
		{
			var ellipseSize = Size;
			if (!OverrideEllipseSize.IsEmpty) ellipseSize = OverrideEllipseSize;


			var relativePoint = BoundingRectangle.Center - istate.PointerPosition;


			if (ellipseSize.IsQuadratic)
			{
				return relativePoint.LengthSquared() <= (ellipseSize.Width / 2) * (ellipseSize.Width / 2);
			}
			else
			{
				// http://math.stackexchange.com/a/76463/126706

				var a = (relativePoint.X * relativePoint.X) / (ellipseSize.Width * ellipseSize.Width);
				var b = (relativePoint.Y * relativePoint.Y) / (ellipseSize.Height * ellipseSize.Height);

				return a + b <= 1;
			}
		}
		
		protected override void DrawDebugHUDBorders(IBatchRenderer sbatch)
		{
			sbatch.DrawRectangle(BoundingRectangle, Color.Magenta);

			if (OverrideEllipseSize.IsEmpty)
			{
				sbatch.DrawEllipse(BoundingRectangle, 90, Color.Magenta, 2f);
			}
			else
			{
				sbatch.DrawEllipse(BoundingRectangle.AsResized(OverrideEllipseSize), 90, Color.Magenta, 2f);
			}
		}
	}
}
