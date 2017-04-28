using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
{
	static class GDHudExtension
	{
		public static GDGameHUD GDHUD(this HUDElement e) => (GDGameHUD) e.HUD;
	}
}
