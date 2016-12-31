using MonoSAMFramework.Portable.Screens.HUD.Operations;
using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.GameMath;
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
			if (button.subButtons == null) return;

			button.subButtons[index].fontProgress = 0f;
		}

		protected override void OnProgress(HUDWorldSettingsButton button, float progress, InputState istate)
		{
			if (button.subButtons == null) return;

			button.subButtons[index].fontProgress = progress;
		}

		protected override void OnEnd(HUDWorldSettingsButton button)
		{
			if (button.subButtons == null) return;

			button.subButtons[index].fontProgress = 1f;
		}
	}
}
