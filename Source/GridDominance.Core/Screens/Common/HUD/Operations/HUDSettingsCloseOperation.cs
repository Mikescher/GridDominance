using GridDominance.Shared.Screens.Common.HUD.Elements;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.Common.HUD.Operations
{
	class HUDSettingsCloseOperation : FixTimeOperation<SettingsButton>
	{
		private readonly int _index;

		private float startScale;

		public HUDSettingsCloseOperation(int idx) : base(0.25f)
		{
			_index = idx;
		}

		protected override void OnStart(SettingsButton button)
		{
			startScale = button.SubButtons[_index].ScaleProgress;
		}

		protected override void OnProgress(SettingsButton button, float progress, SAMTime gameTime, InputState istate)
		{
			button.SubButtons[_index].ScaleProgress = startScale * (1 - progress);
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

		public override string Name => "SettingsCloseOperation";
	}
}
