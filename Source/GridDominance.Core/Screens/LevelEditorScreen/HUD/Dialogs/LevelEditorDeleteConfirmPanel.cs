using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.LevelEditorScreen.HUD.Elements
{
	class LevelEditorDeleteConfirmPanel : HUDContainer
	{
		public override int Depth => 999;

		public LevelEditorScreen GDScreen => (LevelEditorScreen) HUD.Screen;

		public LevelEditorDeleteConfirmPanel() : base()
		{
			//
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			FlatRenderHelper.DrawRoundedBlurPanel(sbatch, bounds, FlatColors.BackgroundHUD2);
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate)   => true;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;

		public override void OnInitialize()
		{
			RelativePosition = new FPoint(0, 0);
			Size = new FSize(12 * GDConstants.TILE_WIDTH, 5 * GDConstants.TILE_WIDTH);
			Alignment = HUDAlignment.CENTER;

			AddElement(new HUDTextButton(1)
			{
				Alignment = HUDAlignment.CENTER,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(8 * GDConstants.TILE_WIDTH, 2 * GDConstants.TILE_WIDTH),

				L10NText = L10NImpl.STR_LVLED_BTN_DELLEVEL,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 75,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Alizarin, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Pomegranate, 16),

				Click = DoDelete,
			});
		}

		private void DoDelete(HUDTextButton sender, HUDButtonEventArgs e)
		{
			GDScreen.LevelData.Delete();
			MainGame.Inst.SetOverworldScreen();
			MainGame.Inst.ShowToast("LEDCP::DEL_CONF", L10N.T(L10NImpl.STR_LVLED_TOAST_DELLEVEL), 40, FlatColors.Flamingo, FlatColors.Foreground, 3f);
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			//
		}
	}
}
