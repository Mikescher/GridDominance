using GridDominance.Shared.Screens.GameScreen.Entities;
using GridDominance.Shared.Framework;
using MonoGame.Extended.Shapes;

namespace GridDominance.Shared.Screens.GameScreen.FractionController
{
	// ReSharper disable ImpureMethodCallOnReadonlyValueField
	class PlayerController : AbstractFractionController
	{
		private bool isMouseDragging = false;

		private readonly CircleF innerBoundings;

		public PlayerController(GameScreen owner, Cannon cannon, Fraction fraction) 
			: base(0f, owner, cannon, fraction)
		{
			innerBoundings = new CircleF(Cannon.Center, Cannon.Scale * Cannon.CANNON_DIAMETER / 2);
		}

		protected override void Calculate(InputState istate)
		{
			if (istate.IsJustDown && innerBoundings.Contains(istate.PointerPosition))
			{
				isMouseDragging = true;
			}
			else if (!istate.IsDown && isMouseDragging)
			{
				isMouseDragging = false;

				Owner.PushNotification($"Cannon :: target({FloatMath.ToDegree(Cannon.Rotation.TargetValue):000}°)");
			}
			else if (isMouseDragging && istate.IsDown && !innerBoundings.Contains(istate.PointerPosition))
			{
				Cannon.Rotation.Set(FloatMath.PositiveAtan2(istate.PointerPosition.Y - Cannon.Center.Y, istate.PointerPosition.X - Cannon.Center.X));
			}
		}
	}
}
