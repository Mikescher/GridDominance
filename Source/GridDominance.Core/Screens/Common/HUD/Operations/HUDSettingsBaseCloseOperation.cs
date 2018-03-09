using GridDominance.Shared.Screens.Common.HUD.Elements;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.Common.HUD.Operations
{
	class HUDSettingsBaseCloseOperation : FixTimeOperation<SettingsButton>
	{
		public HUDSettingsBaseCloseOperation() : base(0.25f)
		{
			//
		}

		protected override void OnStart(SettingsButton button)
		{
			button.OpeningProgress = 1f;
			button.RotationSpeed = 0.5f;
			button.OpeningState = BistateProgress.Closing;
		}

		protected override void OnProgress(SettingsButton button, float progress, SAMTime gameTime, InputState istate)
		{
			button.OpeningProgress = 1 - progress;

			button.RotationSpeed = 0.5f + progress * 0.5f;
		}

		protected override void OnEnd(SettingsButton button)
		{
			button.SubButtons = null;
			button.OpeningProgress = 0f;
			button.RotationSpeed = 1f;
			button.OpeningState = BistateProgress.Closed;
		}

		protected override void OnAbort(SettingsButton owner)
		{
			OnEnd(owner);
		}

		public override string Name => "SettingsCloseOperation";
	}
}
