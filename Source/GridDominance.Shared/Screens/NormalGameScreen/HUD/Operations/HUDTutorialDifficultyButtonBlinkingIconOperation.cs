using GridDominance.Shared.Screens.NormalGameScreen.HUD;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens.HUD.Operations;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUDOperations
{
	public class HUDTutorialDifficultyButtonBlinkingIconOperation : HUDInfiniteElementOperation<HUDTutorialDifficultyButton>
	{
		private const float BLINK_LENGTH = 1.5f;
		
		protected override void OnStart(HUDTutorialDifficultyButton element)
		{
			// NOP
		}

		protected override void OnEnd(HUDTutorialDifficultyButton element)
		{
			// NOP
		}

		protected override void OnProgress(HUDTutorialDifficultyButton element, float value, InputState istate)
		{
			element.ForegroundColor = ColorMath.Blend(FlatColors.SunFlower, FlatColors.Orange, FloatMath.Sin(value / BLINK_LENGTH) / 2f + 0.5f);
		}

		public override string Name => "BlinkingTutorialDifficultyButtonIcon";
	}
}
