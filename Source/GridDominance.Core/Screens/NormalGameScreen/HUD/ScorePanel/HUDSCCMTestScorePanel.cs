using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common.HUD.Elements;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.SCCM;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD.ScorePanel
{
	class HUDSCCMTestScorePanel : HUDRoundedPanel
	{
		public const float WIDTH = 11 * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 7 * GDConstants.TILE_WIDTH;

		public const float FOOTER_HEIGHT = 2 * GDConstants.TILE_WIDTH;
		public const float FOOTER_COLBAR_HEIGHT = GDConstants.TILE_WIDTH / 4f;
		public const float ICON_MARGIN = GDConstants.TILE_WIDTH * (3/8f);
		public const float ICON_SIZE = GDConstants.TILE_WIDTH * 2;

		private GDGameScreen GDScreen => (GDGameScreen)HUD.Screen;

		public override int Depth => 0;

		private readonly LevelBlueprint Level;
		private readonly SCCMLevelData SCCMData;
		private readonly FractionDifficulty Diff;
		private readonly GameSpeedModes Speed;
		private readonly int Time;

		public HUDSCCMTestScorePanel(LevelBlueprint lvl, SCCMLevelData dat, FractionDifficulty d, GameSpeedModes s, int t)
		{
			Level = lvl;
			SCCMData = dat;
			Diff = d;
			Speed = s;
			Time = t;

			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			#region Footer

			AddElement(new HUDRectangle(0)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				Size = new FSize(WIDTH, FOOTER_HEIGHT - 10),

				Definition = HUDBackgroundDefinition.CreateRounded(FlatColors.BackgroundHUD2, 16, false, false, true, true),
			});


			AddElement(new HUDRectangle(2)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0, FOOTER_HEIGHT - FOOTER_COLBAR_HEIGHT),
				Size = new FSize(WIDTH / 3f, FOOTER_COLBAR_HEIGHT),

				Definition = HUDBackgroundDefinition.CreateSimple(FlatColors.Nephritis),
			});

			AddElement(new HUDRectangle(1)
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, FOOTER_HEIGHT - FOOTER_COLBAR_HEIGHT),
				Size = new FSize(WIDTH / 2f, FOOTER_COLBAR_HEIGHT),

				Definition = HUDBackgroundDefinition.CreateSimple(FlatColors.PeterRiver),
			});

			AddElement(new HUDRectangle(2)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(0, FOOTER_HEIGHT - FOOTER_COLBAR_HEIGHT),
				Size = new FSize(WIDTH / 3f, FOOTER_COLBAR_HEIGHT),

				Definition = HUDBackgroundDefinition.CreateSimple(FlatColors.Pomegranate),
			});

			AddElement(new HUDSeperator(HUDOrientation.Vertical, 3)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(WIDTH / 3f, 0),
				Size = new FSize(1, FOOTER_HEIGHT),

				Color = FlatColors.SeperatorHUD,
			});

			AddElement(new HUDSeperator(HUDOrientation.Vertical, 3)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(WIDTH / 3f, 0),
				Size = new FSize(1, FOOTER_HEIGHT),

				Color = FlatColors.SeperatorHUD,
			});

			AddElement(new HUDLabel(2)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(0, 77),
				Size = new FSize(WIDTH / 3f, 40),

				TextAlignment = HUDAlignment.BOTTOMCENTER,
				L10NText = L10NImpl.STR_HSP_TIME_YOU,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontRegular,
				FontSize = 35,
			});

			AddElement(new HUDLabel(2)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(0, 15),
				Size = new FSize(WIDTH / 3f, 60),

				Text = TimeExtension.FormatMilliseconds(Time, false),
				TextAlignment = HUDAlignment.BOTTOMCENTER,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontBold,
				FontSize = 57,
			});

			#endregion

			#region Buttons

			AddElement(new HUDIconTextButton(2)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(24, FOOTER_HEIGHT + 24),
				Size = new FSize(3.5f * GDConstants.TILE_WIDTH, 60),

				L10NText = L10NImpl.STR_HSP_BACK,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				Icon = Textures.TexIconBack,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonHUD, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonPressedHUD, 16),

				Click = (s, a) => { MainGame.Inst.SetLevelEditorScreen(SCCMData); },
			});

			var w = L10N.LANGUAGE == L10N.LANG_EN_US ? 3.5f : 5.0f;
			AddElement(new HUDIconTextButton(2)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(24, FOOTER_HEIGHT + 24),
				Size = new FSize(w * GDConstants.TILE_WIDTH, 60),

				L10NText = L10NImpl.STR_HSP_AGAIN,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				Icon = Textures.TexIconRedo,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.Orange, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.SunFlower, 16),

				Click = (s, a) => ((GDGameScreen)HUD.Screen).RestartLevel(false),
			});

			#endregion

			#region Icons

			AddElement(new HUDDifficultyButton(2, FractionDifficulty.DIFF_0, Diff==FractionDifficulty.DIFF_0 ? HUDDifficultyButton.HUDDifficultyButtonMode.ACTIVATED : HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED, () => StartWithDiff(FractionDifficulty.DIFF_0))
			{
				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(1 * ICON_MARGIN + 0 * ICON_SIZE, ICON_MARGIN),
			});

			AddElement(new HUDDifficultyButton(2, FractionDifficulty.DIFF_1, Diff == FractionDifficulty.DIFF_1 ? HUDDifficultyButton.HUDDifficultyButtonMode.ACTIVATED : HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED, () => StartWithDiff(FractionDifficulty.DIFF_1))
			{
				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(3 * ICON_MARGIN + 1 * ICON_SIZE, ICON_MARGIN),
			});

			AddElement(new HUDDifficultyButton(2, FractionDifficulty.DIFF_2, Diff == FractionDifficulty.DIFF_2 ? HUDDifficultyButton.HUDDifficultyButtonMode.ACTIVATED : HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED, () => StartWithDiff(FractionDifficulty.DIFF_2))
			{
				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(5 * ICON_MARGIN + 2 * ICON_SIZE, ICON_MARGIN),
			});

			AddElement(new HUDDifficultyButton(2, FractionDifficulty.DIFF_3, Diff == FractionDifficulty.DIFF_3 ? HUDDifficultyButton.HUDDifficultyButtonMode.ACTIVATED : HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED, () => StartWithDiff(FractionDifficulty.DIFF_3))
			{
				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(7 * ICON_MARGIN + 3 * ICON_SIZE, ICON_MARGIN),
			});

			#endregion
		}

		private void StartWithDiff(FractionDifficulty d)
		{
			MainGame.Inst.SetEditorTestLevel(Level, d, SCCMData, Speed);
		}
	}
}
