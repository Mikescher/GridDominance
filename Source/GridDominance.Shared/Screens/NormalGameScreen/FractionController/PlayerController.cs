using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.NormalGameScreen.FractionController
{
	class PlayerController : AbstractFractionController
	{
		private const float CROSSHAIR_CENTER_DISTANCE = 85;
		private const float CROSSHAIR_START_SCALE = 0.25f;

		private bool isMouseDragging = false;
		private FPoint dragOrigin;

		private readonly bool _dbc;
		private readonly bool _sbc;
		public override bool DoBarrelRecharge() => _dbc;
		public override bool SimulateBarrelRecharge() => _sbc;

		public PlayerController(GDGameScreen owner, Cannon cannon, Fraction fraction, bool doBarrelRecharge = true, bool simulateBarrelRecharge = false) 
			: base(0f, owner, cannon, fraction, false)
		{
			_dbc = doBarrelRecharge;
			_sbc = simulateBarrelRecharge;
		}

		public override void OnRemove()
		{
			Cannon.CrosshairSize.Set(0f);
		}

		protected override void OnExclusiveDown(InputState istate)
		{
			isMouseDragging = true;
			Cannon.CrosshairSize.SetForce(CROSSHAIR_START_SCALE);
			dragOrigin = istate.GamePointerPositionOnMap;
		}

		protected override void Calculate(InputState istate)
		{
			if (istate.IsRealJustDown)
			{
				// drag started
			}
			else if (!istate.IsRealDown && isMouseDragging)
			{
				isMouseDragging = false;
				Cannon.CrosshairSize.Set(0f);

				//SAMLog.Debug($"Cannon :: target({FloatMath.ToDegree(Cannon.Rotation.TargetValue):000}°)");
			}
			else if (isMouseDragging && istate.IsRealDown && !innerBoundings.Contains(istate.GamePointerPositionOnMap))
			{
				dragOrigin = Cannon.Position;
				Cannon.Rotation.Set(istate.GamePointerPositionOnMap.ToAngle(Cannon.Position));

				var dist = (istate.GamePointerPositionOnMap - Cannon.Position).Length();
				if (dist > 0)
				{
					var crosshairScale = FloatMath.Min(dist / CROSSHAIR_CENTER_DISTANCE, 1f);

					Cannon.CrosshairSize.Set(crosshairScale);
				}
			}
			else if (isMouseDragging && istate.IsRealDown && (istate.GamePointerPositionOnMap - dragOrigin).LengthSquared() > (GDConstants.TILE_WIDTH / 2f) * (GDConstants.TILE_WIDTH / 2f))
			{
				Cannon.Rotation.Set(FloatMath.PositiveAtan2(istate.GamePointerPositionOnMap.Y - dragOrigin.Y, istate.GamePointerPositionOnMap.X - dragOrigin.X));
			}
		}
	}
}
