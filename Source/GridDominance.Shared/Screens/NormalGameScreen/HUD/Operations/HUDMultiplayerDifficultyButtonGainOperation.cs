using GridDominance.Shared.Screens.NormalGameScreen.HUD;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens.HUD.Operations;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUDOperations
{
	public class HUDMultiplayerDifficultyButtonGainOperation : HUDTimedElementOperation<HUDMultiplayerDifficultyButton>
	{
		public HUDMultiplayerDifficultyButtonGainOperation() : base(3f)
		{
		}

		protected override void OnStart(HUDMultiplayerDifficultyButton element)
		{
			//
		}

		protected override void OnEnd(HUDMultiplayerDifficultyButton element)
		{
			//
		}

		protected override void OnProgress(HUDMultiplayerDifficultyButton element, float progress, InputState istate)
		{
			element.BackgroundColor = ColorMath.Blend(FlatColors.ButtonHUD, FlatColors.BackgroundHUD2, progress);
			element.IconScale = FloatMath.FunctionEaseOutElastic(progress);
		}

		public override string Name => "DifficultyButtonGain";
	}
}
