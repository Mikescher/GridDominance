using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class HUDButtonAbout : HUDWorldSubSettingButton
	{
		public HUDButtonAbout(HUDWorldSettingsButton master) : base(master, 0)
		{
		}

		protected override TextureRegion2D GetIcon() => Textures.TexHUDButtonIconAbout;
		protected override string GetText() => "About";

		protected override void OnPress(InputState istate)
		{
			//
		}
	}
}
