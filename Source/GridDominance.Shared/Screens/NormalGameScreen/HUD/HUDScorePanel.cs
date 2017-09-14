using System;
using System.Collections.Generic;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.NormalGameScreen.HUD.Operations;
using GridDominance.Shared.Screens.ScreenGame;
using GridDominance.Shared.Screens.WorldMapScreen;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.Entities.Operation;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
{
	class HUDScorePanel : HUDRoundedPanel
	{
		public const float WIDTH = 11 * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 7 * GDConstants.TILE_WIDTH;

		public const float FOOTER_HEIGHT = 2 * GDConstants.TILE_WIDTH;
		public const float FOOTER_COLBAR_HEIGHT = GDConstants.TILE_WIDTH / 4f;
		public const float ICON_MARGIN = GDConstants.TILE_WIDTH * (3/8f);
		public const float ICON_SIZE = GDConstants.TILE_WIDTH * 2;

		private GDGameScreen_SP GDScreen => (GDGameScreen_SP)HUD.Screen;

		public override int Depth => 0;

		private readonly HashSet<FractionDifficulty> _gainLevel;
		private readonly bool _successScreen;
		private readonly PlayerProfile _profile;
		private readonly LevelBlueprint _level;
		private readonly int _increasePoints;
		private readonly FractionDifficulty _levelDifficulty;
		private readonly int _leveltime;

		public HUDLabel LabelTime1;
		public HUDLabel LabelTime2;

		public HUDScorePanel(LevelBlueprint lvl, PlayerProfile playerprofile, HashSet<FractionDifficulty> newDifficulties, FractionDifficulty d, bool playerHasWon, int pointInc, int time)
		{
			_gainLevel = newDifficulties;
			_successScreen = playerHasWon;
			_profile = playerprofile;
			_increasePoints = pointInc;
			_level = lvl;
			_levelDifficulty = d;
			_leveltime = time;

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
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0, 77),
				Size = new FSize(WIDTH / 3f, 40),

				TextAlignment = HUDAlignment.BOTTOMCENTER,
				L10NText = L10NImpl.STR_HSP_LEVEL,
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

			AddElement(LabelTime1 = new HUDLabel(2)
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

				Text = _level.Name,
				TextAlignment = HUDAlignment.BOTTOMCENTER,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontBold,
				FontSize = 57,
			});

			AddElement(new HUDIncrementIndicatorLabel(_profile.TotalPoints.ToString(), _increasePoints == 0 ? "" : "+" + _increasePoints, 2)
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, 15),
				Size = new FSize(WIDTH / 3f, 60),

				TextAlignment = HUDAlignment.BOTTOMCENTER,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontBold,
				FontSize = 57,
			});

			AddElement(LabelTime2 = new HUDLabel(2)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(0, 15),
				Size = new FSize(WIDTH / 3f, 60),

				Text = TimeExtension.FormatMilliseconds(_leveltime, false),
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

				Click = (s, a) => GDScreen.ExitToMap(false),
			});

			if (_successScreen)
			{
				var next = GetNextNode();

				if (next != null)
				{
					AddElement(new HUDIconTextButton(2)
					{
						Alignment = HUDAlignment.BOTTOMRIGHT,
						RelativePosition = new FPoint(24, FOOTER_HEIGHT + 24),
						Size = new FSize(3.5f * GDConstants.TILE_WIDTH, 60),

						L10NText = L10NImpl.STR_HSP_NEXT,
						TextColor = Color.White,
						Font = Textures.HUDFontRegular,
						FontSize = 55,
						TextAlignment = HUDAlignment.CENTER,
						TextPadding = 8,
						Icon = Textures.TexIconNext,

						BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.Nephritis, 16),
						BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.Emerald, 16),

						Click = (s, a) => MainGame.Inst.SetLevelScreen(next.Item1, next.Item2, GDScreen.WorldBlueprint),
					});
				}
			}
			else
			{
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
			}

			#endregion

			#region Icons

			var finDiff0 = _profile.GetLevelData(this.GDHUD().GDOwner.Blueprint.UniqueID).HasCompletedOrBetter(FractionDifficulty.KI_EASY);
			var finDiff1 = _profile.GetLevelData(this.GDHUD().GDOwner.Blueprint.UniqueID).HasCompletedOrBetter(FractionDifficulty.KI_NORMAL);
			var finDiff2 = _profile.GetLevelData(this.GDHUD().GDOwner.Blueprint.UniqueID).HasCompletedOrBetter(FractionDifficulty.KI_HARD);
			var finDiff3 = _profile.GetLevelData(this.GDHUD().GDOwner.Blueprint.UniqueID).HasCompletedOrBetter(FractionDifficulty.KI_IMPOSSIBLE);


			var modeDiff0 =
				finDiff0 ?
					(_gainLevel.Contains(FractionDifficulty.KI_EASY) ?
						HUDDifficultyButton.HUDDifficultyButtonMode.UNLOCKANIMATION :
						HUDDifficultyButton.HUDDifficultyButtonMode.ACTIVATED) :
					HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED;

			var modeDiff1 =
				finDiff1 ?
					(_gainLevel.Contains(FractionDifficulty.KI_NORMAL) ?
						HUDDifficultyButton.HUDDifficultyButtonMode.UNLOCKANIMATION :
						HUDDifficultyButton.HUDDifficultyButtonMode.ACTIVATED) :
					HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED;

			var modeDiff2 =
				finDiff2 ?
					(_gainLevel.Contains(FractionDifficulty.KI_HARD) ?
						HUDDifficultyButton.HUDDifficultyButtonMode.UNLOCKANIMATION :
						HUDDifficultyButton.HUDDifficultyButtonMode.ACTIVATED) :
					HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED;

			var modeDiff3 =
				finDiff3 ?
					(_gainLevel.Contains(FractionDifficulty.KI_IMPOSSIBLE) ?
						HUDDifficultyButton.HUDDifficultyButtonMode.UNLOCKANIMATION :
						HUDDifficultyButton.HUDDifficultyButtonMode.ACTIVATED) :
					HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED;

			AddElement(new HUDDifficultyButton(2, FractionDifficulty.KI_EASY, modeDiff0)
			{
				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(1 * ICON_MARGIN + 0 * ICON_SIZE, ICON_MARGIN)
			});

			AddElement(new HUDDifficultyButton(2, FractionDifficulty.KI_NORMAL, modeDiff1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(3 * ICON_MARGIN + 1 * ICON_SIZE, ICON_MARGIN)
			});

			AddElement(new HUDDifficultyButton(2, FractionDifficulty.KI_HARD, modeDiff2)
			{
				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(5 * ICON_MARGIN + 2 * ICON_SIZE, ICON_MARGIN)
			});

			AddElement(new HUDDifficultyButton(2, FractionDifficulty.KI_IMPOSSIBLE, modeDiff3)
			{
				Alignment = HUDAlignment.TOPLEFT,
				Size = new FSize(ICON_SIZE, ICON_SIZE),
				RelativePosition = new FPoint(7 * ICON_MARGIN + 3 * ICON_SIZE, ICON_MARGIN)
			});

			#endregion

			if (_successScreen)
			{
				AddHUDOperation(new HUDScorePanelTimeDisplayOperation(
					L10NImpl.STR_HSP_TIME_NOW, TimeExtension.FormatMilliseconds(_leveltime, false),
					L10NImpl.STR_HSP_TIME_BEST, _profile.GetLevelData(_level.UniqueID).GetTimeString(_levelDifficulty, false)));
			}
			else
			{
				LabelTime1.L10NText = L10NImpl.STR_HSP_TIME_BEST;
				LabelTime2.Text = _profile.GetLevelData(_level.UniqueID).GetTimeString(_levelDifficulty, false);
			}
		}

		private Tuple<LevelBlueprint, FractionDifficulty> GetNextNode()
		{
			var x = BlueprintAnalyzer.FindNextNode(GDScreen.WorldBlueprint, GDScreen.Blueprint.UniqueID, GDScreen.Difficulty);
			if (x == null) return null;
			
			return Tuple.Create(Levels.LEVELS[x.Value.LevelID], GDScreen.Difficulty);
		}
	}
}
