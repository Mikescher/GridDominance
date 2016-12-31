using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class HUDButtonVolume : HUDWorldSubSettingButton
	{
		private MainGame mainGame;

		public HUDButtonVolume(HUDWorldSettingsButton master) : base(master, 3)
		{
			mainGame = (MainGame) master.HUD.Screen.Game;
		}

		protected override TextureRegion2D GetIcon() => mainGame.Profile.SoundsEnabled ? Textures.TexHUDButtonIconVolumeOn : Textures.TexHUDButtonIconVolumeOff;
		protected override string GetText() => "Mute";

		protected override void OnPress(InputState istate)
		{
			mainGame.Profile.SoundsEnabled = !mainGame.Profile.SoundsEnabled;
			mainGame.SaveProfile();
		}
	}
}
