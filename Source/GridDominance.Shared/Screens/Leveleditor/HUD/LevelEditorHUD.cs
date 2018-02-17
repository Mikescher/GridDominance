using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Leveleditor.HUD.Elements;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.Leveleditor.HUD
{
	class LevelEditorHUD : GameHUD
	{
		public readonly LevelEditorModePanel ModePanel;
		public readonly LevelEditorAttrPanel AttrPanel;

		public LevelEditorHUD(GameScreen scrn) : base(scrn, Textures.HUDFontRegular)
		{
			AddElement(ModePanel = new LevelEditorModePanel());
			AddElement(AttrPanel = new LevelEditorAttrPanel());
		}

#if DEBUG
		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			root.IsVisible = !DebugSettings.Get("HideHUD");
		}
#endif
	}
}
