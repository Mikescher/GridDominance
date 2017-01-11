using MonoSAMFramework.Portable.Screens.HUD.Operations;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class HUDSettingsFontAppearOperation : HUDTimedElementOperation<HUDWorldSettingsButton>
	{
		private readonly int index;

		public HUDSettingsFontAppearOperation(int idx) : base(0.05f)
		{
			index = idx;
		}

		protected override void OnStart(HUDWorldSettingsButton button)
		{
			if (button.SubButtons == null) return;

			button.SubButtons[index].FontProgress = 0f;
		}

		protected override void OnProgress(HUDWorldSettingsButton button, float progress, InputState istate)
		{
			if (button.SubButtons == null) return;

			button.SubButtons[index].FontProgress = progress;
		}

		protected override void OnEnd(HUDWorldSettingsButton button)
		{
			if (button.SubButtons == null) return;

			button.SubButtons[index].FontProgress = 1f;
		}
	}
}
