using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.Leveleditor.HUD
{
	class LevelEditorHUD : GameHUD
	{
		public LevelEditorHUD(GameScreen scrn) : base(scrn, Textures.HUDFontRegular)
		{
		}

#if DEBUG
		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			root.IsVisible = !DebugSettings.Get("HideHUD");
		}
#endif
	}
}
