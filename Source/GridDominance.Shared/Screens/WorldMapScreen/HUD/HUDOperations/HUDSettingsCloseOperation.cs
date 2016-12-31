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
			button.openingProgress = 1f;
			button.rotationSpeed = 0.5f;
		}

		protected override void OnProgress(HUDWorldSettingsButton button, float progress, InputState istate)
		{
			button.openingProgress = 1 - progress;

			foreach (var sub in button.subButtons)
			{
				sub.scaleProgress = 1 - progress;
				
				sub.fontProgress = 2*FloatMath.Max(0, 0.5f - progress);
			}

			button.rotationSpeed = 0.5f + progress * 0.5f;
		}

		protected override void OnEnd(HUDWorldSettingsButton button)
		{
			foreach (var sub in button.subButtons)
			{
				sub.Alive = false;
			}

			button.subButtons = null;
			button.openingProgress = 0f;
			button.rotationSpeed = 1f;
		}
	}
}
