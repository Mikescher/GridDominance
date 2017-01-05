using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using GridDominance.Shared.Screens.ScreenGame.HUD;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;

namespace GridDominance.Shared.Screens.ScreenGame.hud
{
	class GDGameHUD : GameHUD
	{
		public GDGameScreen GDOwner => (GDGameScreen)Screen;
		
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

			AddElement(new HUDModalDialog(new HUDScorePanel(profile, newDifficulty, playerHasWon)));
		} 
	}
}
