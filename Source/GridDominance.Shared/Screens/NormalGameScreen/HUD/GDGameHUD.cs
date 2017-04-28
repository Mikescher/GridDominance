using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
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

		public void ShowScorePanel(LevelBlueprint lvl, PlayerProfile profile, FractionDifficulty? newDifficulty, bool playerHasWon, int addPoints)
		{
			btnPause.IsEnabled = false;
			btnSpeed.IsEnabled = false;

			GDOwner.GameSpeedMode = GameSpeedModes.NORMAL;

			if (GDOwner.IsTutorial)
				AddModal(new HUDTutorialScorePanel(profile, addPoints), false);
			else
				AddModal(new HUDScorePanel(lvl, profile, newDifficulty, playerHasWon, addPoints), false);
		} 
	}
}
