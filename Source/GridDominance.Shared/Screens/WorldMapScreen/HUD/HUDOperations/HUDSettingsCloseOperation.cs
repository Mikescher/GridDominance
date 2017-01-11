using MonoSAMFramework.Portable.Screens.HUD.Operations;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class HUDSettingsCloseOperation : HUDTimedElementOperation<HUDWorldSettingsButton>
	{
		public HUDSettingsCloseOperation() : base(0.25f)
		{
			
		}

		protected override void OnStart(HUDWorldSettingsButton button)
		{
			button.OpeningProgress = 1f;
			button.RotationSpeed = 0.5f;
		}

		protected override void OnProgress(HUDWorldSettingsButton button, float progress, InputState istate)
		{
			button.OpeningProgress = 1 - progress;

			foreach (var sub in button.SubButtons)
			{
				sub.ScaleProgress = 1 - progress;
				
				sub.FontProgress = 2*FloatMath.Max(0, 0.5f - progress);
			}

			button.RotationSpeed = 0.5f + progress * 0.5f;
		}

		protected override void OnEnd(HUDWorldSettingsButton button)
		{
			foreach (var sub in button.SubButtons)
			{
				sub.Alive = false;
			}

			button.SubButtons = null;
			button.OpeningProgress = 0f;
			button.RotationSpeed = 1f;
		}
	}
}
