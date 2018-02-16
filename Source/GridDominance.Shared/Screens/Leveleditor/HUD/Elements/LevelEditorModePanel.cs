using System;
using System.Collections.Generic;
using System.Text;
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

namespace GridDominance.Shared.Screens.Leveleditor.HUD.Elements
{
	class LevelEditorModePanel : HUDContainer
	{
		private const float WIDTH = GDConstants.TILE_WIDTH * 4;

		public override int Depth => 0;

		private HUDTextButton _btnMouse;
		private HUDTextButton _btnCannon;
		private HUDTextButton _btnWall;
		private HUDTextButton _btnObstacle;
		private HUDTextButton _btnSettings;
		private HUDTextButton _btnPlay;
		private HUDTextButton _btnTest;
		private HUDTextButton _btnExit;

		private List<Tuple<HUDTextButton, HUDBackgroundDefinition, HUDBackgroundDefinition>> _buttons = new List<Tuple<HUDTextButton, HUDBackgroundDefinition, HUDBackgroundDefinition>>();

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			FlatRenderHelper.DrawCornerlessBlurPanel_Opaque(sbatch, bounds, FlatColors.BackgroundHUD2, 16, false, false, false, true);
		}

		public override void OnInitialize()
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HUD.Height);
			Alignment = HUDAlignment.TOPRIGHT;

			AddElement(_btnMouse = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 16 + 75 * 0),
				Size = new FSize((WIDTH - 2 * 24), 64),

				L10NText = L10NImpl.STR_LVLED_MOUSE,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				
				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD,        Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),

				Click = SetModeMouse,
			});

			AddElement(_btnCannon = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 16 + 75 * 1),
				Size = new FSize((WIDTH - 2 * 24), 64),

				L10NText = L10NImpl.STR_LVLED_CANNON,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				
				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD,        Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),

				Click = SetModeCannon,
			});

			AddElement(_btnWall = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 16 + 75 * 2),
				Size = new FSize((WIDTH - 2 * 24), 64),

				L10NText = L10NImpl.STR_LVLED_WALL,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				
				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD,        Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),

				Click = SetModeWall,
			});

			AddElement(_btnObstacle = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 16 + 75 * 3),
				Size = new FSize((WIDTH - 2 * 24), 64),

				L10NText = L10NImpl.STR_LVLED_OBSTACLE,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				
				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD,        Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),

				Click = SetModeObstacle,
			});

			AddElement(_btnSettings = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 16 + 75 * 4),
				Size = new FSize((WIDTH - 2 * 24), 64),

				L10NText = L10NImpl.STR_LVLED_SETTINGS,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD,        Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),

				Click = SetModeSettings,
			});

			AddElement(_btnPlay = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, 16 + 75 * 2),
				Size = new FSize((WIDTH - 2 * 24), 64),

				L10NText = L10NImpl.STR_LVLED_PLAY,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Orange,   Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.SunFlower, Color.Black, HUD.PixelWidth),

				Click = DoPlayTest,
			});

			AddElement(_btnTest = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, 16 + 75 * 1),
				Size = new FSize((WIDTH - 2 * 24), 64),

				L10NText = L10NImpl.STR_LVLED_UPLOAD,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Nephritis, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Emerald,   Color.Black, HUD.PixelWidth),

				Click = TryUpload,
			});

			AddElement(_btnExit = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(24, 16 + 75 * 0),
				Size = new FSize((WIDTH - 3 * 24), 64),

				L10NText = L10NImpl.STR_LVLED_EXIT,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Pomegranate, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Alizarin,    Color.Black, HUD.PixelWidth),

				Click = ExitEditor,
			});

			_buttons.Add(Tuple.Create(_btnMouse,    _btnMouse.BackgroundNormal,    _btnMouse.BackgroundPressed));
			_buttons.Add(Tuple.Create(_btnCannon,   _btnCannon.BackgroundNormal,   _btnCannon.BackgroundPressed));
			_buttons.Add(Tuple.Create(_btnWall,     _btnWall.BackgroundNormal,     _btnWall.BackgroundPressed));
			_buttons.Add(Tuple.Create(_btnObstacle, _btnObstacle.BackgroundNormal, _btnObstacle.BackgroundPressed));
			_buttons.Add(Tuple.Create(_btnSettings, _btnSettings.BackgroundNormal, _btnSettings.BackgroundPressed));
			_buttons.Add(Tuple.Create(_btnPlay,     _btnPlay.BackgroundNormal,     _btnPlay.BackgroundPressed));
			_buttons.Add(Tuple.Create(_btnTest,     _btnTest.BackgroundNormal,     _btnTest.BackgroundPressed));
			_buttons.Add(Tuple.Create(_btnExit,     _btnExit.BackgroundNormal,     _btnExit.BackgroundPressed));


			SetActiveButton(_btnMouse);
		}

		private void ExitEditor(HUDTextButton sender, HUDButtonEventArgs e)
		{
			SetActiveButton(_btnMouse);
		}

		private void TryUpload(HUDTextButton sender, HUDButtonEventArgs e)
		{
			SetActiveButton(_btnMouse);
		}

		private void DoPlayTest(HUDTextButton sender, HUDButtonEventArgs e)
		{
			SetActiveButton(_btnMouse);
		}

		private void SetModeSettings(HUDTextButton sender, HUDButtonEventArgs e)
		{
			SetActiveButton(_btnSettings);
		}

		private void SetModeObstacle(HUDTextButton sender, HUDButtonEventArgs e)
		{
			SetActiveButton(_btnObstacle);
		}

		private void SetModeWall(HUDTextButton sender, HUDButtonEventArgs e)
		{
			SetActiveButton(_btnWall);
		}

		private void SetModeCannon(HUDTextButton sender, HUDButtonEventArgs e)
		{
			SetActiveButton(_btnCannon);
		}

		private void SetModeMouse(HUDTextButton sender, HUDButtonEventArgs e)
		{
			SetActiveButton(_btnMouse);
		}

		private void SetActiveButton(HUDTextButton activeButton)
		{
			foreach (var btnObj in _buttons)
			{
				if (btnObj.Item1 == activeButton)
				{
					btnObj.Item1.BackgroundNormal = HUDBackgroundDefinition.CreateSimpleOutlineBlur(btnObj.Item3.Color, btnObj.Item3.OutlineColor, btnObj.Item3.OutlineThickness, 16);
					btnObj.Item1.BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutlineBlur(btnObj.Item2.Color, btnObj.Item2.OutlineColor, btnObj.Item2.OutlineThickness, 16);
				}
				else
				{
					btnObj.Item1.BackgroundNormal = btnObj.Item2;
					btnObj.Item1.BackgroundPressed = btnObj.Item3;
				}
			}
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
