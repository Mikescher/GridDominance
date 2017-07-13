using GridDominance.Shared.Screens.NormalGameScreen.HUD;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens.HUD.Operations;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUDOperations
{
	public class HUDMultiplayerDifficultyButtonBlinkingIconOperation : HUDInfiniteElementOperation<HUDMultiplayerDifficultyButton>
	{
		private const float BLINK_LENGTH = 1.5f;
		
		protected override void OnStart(HUDMultiplayerDifficultyButton element)
		{
			// NOP
		}

		protected override void OnEnd(HUDMultiplayerDifficultyButton element)
		{
			// NOP
		}

		protected override void OnProgress(HUDMultiplayerDifficultyButton element, float value, InputState istate)
		{
			element.ForegroundColor = ColorMath.Blend(FlatColors.SunFlower, FlatColors.Orange, FloatMath.Sin(value / BLINK_LENGTH) / 2f + 0.5f);
		}

		public override string Name => "BlinkingDifficultyButtonIcon";
	}
}
