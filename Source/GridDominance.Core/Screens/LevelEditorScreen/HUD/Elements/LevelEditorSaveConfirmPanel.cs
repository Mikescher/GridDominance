using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.LevelEditorScreen.HUD.Elements
{
	class LevelEditorSaveConfirmPanel : HUDContainer
	{
		public override int Depth => 999;

		public LevelEditorScreen GDScreen => (LevelEditorScreen) HUD.Screen;

		public LevelEditorSaveConfirmPanel() : base()
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
			Size = new FSize(14 * GDConstants.TILE_WIDTH, 7 * GDConstants.TILE_WIDTH);
			Alignment = HUDAlignment.CENTER;

			AddElement(new HUDTextButton(1)
			{
				Alignment = HUDAlignment.CENTER,
				RelativePosition = new FPoint(0, -1.5f * GDConstants.TILE_WIDTH),
				Size = new FSize(12 * GDConstants.TILE_WIDTH, 2 * GDConstants.TILE_WIDTH),

				L10NText = L10NImpl.STR_LVLED_BTN_SAVE,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 75,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.PeterRiver, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.BelizeHole, 16),

				Click = DoSave,
			});

			AddElement(new HUDTextButton(1)
			{
				Alignment = HUDAlignment.CENTER,
				RelativePosition = new FPoint(0, +1.5f * GDConstants.TILE_WIDTH),
				Size = new FSize(12 * GDConstants.TILE_WIDTH, 2 * GDConstants.TILE_WIDTH),

				L10NText = L10NImpl.STR_LVLED_BTN_DISCARD,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 75,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Concrete, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Asbestos, 16),

				Click = DoDiscard,
			});
		}

		private void DoSave(HUDTextButton sender, HUDButtonEventArgs e)
		{
			var ret = GDScreen.LevelData.UpdateAndSave(GDScreen);
			if (ret) MainGame.Inst.SetOverworldScreen(); //TODO open sccm pnl + myleveltab
		}

		private void DoDiscard(HUDTextButton sender, HUDButtonEventArgs e)
		{
			MainGame.Inst.SetOverworldScreen(); //TODO open sccm pnl + myleveltab
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
