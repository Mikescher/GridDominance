using GridDominance.Shared.Screens.NormalGameScreen.HUD;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUDOperations
{
	class HUDIncrementIndicatorLabelWiggleOperation : SAMUpdateOp<HUDIncrementIndicatorLabel>
	{
		private const float WIGGLE_SPEED = 2f;
		private const float WIGGLE_OFFSET = 2.5f;
		
		protected override void OnUpdate(HUDIncrementIndicatorLabel element, SAMTime gameTime, InputState istate)
		{
			element.AnimationOffset = WIGGLE_OFFSET * FloatMath.Sin(Lifetime * WIGGLE_SPEED * FloatMath.PI);
		}

		public override string Name => "IncrementIndicatorLabelWiggle";
	}
}
