using MonoSAMFramework.Portable.Screens.HUD.Operations;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class HUDSettingsOpenOperation : HUDTimedElementOperation<SettingsButton>
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

		protected override void OnProgress(SettingsButton button, float progress, InputState istate)
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

		public override string Name => "SettingsOpen";
	}
}
