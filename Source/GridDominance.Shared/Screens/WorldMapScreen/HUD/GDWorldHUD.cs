using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class GDWorldHUD : GameHUD
	{
		public GDWorldMapScreen GDOwner => (GDWorldMapScreen)Screen;

		public GDWorldHUD(GDWorldMapScreen scrn) : base(scrn, Textures.HUDFontRegular)
		{
			AddElement(new HUDWorldSettingsButton());
		}
	}
}
