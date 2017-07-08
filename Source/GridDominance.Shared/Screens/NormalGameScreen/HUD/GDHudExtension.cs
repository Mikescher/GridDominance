using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
{
	static class GDHudExtension
	{
		public static IGDGameHUD GDHUD(this HUDElement e) => (IGDGameHUD) e.HUD;
	}
}
