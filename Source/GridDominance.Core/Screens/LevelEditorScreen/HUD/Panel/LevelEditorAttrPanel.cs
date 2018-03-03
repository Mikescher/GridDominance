using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.LevelEditorScreen.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.LevelEditorScreen.HUD.Elements
{
	public class LevelEditorAttrPanel : HUDContainer
	{
		private const float HEIGHT = GDConstants.TILE_WIDTH * 4;

		public override int Depth => 0;

		public LevelEditorScreen GDScreen => (LevelEditorScreen) HUD.Screen;

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			FlatRenderHelper.DrawEdgeAlignedBlurPanel_Opaque(sbatch, bounds, FlatColors.BackgroundHUD2, FlatAlign5.BOTTOM, 16);
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate)   => true;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;

		protected override void RecalculatePosition()
		{
			base.RecalculatePosition();

			RelativePosition = new FPoint(GDConstants.TILE_WIDTH / 2f, 0);
			Size = new FSize(HUD.Width - 5 * GDConstants.TILE_WIDTH, HEIGHT);
			Alignment = HUDAlignment.BOTTOMLEFT;
		}

		public override void OnInitialize()
		{
			RelativePosition = new FPoint(GDConstants.TILE_WIDTH/2f, 0);
			Size = new FSize(HUD.Width - 5 * GDConstants.TILE_WIDTH, HEIGHT);
			Alignment = HUDAlignment.BOTTOMLEFT;
			IsVisible = false;
		}
		
		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			//
		}

		public void Recreate(ILeveleditorStub selection)
		{
			ClearChildren();
			if (selection == null)
			{
				IsVisible = false;
				return;
			}

			IsVisible = true;

			int i = 0;
			foreach (var opt in selection.AttrOptions)
			{
				AddElement(new AttributeButton
				{
					RelativePosition = new FPoint(32 + i * (128 + 64), 32),
					Size = new FSize(128, 128),
					Alignment = HUDAlignment.BOTTOMLEFT,

					Data = opt,
				});
				i++;
			}

			AddElement(new HUDTextButton
			{
				RelativePosition = new FPoint(32, 32),
				Size = new FSize(192, 64),
				Alignment = HUDAlignment.BOTTOMRIGHT,

				L10NText = L10NImpl.STR_LVLED_BTN_DEL,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				Click = DeleteSelected,

				BackgroundNormal = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Pomegranate, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Alizarin, Color.Black, HUD.PixelWidth),
			});

		}

		private void DeleteSelected(HUDTextButton sender, HUDButtonEventArgs e)
		{
			if (GDScreen.Selection is GameEntity ge)
			{
				ge.Alive = false;
				GDScreen.SelectStub(null);
			}
		}
	}
}
