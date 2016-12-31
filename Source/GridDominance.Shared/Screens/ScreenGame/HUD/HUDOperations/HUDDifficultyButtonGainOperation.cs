using GridDominance.Shared.Screens.ScreenGame.HUD;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens.HUD.Operations;

namespace GridDominance.Shared.Screens.ScreenGame.HUDOperations
{
	public class HUDDifficultyButtonGainOperation : HUDTimedElementOperation<HUDDifficultyButton>
	{
		public HUDDifficultyButtonGainOperation() : base(3f)
		{
		}

		protected override void OnStart(HUDDifficultyButton element)
		{
			//
		}

		protected override void OnEnd(HUDDifficultyButton element)
		{
			//
		}

		protected override void OnProgress(HUDDifficultyButton element, float progress, InputState istate)
		{
			element.BackgroundColor = ColorMath.Blend(FlatColors.ButtonHUD, FlatColors.BackgroundHUD2, progress);
			element.IconScale = FloatMath.FunctionEaseOutElastic(progress);
		}
	}
}
