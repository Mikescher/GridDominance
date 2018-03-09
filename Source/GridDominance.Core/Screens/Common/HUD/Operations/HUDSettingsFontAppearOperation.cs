using GridDominance.Shared.Screens.Common.HUD.Elements;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.Common.HUD.Operations
{
	class HUDSettingsFontAppearOperation : FixTimeOperation<SettingsButton>
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

		protected override void OnProgress(SettingsButton button, float progress, SAMTime gameTime, InputState istate)
		{
			if (button.SubButtons == null) return;

			button.SubButtons[index].FontProgress = progress;
		}

		protected override void OnEnd(SettingsButton button)
		{
			if (button.SubButtons == null) return;

			button.SubButtons[index].FontProgress = 1f;
		}

		protected override void OnAbort(SettingsButton owner)
		{
			OnEnd(owner);
		}

		public override string Name => "SettingsFontAppear";
	}
}
