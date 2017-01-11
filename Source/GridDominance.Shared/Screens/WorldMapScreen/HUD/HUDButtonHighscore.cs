using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class HUDButtonHighscore : HUDWorldSubSettingButton
	{
		public HUDButtonHighscore(HUDWorldSettingsButton master) : base(master, 2)
		{
		}

		protected override TextureRegion2D GetIcon() => Textures.TexHUDButtonIconHighscore;
		protected override string GetText() => "Highscore";

		protected override void OnPress(InputState istate)
		{
			//
		}
	}
}
