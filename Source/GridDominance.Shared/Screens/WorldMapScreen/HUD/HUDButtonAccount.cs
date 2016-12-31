using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class HUDButtonAccount : HUDWorldSubSettingButton
	{
		public HUDButtonAccount(HUDWorldSettingsButton master) : base(master, 1)
		{
		}

		protected override TextureRegion2D GetIcon() => Textures.TexHUDButtonIconAccount;
		protected override string GetText() => "Account";

		protected override void OnPress(InputState istate)
		{
			//
		}
	}
}
