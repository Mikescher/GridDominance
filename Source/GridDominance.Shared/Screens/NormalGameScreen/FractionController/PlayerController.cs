using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.NormalGameScreen.FractionController
{
	class PlayerController : AbstractFractionController
	{
		private const float CROSSHAIR_CENTER_DISTANCE = 85;
		private const float CROSSHAIR_START_SCALE = 0.25f;

		private bool isMouseDragging = false;
		private FPoint dragOrigin;

		private readonly FCircle innerBoundings;

		public override bool DoBarrelRecharge() => true;

		public PlayerController(GDGameScreen owner, Cannon cannon, Fraction fraction) 
			: base(0f, owner, cannon, fraction)
		{
			innerBoundings = new FCircle(Cannon.Position, Cannon.Scale * Cannon.CANNON_OUTER_DIAMETER / 2);
		}

		protected override void Calculate(InputState istate)
		{
			if (istate.IsExclusiveJustDown && innerBoundings.Contains(istate.GamePointerPosition))
			{
				istate.Swallow(InputConsumer.GameEntity);

				isMouseDragging = true;
				Cannon.CrosshairSize.SetForce(CROSSHAIR_START_SCALE);
				dragOrigin = istate.GamePointerPosition;
			}
			else if (!istate.IsRealDown && isMouseDragging)
			{
				isMouseDragging = false;
				Cannon.CrosshairSize.Set(0f);

				//Screen.PushNotification($"Cannon :: target({FloatMath.ToDegree(Cannon.Rotation.TargetValue):000}°)");
			}
			else if (isMouseDragging && istate.IsRealDown && !innerBoundings.Contains(istate.GamePointerPosition))
			{
				dragOrigin = Cannon.Position.ToFPoint();
				Cannon.Rotation.Set(FloatMath.PositiveAtan2(istate.GamePointerPosition.Y - Cannon.Position.Y, istate.GamePointerPosition.X - Cannon.Position.X));

				var dist = (istate.GamePointerPosition - Cannon.Position).Length();
				if (dist > 0)
				{
					var crosshairScale = FloatMath.Min(dist / CROSSHAIR_CENTER_DISTANCE, 1f);

					Cannon.CrosshairSize.Set(crosshairScale);
				}
			}
			else if (isMouseDragging && istate.IsRealDown && (istate.GamePointerPosition - dragOrigin).LengthSquared() > (GDConstants.TILE_WIDTH / 2f) * (GDConstants.TILE_WIDTH / 2f))
			{
				Cannon.Rotation.Set(FloatMath.PositiveAtan2(istate.GamePointerPosition.Y - dragOrigin.Y, istate.GamePointerPosition.X - dragOrigin.X));
			}
		}
	}
}
