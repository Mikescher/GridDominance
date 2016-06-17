using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
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

		private readonly HUDPauseButton btnPause;
		private readonly HUDSpeedBaseButton btnSpeed;

		public GDGameHUD(GDGameScreen scrn) : base(scrn, Textures.HUDFontRegular)
		{
			AddElement(btnPause = new HUDPauseButton());
			AddElement(btnSpeed = new HUDSpeedBaseButton());
		}

		public void ShowScorePanel(PlayerProfile.PlayerProfile profile, FractionDifficulty? newDifficulty, bool playerHasWon)
		{
			btnPause.IsEnabled = false;
			btnSpeed.IsEnabled = false;

			GDOwner.GameSpeedMode = GameSpeedModes.NORMAL;

			AddElement(new HUDScorePanel(profile, newDifficulty, playerHasWon));
		} 
	}
}
