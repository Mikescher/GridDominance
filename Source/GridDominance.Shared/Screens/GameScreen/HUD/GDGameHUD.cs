using GridDominance.Levelfileformat.Parser;
using GridDominance.Levelformat.Parser;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using GridDominance.Shared.Screens.ScreenGame.HUD;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.ScreenGame.hud
{
	public class GDGameHUD : GameHUD
	{
		public GDGameScreen GDOwner => (GDGameScreen)Screen;
		
		private readonly HUDPauseButton btnPause;
		private readonly HUDSpeedBaseButton btnSpeed;

		public GDGameHUD(GDGameScreen scrn) : base(scrn, Textures.HUDFontRegular)
		{
			AddElement(btnPause = new HUDPauseButton());
			AddElement(btnSpeed = new HUDSpeedBaseButton());
		}

		public void ShowScorePanel(LevelFile lvl, PlayerProfile profile, FractionDifficulty? newDifficulty, bool playerHasWon, int addPoints)
		{
			btnPause.IsEnabled = false;
			btnSpeed.IsEnabled = false;

			GDOwner.GameSpeedMode = GameSpeedModes.NORMAL;

			AddModal(new HUDScorePanel(lvl, profile, newDifficulty, playerHasWon, addPoints), false);
		} 
	}
}
