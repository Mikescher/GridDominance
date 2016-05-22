using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.HUD
{
	public abstract class HUDEllipseButton : HUDButton
	{
		protected Size OverrideEllipseSize = Size.Empty;

		protected override bool IsCursorOnButton(InputState istate)
		{
			var ellipseSize = Size;
			if (!OverrideEllipseSize.IsEmpty) ellipseSize = OverrideEllipseSize;


			var relativePoint = BoundingRectangle.Center - istate.PointerPosition;


			if (ellipseSize.Width == ellipseSize.Height)
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
		
		protected override void DrawDebugHUDBorders(SpriteBatch sbatch)
		{
			sbatch.DrawRectangle(BoundingRectangle, Color.Magenta);

			if (OverrideEllipseSize.IsEmpty)
			{
				sbatch.DrawEllipse(BoundingRectangle, 90, Color.Magenta, 2f);
			}
			else
			{
				sbatch.DrawEllipse(BoundingRectangle.Resize(OverrideEllipseSize), 90, Color.Magenta, 2f);
			}
		}
	}
}
