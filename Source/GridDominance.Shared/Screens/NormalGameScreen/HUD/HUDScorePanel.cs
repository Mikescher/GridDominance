using System;
using System.Linq;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using GridDominance.Shared.Screens.WorldMapScreen;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using MonoSAMFramework.Portable.Localization;

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

		private readonly FractionDifficulty? gainLevel;
		private readonly bool successScreen;
		private readonly PlayerProfile profile;
		private readonly LevelBlueprint Level;
		private readonly int increasePoints;
		
		public HUDScorePanel(LevelBlueprint lvl, PlayerProfile playerprofile, FractionDifficulty? newDifficulty, bool playerHasWon, int pointInc)
		{
			gainLevel = newDifficulty;
			successScreen = playerHasWon;
			profile = playerprofile;
			increasePoints = pointInc;
			Level = lvl;

			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			#region Footer

			AddElement(new HUDRoundedRectangle(0)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				Size = new FSize(WIDTH, FOOTER_HEIGHT - 10),

				Color = FlatColors.BackgroundHUD2,
				RoundCornerTL = false,
				RoundCornerTR = false,
				RoundCornerBL = true,
				RoundCornerBR = true,
			});


			AddElement(new HUDRectangle(2)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0, FOOTER_HEIGHT - FOOTER_COLBAR_HEIGHT),
				Size = new FSize(WIDTH / 3f, FOOTER_COLBAR_HEIGHT),

				Color = FlatColors.Nephritis,
			});

			AddElement(new HUDRectangle(1)
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, FOOTER_HEIGHT - FOOTER_COLBAR_HEIGHT),
				Size = new FSize(WIDTH / 2f, FOOTER_COLBAR_HEIGHT),

				Color = FlatColors.PeterRiver,
			});

			AddElement(new HUDRectangle(2)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(0, FOOTER_HEIGHT - FOOTER_COLBAR_HEIGHT),
				Size = new FSize(WIDTH / 3f, FOOTER_COLBAR_HEIGHT),

				Color = FlatColors.Pomegranate,
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

			AddElement(new HUDLabel(2)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(0, 77),
				Size = new FSize(WIDTH / 3f, 40),

				TextAlignment = HUDAlignment.BOTTOMCENTER,
				L10NText = L10NImpl.STR_HSP_PROGRESS,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontRegular,
				FontSize = 35,
			});

			AddElement(new HUDLabel(2)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0, 15),
				Size = new FSize(WIDTH / 3f, 60),

				Text = Level.Name,
				TextAlignment = HUDAlignment.BOTTOMCENTER,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontBold,
				FontSize = 57,
			});

			AddElement(new HUDIncrementIndicatorLabel(profile.TotalPoints.ToString(), increasePoints == 0 ? "" : "+" + increasePoints, 2)
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, 15),
				Size = new FSize(WIDTH / 3f, 60),

				TextAlignment = HUDAlignment.BOTTOMCENTER,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontBold,
				FontSize = 57,
			});

			AddElement(new HUDLabel(2)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(0, 15),
				Size = new FSize(WIDTH / 3f, 60),

				Text = profile.GetLevelData(Level.UniqueID).CompletionCount + " / 4",
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
				BackgoundType = HUDBackgroundType.Rounded,
				Color = FlatColors.ButtonHUD,
				ColorPressed = FlatColors.ButtonPressedHUD,

				Click = (s, a) => GDScreen.ExitToMap(),
			});

			if (successScreen)
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
						BackgoundType = HUDBackgroundType.Rounded,
						Color = FlatColors.Nephritis,
						ColorPressed = FlatColors.Emerald,

						Click = (s, a) => MainGame.Inst.SetLevelScreen(next.Item1, next.Item2, GDScreen.WorldBlueprint),
					});
				}
			}
			else
			{
				AddElement(new HUDIconTextButton(2)
				{
					Alignment = HUDAlignment.BOTTOMRIGHT,
					RelativePosition = new FPoint(24, FOOTER_HEIGHT + 24),
					Size = new FSize(3.5f * GDConstants.TILE_WIDTH, 60),

					L10NText = L10NImpl.STR_HSP_AGAIN,
					TextColor = Color.White,
					Font = Textures.HUDFontRegular,
					FontSize = 55,
					TextAlignment = HUDAlignment.CENTER,
					TextPadding = 8,
					Icon = Textures.TexIconRedo,
					BackgoundType = HUDBackgroundType.Rounded,
					Color = FlatColors.Orange,
					ColorPressed = FlatColors.SunFlower,

					Click = (s, a) => ((GDGameScreen)HUD.Screen).RestartLevel(),
				});
			}

			#endregion

			#region Icons

			var finDiff0 = profile.GetLevelData(this.GDHUD().GDOwner.Blueprint.UniqueID).HasCompleted(FractionDifficulty.KI_EASY);
			var finDiff1 = profile.GetLevelData(this.GDHUD().GDOwner.Blueprint.UniqueID).HasCompleted(FractionDifficulty.KI_NORMAL);
			var finDiff2 = profile.GetLevelData(this.GDHUD().GDOwner.Blueprint.UniqueID).HasCompleted(FractionDifficulty.KI_HARD);
			var finDiff3 = profile.GetLevelData(this.GDHUD().GDOwner.Blueprint.UniqueID).HasCompleted(FractionDifficulty.KI_IMPOSSIBLE);


			var modeDiff0 =
				finDiff0 ?
					(gainLevel == FractionDifficulty.KI_EASY ?
						HUDDifficultyButton.HUDDifficultyButtonMode.UNLOCKANIMATION :
						HUDDifficultyButton.HUDDifficultyButtonMode.ACTIVATED) :
					HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED;

			var modeDiff1 =
				finDiff1 ?
					(gainLevel == FractionDifficulty.KI_NORMAL ?
						HUDDifficultyButton.HUDDifficultyButtonMode.UNLOCKANIMATION :
						HUDDifficultyButton.HUDDifficultyButtonMode.ACTIVATED) :
					HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED;

			var modeDiff2 =
				finDiff2 ?
					(gainLevel == FractionDifficulty.KI_HARD ?
						HUDDifficultyButton.HUDDifficultyButtonMode.UNLOCKANIMATION :
						HUDDifficultyButton.HUDDifficultyButtonMode.ACTIVATED) :
					HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED;

			var modeDiff3 =
				finDiff3 ?
					(gainLevel == FractionDifficulty.KI_IMPOSSIBLE ?
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
		}

		private Tuple<LevelBlueprint, FractionDifficulty> GetNextNode()
		{
			var x = BlueprintAnalyzer.FindNextNode(GDScreen.WorldBlueprint, GDScreen.Blueprint.UniqueID, GDScreen.Difficulty);
			if (x == null) return null;
			
			return Tuple.Create(Levels.LEVELS[x.Value.LevelID], GDScreen.Difficulty);
		}
	}
}
