using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.HUD.Elements;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
{
	public class GDGameHUD : GameHUD, IGDGameHUD
	{
		public GDGameScreen GDOwner => (GDGameScreen)Screen;
		
		public readonly HUDPauseButton BtnPause;
		public readonly HUDSpeedBaseButton BtnSpeed;

		public GDGameHUD(GDGameScreen scrn) : base(scrn, Textures.HUDFontRegular)
		{
			AddElement(BtnPause = new HUDPauseButton(true, true, true));
			AddElement(BtnSpeed = new HUDSpeedBaseButton());
		}
		
#if DEBUG
		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			root.IsVisible = !DebugSettings.Get("HideHUD");
		}
#endif
	}
}
