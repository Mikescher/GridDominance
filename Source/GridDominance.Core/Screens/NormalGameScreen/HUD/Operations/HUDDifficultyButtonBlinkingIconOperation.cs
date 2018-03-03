using GridDominance.Shared.Screens.NormalGameScreen.HUD;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUDOperations
{
	public class HUDDifficultyButtonBlinkingIconOperation : SAMUpdateOp<HUDDifficultyButton>
	{
		private const float BLINK_LENGTH = 1.5f;
		
		protected override void OnUpdate(HUDDifficultyButton element, SAMTime gameTime, InputState istate)
		{
			element.ForegroundColor = ColorMath.Blend(FlatColors.SunFlower, FlatColors.Orange, FloatMath.PercSin(Lifetime / BLINK_LENGTH));
		}

		public override string Name => "BlinkingDifficultyButtonIcon";
	}
}
