using GridDominance.Shared.Screens.ScreenGame.HUD;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens.HUD.Operations;

namespace GridDominance.Shared.Screens.ScreenGame.HUDOperations
{
	public class HUDBlinkingDifficultyButtonIconOperation : HUDInfiniteElementOperation<HUDDifficultyButton>
	{
		private const float BLINK_LENGTH = 1.5f;
		
		protected override void OnStart(HUDDifficultyButton element)
		{
			// NOP
		}

		protected override void OnEnd(HUDDifficultyButton element)
		{
			// NOP
		}

		protected override void OnProgress(HUDDifficultyButton element, float value, InputState istate)
		{
			element.ForegroundColor = ColorMath.Blend(FlatColors.SunFlower, FlatColors.Orange, FloatMath.Sin(value / BLINK_LENGTH) / 2f + 0.5f);
		}

		public override string Name => "BlinkingDifficultyButtonIcon";
	}
}
