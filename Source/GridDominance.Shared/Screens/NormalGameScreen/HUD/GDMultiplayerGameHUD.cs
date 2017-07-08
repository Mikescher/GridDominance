using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
{
	public class GDMultiplayerGameHUD : GameHUD, IGDGameHUD
	{
		public GDGameScreen GDOwner => (GDGameScreen)Screen;
		
		public readonly HUDPauseButton BtnPause;

		public GDMultiplayerGameHUD(GDGameScreen scrn) : base(scrn, Textures.HUDFontRegular)
		{
			AddElement(BtnPause = new HUDPauseButton(false, false, true));
		}
		
#if DEBUG
		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			root.IsVisible = !DebugSettings.Get("HideHUD");
		}
#endif
	}
}
