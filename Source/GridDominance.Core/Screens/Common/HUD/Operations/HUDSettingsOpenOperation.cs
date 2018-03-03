using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class HUDSettingsOpenOperation : FixTimeOperation<SettingsButton>
	{
		public HUDSettingsOpenOperation() : base(0.5f)
		{

		}

		protected override void OnStart(SettingsButton button)
		{
			button.CreateSubButtons();

			button.OpeningProgress = 0f;

			button.RotationSpeed = 1f;
		}

		protected override void OnProgress(SettingsButton button, float progress, SAMTime gameTime, InputState istate)
		{
			button.OpeningProgress = progress;

			foreach (var sub in button.SubButtons)
			{
				sub.OffsetProgress = FloatMath.FunctionEaseInOutCubic(progress);
			}

			button.RotationSpeed = 1f - progress*0.5f;
		}

		protected override void OnEnd(SettingsButton button)
		{
			button.OpeningProgress = 1f;
			button.RotationSpeed = 0.5f;
		}

		protected override void OnAbort(SettingsButton owner)
		{
			OnEnd(owner);
		}

		public override string Name => "SettingsOpen";
	}
}
