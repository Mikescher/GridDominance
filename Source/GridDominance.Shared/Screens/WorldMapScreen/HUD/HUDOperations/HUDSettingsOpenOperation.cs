using MonoSAMFramework.Portable.Screens.HUD.Operations;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class HUDSettingsOpenOperation : HUDTimedElementOperation<HUDWorldSettingsButton>
	{
		public HUDSettingsOpenOperation() : base(0.5f)
		{

		}

		protected override void OnStart(HUDWorldSettingsButton button)
		{
			button.SubButtons = new HUDWorldSubSettingButton[5];

			button.SubButtons[0] = new HUDButtonAbout(button);
			button.SubButtons[1] = new HUDButtonAccount(button);
			button.SubButtons[2] = new HUDButtonHighscore(button);
			button.SubButtons[3] = new HUDButtonVolume(button);
			button.SubButtons[4] = new HUDButtonEffects(button);

			button.HUD.AddElements(button.SubButtons);

			button.OpeningProgress = 0f;

			button.RotationSpeed = 1f;
		}

		protected override void OnProgress(HUDWorldSettingsButton button, float progress, InputState istate)
		{
			button.OpeningProgress = progress;

			foreach (var sub in button.SubButtons)
			{
				sub.OffsetProgress = FloatMath.FunctionEaseInOutCubic(progress);
			}

			button.RotationSpeed = 1f - progress*0.5f;
		}

		protected override void OnEnd(HUDWorldSettingsButton button)
		{
			button.OpeningProgress = 1f;
			button.RotationSpeed = 0.5f;
		}
	}
}
