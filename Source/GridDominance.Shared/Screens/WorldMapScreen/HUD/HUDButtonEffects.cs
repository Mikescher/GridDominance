using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class HUDButtonEffects : HUDWorldSubSettingButton
	{
		public HUDButtonEffects(HUDWorldSettingsButton master) : base(master, 4)
		{
			//
		}

		protected override TextureRegion2D GetIcon() => MainGame.Inst.Profile.EffectsEnabled ? Textures.TexHUDButtonIconEffectsOn : Textures.TexHUDButtonIconEffectsOff;
		protected override string GetText() => "Effects";

		protected override void OnPress(InputState istate)
		{
			MainGame.Inst.Profile.EffectsEnabled = !MainGame.Inst.Profile.EffectsEnabled;
			MainGame.Inst.SaveProfile();
		}
	}
}
