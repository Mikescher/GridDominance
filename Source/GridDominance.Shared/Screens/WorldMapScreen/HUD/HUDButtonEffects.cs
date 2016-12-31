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
		private MainGame mainGame;

		public HUDButtonEffects(HUDWorldSettingsButton master) : base(master, 4)
		{
			mainGame = (MainGame)master.HUD.Screen.Game;
		}

		protected override TextureRegion2D GetIcon() => mainGame.Profile.EffectsEnabled ? Textures.TexHUDButtonIconEffectsOn : Textures.TexHUDButtonIconEffectsOff;
		protected override string GetText() => "Effects";

		protected override void OnPress(InputState istate)
		{
			mainGame.Profile.EffectsEnabled = !mainGame.Profile.EffectsEnabled;
			mainGame.SaveProfile();
		}
	}
}
