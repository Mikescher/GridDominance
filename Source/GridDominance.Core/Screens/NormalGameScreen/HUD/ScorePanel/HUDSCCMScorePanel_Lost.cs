using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.NormalGameScreen.HUD.Operations;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
{
	class HUDSCCMScorePanel_Lost : HUDRoundedPanel
	{
		public const float TW = GDConstants.TILE_WIDTH;

		public const float WIDTH = 11    * TW;
		public const float HEIGHT = 8.5f * TW;

		public const float FOOTER_HEIGHT = 2 * TW;
		public const float FOOTER_COLBAR_HEIGHT = TW / 4f;
		public const float ICON_MARGIN = TW * (3/8f);
		public const float ICON_SIZE = TW * 2;

		private GDGameScreen_SP GDScreen => (GDGameScreen_SP)HUD.Screen;

		public override int Depth => 0;

		private readonly LevelBlueprint _level;
		public FractionDifficulty SelectedDifficulty;
		
		private EnhancedHUDDifficultyButton _diffButton0;
		private EnhancedHUDDifficultyButton _diffButton1;
		private EnhancedHUDDifficultyButton _diffButton2;
		private EnhancedHUDDifficultyButton _diffButton3;

		public HUDSCCMScorePanel_Lost(LevelBlueprint lvl, FractionDifficulty d)
		{
			_level = lvl;
			SelectedDifficulty = d;

			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			#region Header
			
			AddElement(new HUDLabel
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(WIDTH, 48),

				TextAlignment = HUDAlignment.CENTER,
				Text = _level.FullName,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontBold,
				FontSize = 48,
			});

			#endregion
			
			#region Icons

			AddElement(_diffButton0 = new EnhancedHUDDifficultyButton(2, FractionDifficulty.DIFF_0, () => SelectDiff(FractionDifficulty.DIFF_0))
			{
				Active = MainGame.Inst.Profile.HasCustomLevelBeaten(_level.CustomMeta_LevelID, FractionDifficulty.DIFF_0),
				Selected = FractionDifficulty.DIFF_0 == SelectedDifficulty,

				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(1 * ICON_MARGIN + 0 * ICON_SIZE, ICON_MARGIN + 32)
			});

			AddElement(_diffButton1 = new EnhancedHUDDifficultyButton(2, FractionDifficulty.DIFF_1, () => SelectDiff(FractionDifficulty.DIFF_1))
			{
				Active = MainGame.Inst.Profile.HasCustomLevelBeaten(_level.CustomMeta_LevelID, FractionDifficulty.DIFF_1),
				Selected = FractionDifficulty.DIFF_1 == SelectedDifficulty,

				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(3 * ICON_MARGIN + 1 * ICON_SIZE, ICON_MARGIN + 32)
			});

			AddElement(_diffButton2 = new EnhancedHUDDifficultyButton(2, FractionDifficulty.DIFF_2, () => SelectDiff(FractionDifficulty.DIFF_2))
			{
				Active = MainGame.Inst.Profile.HasCustomLevelBeaten(_level.CustomMeta_LevelID, FractionDifficulty.DIFF_2),
				Selected = FractionDifficulty.DIFF_2 == SelectedDifficulty,

				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(5 * ICON_MARGIN + 2 * ICON_SIZE, ICON_MARGIN + 32)
			});

			AddElement(_diffButton3 = new EnhancedHUDDifficultyButton(2, FractionDifficulty.DIFF_3, () => SelectDiff(FractionDifficulty.DIFF_3))
			{
				Active = MainGame.Inst.Profile.HasCustomLevelBeaten(_level.CustomMeta_LevelID, FractionDifficulty.DIFF_3),
				Selected = FractionDifficulty.DIFF_3 == SelectedDifficulty,

				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(7 * ICON_MARGIN + 3 * ICON_SIZE, ICON_MARGIN + 32)
			});

			#endregion
			
			#region Buttons

			AddElement(new HUDIconTextButton(2)
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, 3.50f * TW),
				Size = new FSize(3.5f * TW, 60),

				L10NText = L10NImpl.STR_HSP_AGAIN,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				Icon = Textures.TexIconBack,
				
				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.Orange, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.SunFlower, 16),

				Click = (s, a) => Replay(SelectedDifficulty),
			});
			
			#endregion

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
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, 77),
				Size = new FSize(WIDTH / 3f, 40),

				TextAlignment = HUDAlignment.BOTTOMCENTER,
				L10NText = L10NImpl.STR_HSP_POINTS,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontRegular,
				FontSize = 35,
			});

			AddElement(new HUDIncrementIndicatorLabel(MainGame.Inst.Profile.ScoreSCCM.ToString(), "", 2)
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, 16),
				Size = new FSize(WIDTH / 3f, 60),

				TextAlignment = HUDAlignment.BOTTOMCENTER,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontBold,
				FontSize = 57,
			});

			#endregion
		}

		private void Replay(FractionDifficulty d)
		{
			((GDGameScreen)HUD.Screen).ReplayLevel(d);
		}

		private void SelectDiff(FractionDifficulty d)
		{
			SelectedDifficulty = d;

			_diffButton0.Selected = (FractionDifficulty.DIFF_0 == SelectedDifficulty);
			_diffButton1.Selected = (FractionDifficulty.DIFF_1 == SelectedDifficulty);
			_diffButton2.Selected = (FractionDifficulty.DIFF_2 == SelectedDifficulty);
			_diffButton3.Selected = (FractionDifficulty.DIFF_3 == SelectedDifficulty);
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			//
		}
	}
}
