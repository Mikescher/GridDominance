using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Leveleditor.Entities;
using GridDominance.Shared.Screens.NormalGameScreen;
using GridDominance.Shared.SCCM;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.Leveleditor.HUD.Elements
{
	class LevelEditorConfigPanel : HUDContainer
	{
		public override int Depth => 0;

		public LevelEditorScreen GDScreen => (LevelEditorScreen) HUD.Screen;

		private int _id;
		private string _name;
		private DSize _size;
		private FlatAlign9 _view;
		private GameWrapMode _geometry;

		private HUDLabel _ctrlID;
		private HUDTextBox _ctrlName;
		private HUDTextButton _ctrlSize;
		private HUDTextButton _ctrlView;
		private HUDTextButton _ctrlGeometry;

		public LevelEditorConfigPanel() : base()
		{
			_id = 808008;
			_name = "ASDF";
			_size = SCCMLevelData.SIZES[0];
			_view = FlatAlign9.CENTER;
			_geometry = GameWrapMode.Death;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			FlatRenderHelper.DrawSimpleBlurPanel_Opaque(sbatch, bounds, FlatColors.BackgroundHUD2, 16);
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate)   => true;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;

		protected override void RecalculatePosition()
		{
			base.RecalculatePosition();

			RelativePosition = new FPoint(GDConstants.TILE_WIDTH, 0);
			Size = new FSize(HUD.Width - 6 * GDConstants.TILE_WIDTH, HUD.Height - 2 * GDConstants.TILE_WIDTH);
			Alignment = HUDAlignment.CENTERLEFT;
		}

		public override void OnInitialize()
		{
			RelativePosition = new FPoint(GDConstants.TILE_WIDTH, 0);
			Size = new FSize(HUD.Width - 6 * GDConstants.TILE_WIDTH, HUD.Height - 2 * GDConstants.TILE_WIDTH);
			Alignment = HUDAlignment.CENTERLEFT;

			AddElement(new HUDLabel(1)
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(32, 32 + (96 * 0)),
				Size = new FSize(384, 64),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText = L10NImpl.STR_LVLED_CFG_ID,
				TextColor = FlatColors.Clouds,
			});

			AddElement(_ctrlID = new HUDLabel(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(416, 32 + (96 * 0)),
				Size = new FSize(512, 64),

				Text = "000000000000",
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,

				Background = HUDBackgroundDefinition.CreateSimpleOutline(Color.White, Color.Black, HUD.PixelWidth),
			});

			AddElement(new HUDLabel(1)
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(32, 32 + (96 * 1)),
				Size = new FSize(384, 64),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText = L10NImpl.STR_LVLED_CFG_NAME,
				TextColor = FlatColors.Clouds,
			});

			AddElement(_ctrlName = new HUDSimpleTextBox(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(416, 32 + (96 * 1)),
				Size = new FSize(512, 64),

				ColorText = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,

				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(Color.White, Color.Black, HUD.PixelWidth),
				BackgroundFocused = HUDBackgroundDefinition.CreateSimpleOutline(Color.White, Color.Black, HUD.PixelWidth),
			});

			AddElement(new HUDLabel(1)
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(32, 32 + (96 * 2)),
				Size = new FSize(384, 64),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText = L10NImpl.STR_LVLED_CFG_SIZE,
				TextColor = FlatColors.Clouds,
			});

			AddElement(_ctrlSize = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(416, 32 + (96 * 2)),
				Size = new FSize(512, 64),

				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),
			});

			AddElement(new HUDLabel(1)
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(32, 32 + (96 * 3)),
				Size = new FSize(384, 64),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText = L10NImpl.STR_LVLED_CFG_VIEW,
				TextColor = FlatColors.Clouds,
			});

			AddElement(_ctrlView = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(416, 32 + (96 * 3)),
				Size = new FSize(512, 64),

				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),
			});

			AddElement(new HUDLabel(1)
			{
				TextAlignment = HUDAlignment.CENTERLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(32, 32 + (96 * 4)),
				Size = new FSize(384, 64),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText = L10NImpl.STR_LVLED_CFG_GEOMETRY,
				TextColor = FlatColors.Clouds,
			});

			AddElement(_ctrlGeometry = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(416, 32 + (96 * 4)),
				Size = new FSize(512, 64),

				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),
			});

			//------

			AddElement(new HUDTextButton(1)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(24, 16 + 75 * 0),
				Size = new FSize(384, 64),

				L10NText = L10NImpl.STR_BTN_YES,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Pomegranate, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Alizarin, Color.Black, HUD.PixelWidth),

				Click = Close,
			});

			RefreshControls();
		}

		private void RefreshControls()
		{
			_ctrlID.Text = _id.ToString();
			_ctrlName.Text = _name;
			_ctrlSize.Text = _size.Width + "x"+_size.Height;
			_ctrlView.Text = FlatAlign9Helper.LETTERS[_view];
			switch (_geometry)
			{
				case GameWrapMode.Death:
					_ctrlGeometry.L10NText = L10NImpl.STR_LVLED_CFG_WRAP_INFINITY;
					break;
				case GameWrapMode.Donut:
					_ctrlGeometry.L10NText = L10NImpl.STR_LVLED_CFG_WRAP_DONUT;
					break;
				case GameWrapMode.Reflect:
					_ctrlGeometry.L10NText = L10NImpl.STR_LVLED_CFG_WRAP_REFLECT;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void Close(HUDTextButton sender, HUDButtonEventArgs e)
		{
			//todo apply changes
			this.Remove();
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
