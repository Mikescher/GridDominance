using GridDominance.Shared.Screens.ScreenGame.HUD;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens.HUD.Operations;

namespace GridDominance.Shared.Screens.ScreenGame.HUDOperations
{
	class HUDIncrementIndicatorLabelWiggleOperation : HUDInfiniteElementOperation<HUDIncrementIndicatorLabel>
	{
		private const float WIGGLE_SPEED = 2f;
		private const float WIGGLE_OFFSET = 2.5f;

		protected override void OnStart(HUDIncrementIndicatorLabel element)
		{
			// NOP
		}

		protected override void OnEnd(HUDIncrementIndicatorLabel element)
		{
			// NOP
		}

		protected override void OnProgress(HUDIncrementIndicatorLabel element, float lifetime, InputState istate)
		{
			element.AnimationOffset = WIGGLE_OFFSET * FloatMath.Sin(lifetime * WIGGLE_SPEED * FloatMath.PI);
		}
	}
}
