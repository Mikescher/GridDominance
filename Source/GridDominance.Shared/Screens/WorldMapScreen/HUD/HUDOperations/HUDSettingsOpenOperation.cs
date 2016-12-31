using MonoSAMFramework.Portable.Screens.HUD.Operations;
using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class HUDSettingsOpenOperation : HUDTimedElementOperation<HUDWorldSettingsButton>
	{
		public HUDSettingsOpenOperation() : base(0.5f)
		{

		}

		protected override void OnStart(HUDWorldSettingsButton button)
		{
			button.subButtons = new HUDWorldSubSettingButton[5];

			button.subButtons[0] = new HUDButtonAbout(button);
			button.subButtons[1] = new HUDButtonAccount(button);
			button.subButtons[2] = new HUDButtonHighscore(button);
			button.subButtons[3] = new HUDButtonVolume(button);
			button.subButtons[4] = new HUDButtonEffects(button);

			button.HUD.AddElements(button.subButtons);

			button.openingProgress = 0f;

			button.rotationSpeed = 1f;
		}

		protected override void OnProgress(HUDWorldSettingsButton button, float progress, InputState istate)
		{
			button.openingProgress = progress;

			foreach (var sub in button.subButtons)
			{
				sub.offsetProgress = FloatMath.FunctionEaseInOutCubic(progress);
			}

			button.rotationSpeed = 1f - progress*0.5f;
		}

		protected override void OnEnd(HUDWorldSettingsButton button)
		{
			button.openingProgress = 1f;
			button.rotationSpeed = 0.5f;
		}
	}
}
