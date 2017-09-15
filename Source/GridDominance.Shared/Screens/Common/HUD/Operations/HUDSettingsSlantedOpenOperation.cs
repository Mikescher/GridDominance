using MonoSAMFramework.Portable.Screens.HUD.Operations;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class HUDSettingsHorizontalOpenOperation : HUDTimedElementOperation<SettingsButton>
	{
		private readonly int _index;
		private SubSettingButton _button;

		private float startX;
		private float endX;

		public HUDSettingsHorizontalOpenOperation(int idx, int btnindex) : base(0.4f)
		{
			_index = idx;
		}

		protected override void OnStart(SettingsButton button)
		{
			_button = button.SubButtons[_index];

			startX = _button.RelativePosition.X;
			endX = _button.TargetPosition.X;
		}

		protected override void OnProgress(SettingsButton button, float progress, InputState istate)
		{
			var xx = startX + (endX - startX) * FloatMath.FunctionEaseOutQuad(progress);
			var yy = _button.RelativePosition.Y;

			_button.RelativePosition = new FPoint(xx, yy);
		}

		protected override void OnEnd(SettingsButton button)
		{
			button.OpeningProgress = 1f;
			button.RotationSpeed = 0.5f;
		}

		public override string Name => "SettingsOpen";
	}
}
