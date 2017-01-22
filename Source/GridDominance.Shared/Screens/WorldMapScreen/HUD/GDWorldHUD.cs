using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.WorldMapScreen.Entities;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class GDWorldHUD : GameHUD
	{
		public GDWorldMapScreen GDOwner => (GDWorldMapScreen)Screen;

		public LevelNode SelectedNode = null;

		public readonly TopLevelDisplay TopLevelDisplay;

		public GDWorldHUD(GDWorldMapScreen scrn) : base(scrn, Textures.HUDFontRegular)
		{
			AddElement(new SettingsButton());
			AddElement(new ScoreDisplay());
			AddElement(TopLevelDisplay = new TopLevelDisplay());
			AddElement(new InformationDisplay());
		}

		public void SelectNode(LevelNode n)
		{
			SelectedNode = n;
		}
	}
}
