using System.Threading.Tasks;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common.HUD.Elements;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.NormalGameScreen.HUD.Elements;
using GridDominance.Shared.Screens.NormalGameScreen.HUD.Operations;
using GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM;
using GridDominance.Shared.SCCM;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD.ScorePanel
{
	class HUDSCCMScorePanel_Won : HUDRoundedPanel
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
		public readonly SCCMLevelMeta  LevelMeta;
		public readonly int?[]  PersonalTimes;
		public readonly FractionDifficulty LevelDifficulty;
		public readonly int LevelTime;
		private readonly int _scoreGain;
		
		public FractionDifficulty SelectedDifficulty;

		public HUDLabel LabelTimeHeader;
		public HUDLabel LabelTimeValue;
		private EnhancedHUDDifficultyButton _diffButton0;
		private EnhancedHUDDifficultyButton _diffButton1;
		private EnhancedHUDDifficultyButton _diffButton2;
		private EnhancedHUDDifficultyButton _diffButton3;
		private HUDIconTextButton _btnReplay;
		private HUDEllipseImageButton _btnStar;
		private HUDLabel _lblStarCount;

		public int TimeDisplayMode = 1; // (PB, WR, CURR)

		public HUDSCCMScorePanel_Won(LevelBlueprint lvl, SCCMLevelMeta meta, FractionDifficulty d, int scoreGain, int time)
		{
			_level = lvl;
			LevelMeta = meta;
			LevelDifficulty = d;
			SelectedDifficulty = d;
			_scoreGain = scoreGain;
			PersonalTimes = MainGame.Inst.Profile.GetCustomLevelTimes(meta.OnlineID);
			LevelTime=time;

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
				Text = LevelMeta.LevelName,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontBold,
				FontSize = 48,
			});

			#endregion
			
			#region Icons

			AddElement(_diffButton0 = new EnhancedHUDDifficultyButton(2, FractionDifficulty.DIFF_0, () => SelectDiff(FractionDifficulty.DIFF_0))
			{
				Active = MainGame.Inst.Profile.HasCustomLevelBeaten(LevelMeta.OnlineID, FractionDifficulty.DIFF_0),
				Selected = FractionDifficulty.DIFF_0 == SelectedDifficulty,

				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(1 * ICON_MARGIN + 0 * ICON_SIZE, ICON_MARGIN + 32)
			});

			AddElement(_diffButton1 = new EnhancedHUDDifficultyButton(2, FractionDifficulty.DIFF_1, () => SelectDiff(FractionDifficulty.DIFF_1))
			{
				Active = MainGame.Inst.Profile.HasCustomLevelBeaten(LevelMeta.OnlineID, FractionDifficulty.DIFF_1),
				Selected = FractionDifficulty.DIFF_1 == SelectedDifficulty,

				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(3 * ICON_MARGIN + 1 * ICON_SIZE, ICON_MARGIN + 32)
			});

			AddElement(_diffButton2 = new EnhancedHUDDifficultyButton(2, FractionDifficulty.DIFF_2, () => SelectDiff(FractionDifficulty.DIFF_2))
			{
				Active = MainGame.Inst.Profile.HasCustomLevelBeaten(LevelMeta.OnlineID, FractionDifficulty.DIFF_2),
				Selected = FractionDifficulty.DIFF_2 == SelectedDifficulty,

				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(5 * ICON_MARGIN + 2 * ICON_SIZE, ICON_MARGIN + 32)
			});

			AddElement(_diffButton3 = new EnhancedHUDDifficultyButton(2, FractionDifficulty.DIFF_3, () => SelectDiff(FractionDifficulty.DIFF_3))
			{
				Active = MainGame.Inst.Profile.HasCustomLevelBeaten(LevelMeta.OnlineID, FractionDifficulty.DIFF_3),
				Selected = FractionDifficulty.DIFF_3 == SelectedDifficulty,

				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(7 * ICON_MARGIN + 3 * ICON_SIZE, ICON_MARGIN + 32)
			});

			#endregion
			
			#region Buttons

			AddElement(new HUDIconTextButton(2)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(24, FOOTER_HEIGHT + 24),
				Size = new FSize(3.5f * TW, 60),

				L10NText = L10NImpl.STR_HSP_BACK,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				Icon = Textures.TexIconBack,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonHUD, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonPressedHUD, 16),

				Click = (s, a) => MainGame.Inst.SetOverworldScreenWithSCCM(SCCMMainPanel.SCCMTab.Hot),
			});
			
			AddElement(_btnReplay = new HUDIconTextButton(2)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(24, FOOTER_HEIGHT + 24),
				Size = new FSize(3.5f * TW, 60),

				L10NText = L10NImpl.STR_HSP_AGAIN,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				Icon = Textures.TexIconRedo,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.Nephritis, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.Emerald, 16),

				Click = (s, a) => Replay(SelectedDifficulty),
			});
			
			#endregion

			#region Star
			
			AddElement(_btnStar = new HUDEllipseImageButton
			{
				Alignment = HUDAlignment.CENTER,
				RelativePosition = new FPoint(0, 4.25f),
				Size = new FSize(3f*TW, 3f*TW),

				Image = Textures.TexIconStar,
				ImageColor = (MainGame.Inst.Profile.HasCustomLevelStarred(LevelMeta.OnlineID) || LevelMeta.UserID == MainGame.Inst.Profile.OnlineUserID) ? FlatColors.SunFlower : FlatColors.BackgroundHUD,
				ImagePadding = 16f,

				BackgroundNormal = FlatColors.ButtonHUD,
				BackgroundPressed = FlatColors.ButtonPressedHUD,
				
				IsEnabled = (LevelMeta.UserID != MainGame.Inst.Profile.OnlineUserID),
				Click = (s, a) => ToggleStar(),
			});
			
			AddElement(_lblStarCount = new HUDLabel(4)
			{
				Alignment = HUDAlignment.CENTER,
				RelativePosition = new FPoint(0, 4),
				Size = new FSize(4*TW, 4*TW),

				TextAlignment = HUDAlignment.CENTER,
				Text = LevelMeta.Stars.ToString(),
				TextColor = FlatColors.Foreground,
				Font = Textures.HUDFontBold,
				FontSize = 48,
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
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0, 77),
				Size = new FSize(WIDTH / 3f, 40),

				TextAlignment = HUDAlignment.BOTTOMCENTER,
				L10NText = L10NImpl.STR_HSP_AUTHOR,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontRegular,
				FontSize = 35,
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

			AddElement(LabelTimeHeader = new HUDLabel(2)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(0, 77),
				Size = new FSize(WIDTH / 3f, 40),

				TextAlignment = HUDAlignment.BOTTOMCENTER,
				L10NText = L10NImpl.STR_HSP_TIME_NOW,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontRegular,
				FontSize = 35,
			});

			AddElement(new HUDLabel(2)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0, 15),
				Size = new FSize(WIDTH / 3f, 60),

				Text = LevelMeta.Username ?? "Unknown",
				WordWrap = HUDWordWrap.Ellipsis,
				TextAlignment = HUDAlignment.BOTTOMCENTER,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontBold,
				FontSize = 57,

				AutoFontSizeShrink = true,
			});

			AddElement(new HUDIncrementIndicatorLabel(MainGame.Inst.Profile.ScoreSCCM.ToString(), _scoreGain == 0 ? "" : "+" + _scoreGain, 2)
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, 15),
				Size = new FSize(WIDTH / 3f, 60),

				TextAlignment = HUDAlignment.BOTTOMCENTER,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontBold,
				FontSize = 57,
			});

			AddElement(LabelTimeValue = new HUDLabel(2)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(0, 15),
				Size = new FSize(WIDTH / 3f, 60),

				Text = TimeExtension.FormatMilliseconds(PersonalTimes[(int)SelectedDifficulty] ?? -1, false),
				TextAlignment = HUDAlignment.BOTTOMCENTER,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontBold,
				FontSize = 57,
			});

			#endregion

			AddOperation(new HUDSCCMScorePanelTimeDisplayOperation());
			UpdateTDMLabels();
		}

		private void Replay(FractionDifficulty d)
		{
			((GDGameScreen)HUD.Screen).ReplayLevel(d);
		}

		private void ToggleStar()
		{
			if (_btnStar.Image == Textures.CannonCogBig) return;
			if (LevelMeta.UserID == MainGame.Inst.Profile.OnlineUserID) return;

			_btnStar.Image = Textures.CannonCogBig;
			_btnStar.ImageColor = FlatColors.BackgroundHUD;
			_btnStar.ImageRotation = 0f;
			_btnStar.ImageRotationSpeed = 0.5f;
			
			DoToggleStar(!MainGame.Inst.Profile.HasCustomLevelStarred(LevelMeta.OnlineID)).RunAsync();
		}

		private async Task DoToggleStar(bool v)
		{
			// [Request] -> [Set icon to spinner] -> [Response] -> [Save Profile] -> [update icon]

			var r = await MainGame.Inst.Backend.SetCustomLevelStarred(MainGame.Inst.Profile, LevelMeta.OnlineID, v);

			MainGame.Inst.DispatchBeginInvoke(() => 
			{
				if (r != null)
				{
					_btnStar.Image = Textures.TexIconStar;
					_btnStar.ImageColor = (r.Item2) ? FlatColors.SunFlower : FlatColors.BackgroundHUD;
					_btnStar.ImageRotation = 0f;
					_btnStar.ImageRotationSpeed = 0f;

					_lblStarCount.Text = r.Item1.ToString();
				}
				else
				{
					_btnStar.Image = Textures.TexIconStar;
					_btnStar.ImageColor = MainGame.Inst.Profile.HasCustomLevelStarred(LevelMeta.OnlineID) ? FlatColors.SunFlower : FlatColors.BackgroundHUD;
					_btnStar.ImageRotation = 0f;
					_btnStar.ImageRotationSpeed = 0f;

					_lblStarCount.Text = (LevelMeta==null) ? "???" : LevelMeta.Stars.ToString();
				}
			});
		}

		private void SelectDiff(FractionDifficulty d)
		{
			SelectedDifficulty = d;
			if (SelectedDifficulty == LevelDifficulty && TimeDisplayMode==2) TimeDisplayMode=0;
			UpdateTDMLabels();

			_diffButton0.Selected = (FractionDifficulty.DIFF_0 == SelectedDifficulty);
			_diffButton1.Selected = (FractionDifficulty.DIFF_1 == SelectedDifficulty);
			_diffButton2.Selected = (FractionDifficulty.DIFF_2 == SelectedDifficulty);
			_diffButton3.Selected = (FractionDifficulty.DIFF_3 == SelectedDifficulty);
			
			_btnReplay.L10NText = (LevelDifficulty == SelectedDifficulty) ? L10NImpl.STR_HSP_AGAIN :  L10NImpl.STR_LVLED_BTN_PLAY;
			_btnReplay.Icon = (LevelDifficulty == SelectedDifficulty) ? Textures.TexIconRedo : Textures.TexHUDIconPlay;
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			//
		}

		public void UpdateTDMLabels()
		{
			if (TimeDisplayMode==0) // PB
			{
				LabelTimeHeader.L10NText = L10NImpl.STR_HSP_TIME_YOU;
				LabelTimeValue.Text = TimeExtension.FormatMilliseconds(PersonalTimes[(int)SelectedDifficulty] ?? -1, false);
			}
			else if (TimeDisplayMode==1) // WR
			{
				LabelTimeHeader.L10NText = L10NImpl.STR_INF_HIGHSCORE;
				LabelTimeValue.Text = TimeExtension.FormatMilliseconds(LevelMeta.Highscores[(int)SelectedDifficulty].HighscoreTime ?? -1, false);
			}
			else if (TimeDisplayMode==2) // Curr
			{
				LabelTimeHeader.L10NText = L10NImpl.STR_HSP_TIME_NOW;
				LabelTimeValue.Text = TimeExtension.FormatMilliseconds(LevelTime, false);
			}
		}
	}
}
