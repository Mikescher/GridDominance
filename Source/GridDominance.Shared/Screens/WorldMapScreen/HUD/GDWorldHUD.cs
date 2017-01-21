using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class GDWorldHUD : GameHUD
	{
		public GDWorldMapScreen GDOwner => (GDWorldMapScreen)Screen;
		

		public GDWorldHUD(GDWorldMapScreen scrn) : base(scrn, Textures.HUDFontRegular)
		{
			AddElement(new SettingsButton());
			AddElement(new ScoreDisplay());
			AddElement(new TopLevelDisplay());
		}
	}
}
