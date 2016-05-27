using GridDominance.Shared.Screens.ScreenGame.HUD;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.ScreenGame.hud
{
	class GDGameHUD : GameHUD
	{
		public GDGameScreen GDOwner => (GDGameScreen)Screen;

		public override int Top  => -(int)GDOwner.GDViewport.VirtualOffsetY;
		public override int Left => -(int)GDOwner.GDViewport.VirtualOffsetX;

		public override int Bottom => (int)(GDOwner.GDViewport.VirtualHeight + GDOwner.GDViewport.VirtualOffsetY);
		public override int Right => (int)(GDOwner.GDViewport.VirtualWidth + GDOwner.GDViewport.VirtualOffsetX);

		public GDGameHUD(GDGameScreen scrn) : base(scrn)
		{
			AddElement(new HUDPauseButton());
			AddElement(new HUDSpeedBaseButton());
			AddElement(new HUDScorePanel());
		}
	}
}
