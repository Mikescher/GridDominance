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

		public HUDTextButton BtnMouse;
		public HUDTextButton BtnCannon;
		public HUDTextButton BtnWall;
		public HUDTextButton BtnObstacle;
		public HUDTextButton BtnSettings;
		public HUDTextButton BtnPlay;
		public HUDTextButton BtnTest;
		public HUDTextButton BtnExit;

		public List<Tuple<HUDTextButton, HUDBackgroundDefinition, HUDBackgroundDefinition>> Buttons = new List<Tuple<HUDTextButton, HUDBackgroundDefinition, HUDBackgroundDefinition>>();

		public LevelEditorScreen GDScreen => (LevelEditorScreen) HUD.Screen;

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			FlatRenderHelper.DrawCornerlessBlurPanel_Opaque(sbatch, bounds, FlatColors.BackgroundHUD2, 16, false, false, false, true);
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate)   => true;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;

		public override void OnInitialize()
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HUD.Height);
			Alignment = HUDAlignment.TOPRIGHT;

			AddElement(BtnMouse = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 16 + 75 * 0),
				Size = new FSize((WIDTH - 2 * 24), 64),

				L10NText = L10NImpl.STR_LVLED_MOUSE,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				
				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD,        Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),

				Click = SetModeMouse,
			});

			AddElement(BtnCannon = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 16 + 75 * 1),
				Size = new FSize((WIDTH - 2 * 24), 64),

				L10NText = L10NImpl.STR_LVLED_CANNON,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				
				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD,        Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),

				Click = SetModeCannon,
			});

			AddElement(BtnWall = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 16 + 75 * 2),
				Size = new FSize((WIDTH - 2 * 24), 64),

				L10NText = L10NImpl.STR_LVLED_WALL,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				
				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD,        Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),

				Click = SetModeWall,
			});

			AddElement(BtnObstacle = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 16 + 75 * 3),
				Size = new FSize((WIDTH - 2 * 24), 64),

				L10NText = L10NImpl.STR_LVLED_OBSTACLE,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				
				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD,        Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),

				Click = SetModeObstacle,
			});

			AddElement(BtnSettings = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 16 + 75 * 4),
				Size = new FSize((WIDTH - 2 * 24), 64),

				L10NText = L10NImpl.STR_LVLED_SETTINGS,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD,        Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),

				Click = SetModeSettings,
			});

			AddElement(BtnPlay = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, 16 + 75 * 3),
				Size = new FSize((WIDTH - 2 * 24), 64),

				L10NText = L10NImpl.STR_LVLED_PLAY,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Orange,   Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.SunFlower, Color.Black, HUD.PixelWidth),

				Click = DoPlayTest,
			});

			AddElement(BtnTest = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, 16 + 75 * 2),
				Size = new FSize((WIDTH - 2 * 24), 64),

				L10NText = L10NImpl.STR_LVLED_UPLOAD,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Nephritis, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Emerald,   Color.Black, HUD.PixelWidth),

				Click = TryUpload,
			});

			AddElement(BtnExit = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(24, 16 + 75 * 0),
				Size = new FSize((WIDTH - 3 * 24), 64),

				L10NText = L10NImpl.STR_LVLED_EXIT,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 48,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Pomegranate, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Alizarin,    Color.Black, HUD.PixelWidth),

				Click = ExitEditor,
			});

			Buttons.Add(Tuple.Create(BtnMouse,    BtnMouse.BackgroundNormal,    BtnMouse.BackgroundPressed));
			Buttons.Add(Tuple.Create(BtnCannon,   BtnCannon.BackgroundNormal,   BtnCannon.BackgroundPressed));
			Buttons.Add(Tuple.Create(BtnWall,     BtnWall.BackgroundNormal,     BtnWall.BackgroundPressed));
			Buttons.Add(Tuple.Create(BtnObstacle, BtnObstacle.BackgroundNormal, BtnObstacle.BackgroundPressed));
			Buttons.Add(Tuple.Create(BtnSettings, BtnSettings.BackgroundNormal, BtnSettings.BackgroundPressed));
			Buttons.Add(Tuple.Create(BtnPlay,     BtnPlay.BackgroundNormal,     BtnPlay.BackgroundPressed));
			Buttons.Add(Tuple.Create(BtnTest,     BtnTest.BackgroundNormal,     BtnTest.BackgroundPressed));
			Buttons.Add(Tuple.Create(BtnExit,     BtnExit.BackgroundNormal,     BtnExit.BackgroundPressed));


			SetActiveButton(BtnMouse);
		}

		private void ExitEditor(HUDTextButton sender, HUDButtonEventArgs e)
		{
			GDScreen.SetMode(LevelEditorMode.Mouse);
		}

		private void TryUpload(HUDTextButton sender, HUDButtonEventArgs e)
		{
			GDScreen.SetMode(LevelEditorMode.Mouse);
		}

		private void DoPlayTest(HUDTextButton sender, HUDButtonEventArgs e)
		{
			GDScreen.SetMode(LevelEditorMode.Mouse);
		}

		private void SetModeSettings(HUDTextButton sender, HUDButtonEventArgs e)
		{
			GDScreen.SetMode(LevelEditorMode.Mouse);
		}

		private void SetModeObstacle(HUDTextButton sender, HUDButtonEventArgs e)
		{
			GDScreen.SetMode(LevelEditorMode.AddObstacle);
		}

		private void SetModeWall(HUDTextButton sender, HUDButtonEventArgs e)
		{
			GDScreen.SetMode(LevelEditorMode.AddWall);
		}

		private void SetModeCannon(HUDTextButton sender, HUDButtonEventArgs e)
		{
			GDScreen.SetMode(LevelEditorMode.AddCannon);
		}

		private void SetModeMouse(HUDTextButton sender, HUDButtonEventArgs e)
		{
			GDScreen.SetMode(LevelEditorMode.Mouse);
		}

		public void SetActiveButton(HUDTextButton activeButton)
		{
			foreach (var btnObj in Buttons)
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
