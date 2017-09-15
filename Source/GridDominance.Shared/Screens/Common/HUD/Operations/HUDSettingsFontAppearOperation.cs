using MonoSAMFramework.Portable.Screens.HUD.Operations;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class HUDSettingsFontAppearOperation : HUDTimedElementOperation<SettingsButton>
	{
		private readonly int index;

		public HUDSettingsFontAppearOperation(int idx) : base(0.05f)
		{
			index = idx;
		}

		protected override void OnStart(SettingsButton button)
		{
			if (button.SubButtons == null) return;

			button.SubButtons[index].FontProgress = 0f;
		}

		protected override void OnProgress(SettingsButton button, float progress, InputState istate)
		{
			if (button.SubButtons == null) return;

			button.SubButtons[index].FontProgress = progress;
		}

		protected override void OnEnd(SettingsButton button)
		{
			if (button.SubButtons == null) return;

			button.SubButtons[index].FontProgress = 1f;
		}

		public override string Name => "SettingsFontAppear";
	}
}
