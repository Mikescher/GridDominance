using GridDominance.Shared.Screens.ScreenGame.hud;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.ScreenGame.HUD
{
	abstract class GDGameHUDElement : GameHUDElement
	{
		public GDGameHUD GDOwner => (GDGameHUD) Owner;
	}
}
