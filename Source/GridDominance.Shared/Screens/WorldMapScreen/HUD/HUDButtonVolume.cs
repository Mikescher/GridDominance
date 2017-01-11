using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class HUDButtonVolume : HUDWorldSubSettingButton
	{
		public HUDButtonVolume(HUDWorldSettingsButton master) : base(master, 3)
		{
			//
		}

		protected override TextureRegion2D GetIcon() => MainGame.Inst.Profile.SoundsEnabled ? Textures.TexHUDButtonIconVolumeOn : Textures.TexHUDButtonIconVolumeOff;
		protected override string GetText() => "Mute";

		protected override void OnPress(InputState istate)
		{
			MainGame.Inst.Profile.SoundsEnabled = !MainGame.Inst.Profile.SoundsEnabled;
			MainGame.Inst.SaveProfile();
		}
	}
}
