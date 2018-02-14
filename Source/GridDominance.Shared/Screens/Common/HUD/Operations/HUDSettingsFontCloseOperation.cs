using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class HUDSettingsFontCloseOperation : FixTimeOperation<SettingsButton>
	{
		private readonly int _index;

		private float startState;

		public HUDSettingsFontCloseOperation(int idx) : base(0.25f)
		{
			_index = idx;
		}

		protected override void OnStart(SettingsButton button)
		{
			startState = button.SubButtons[_index].FontProgress;
		}

		protected override void OnProgress(SettingsButton button, float progress, SAMTime gameTime, InputState istate)
		{
			button.SubButtons[_index].FontProgress = startState * (1 - progress);
		}

		protected override void OnEnd(SettingsButton button)
		{
			button.SubButtons[_index].Alive = false;
			button.SubButtons[_index].ScaleProgress = 0f;
		}

		protected override void OnAbort(SettingsButton owner)
		{
			OnEnd(owner);
		}

		public override string Name => "SettingsFontCloseOperation";
	}
}
