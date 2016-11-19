using GridDominance.Shared.Screens.ScreenGame.Entities;
using MonoGame.Extended.Shapes;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using GridDominance.Shared.Screens.ScreenGame.Fractions;

namespace GridDominance.Shared.Screens.ScreenGame.FractionController
{
	class PlayerController : AbstractFractionController
	{
		private const float CROSSHAIR_CENTER_DISTANCE = 85;
		private const float CROSSHAIR_START_SCALE = 0.25f;

		private bool isMouseDragging = false;

		private readonly CircleF innerBoundings;

		public override bool DoBarrelRecharge() => true;

		public PlayerController(GDGameScreen owner, Cannon cannon, Fraction fraction) 
			: base(0f, owner, cannon, fraction)
		{
			innerBoundings = new CircleF(Cannon.Center, Cannon.Scale * Cannon.CANNON_OUTER_DIAMETER / 2);
		}

		protected override void Calculate(InputState istate)
		{
			if (istate.IsJustDown && innerBoundings.Contains(istate.PointerPosition))
			{
				isMouseDragging = true;
				Cannon.CrosshairSize.SetForce(CROSSHAIR_START_SCALE);
			}
			else if (!istate.IsDown && isMouseDragging)
			{
				isMouseDragging = false;
				Cannon.CrosshairSize.Set(0f);

				//Screen.PushNotification($"Cannon :: target({FloatMath.ToDegree(Cannon.Rotation.TargetValue):000}°)");
			}
			else if (isMouseDragging && istate.IsDown && !innerBoundings.Contains(istate.PointerPosition))
			{
				Cannon.Rotation.Set(FloatMath.PositiveAtan2(istate.PointerPosition.Y - Cannon.Center.Y, istate.PointerPosition.X - Cannon.Center.X));

				var dist = (istate.PointerPosition - Cannon.Center).Length();
				if (dist > 0)
				{
					var crosshairScale = FloatMath.Min(dist / CROSSHAIR_CENTER_DISTANCE, 1f);

					Cannon.CrosshairSize.Set(crosshairScale);
				}
			}
		}
	}
}
