using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.ScreenGame.HUD
{
	// TODO This should eytend some kind of (modal) DialogContainer
	// That spans an invisible root around the whole screen and swallows all mouse events (or simply swallows all by other means)
	// Cause currently the pause button is clickable while the scorescreen is open
	class HUDScorePanel : HUDRoundedPanel
	{
		public const float WIDTH = 11 * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 7 * GDConstants.TILE_WIDTH;

		public const float FOOTER_HEIGHT = 2 * GDConstants.TILE_WIDTH;
		public const float FOOTER_COLBAR_HEIGHT = GDConstants.TILE_WIDTH / 4f;
		public const float ICON_MARGIN = GDConstants.TILE_WIDTH * (3/8f);
		public const float ICON_SIZE = GDConstants.TILE_WIDTH * 2;

		public override int Depth => 0;

		private readonly FractionDifficulty? gainLevel;
		private readonly bool successScreen;
		private readonly PlayerProfile.PlayerProfile profile;

		private HUDLabel lblPoints;
		private HUDIconTextButton btnMenu;
		private HUDIconTextButton btnNext;

		public HUDScorePanel(PlayerProfile.PlayerProfile playerprofile, FractionDifficulty? newDifficulty, bool playerHasWon)
		{
			gainLevel = newDifficulty;
			successScreen = playerHasWon;
			profile = playerprofile;

			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		protected override bool isClickable() => false;

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
				Text = "Level",
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontRegular,
				FontSize = 35,
			});

			AddElement(lblPoints = new HUDLabel(2)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0, 15),
				Size = new FSize(WIDTH / 3f, 60),

				Text = "4 - 4", // TODO
				TextAlignment = HUDAlignment.BOTTOMCENTER,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontBold,
				FontSize = 57,
			});

			AddElement(new HUDLabel(2)
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, 77),
				Size = new FSize(WIDTH / 3f, 40),

				TextAlignment = HUDAlignment.BOTTOMCENTER,
				Text = "Points",
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontRegular,
				FontSize = 35,
			});

			AddElement(lblPoints = new HUDIncrementIndicatorLabel("236", "+20", 2) // TODO
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
				RelativePosition = new FPoint(0, 77),
				Size = new FSize(WIDTH / 3f, 40),

				TextAlignment = HUDAlignment.BOTTOMCENTER,
				Text = "Progress",
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontRegular,
				FontSize = 35,
			});

			AddElement(lblPoints = new HUDLabel(2)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(0, 15),
				Size = new FSize(WIDTH / 3f, 60),

				Text = "2 / 4", // TODO
				TextAlignment = HUDAlignment.BOTTOMCENTER,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontBold,
				FontSize = 57,
			});

			#endregion

			#region Buttons

			AddElement(btnMenu = new HUDIconTextButton(2)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(24, FOOTER_HEIGHT + 24),
				Size = new FSize(3.5f * GDConstants.TILE_WIDTH, 60),

				Text = "Back",
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				Icon = Textures.TexIconBack,
				BackgoundType = HUDButtonBackground.Rounded,
				Color = FlatColors.ButtonHUD,
				ColorPressed = FlatColors.ButtonPressedHUD,
			});
			btnMenu.ButtonClick += (s, a) => MainGame.Inst.SetWorldMapScreen();

			if (successScreen)
			{
				AddElement(btnNext = new HUDIconTextButton(2)
				{
					Alignment = HUDAlignment.BOTTOMRIGHT,
					RelativePosition = new FPoint(24, FOOTER_HEIGHT + 24),
					Size = new FSize(3.5f * GDConstants.TILE_WIDTH, 60),

					Text = "Next",
					TextColor = Color.White,
					Font = Textures.HUDFontRegular,
					FontSize = 55,
					TextAlignment = HUDAlignment.CENTER,
					TextPadding = 8,
					Icon = Textures.TexIconNext,
					BackgoundType = HUDButtonBackground.Rounded,
					Color = FlatColors.Nephritis,
					ColorPressed = FlatColors.Emerald,
				});
				btnNext.ButtonClick += (s, a) => HUD.Screen.PushNotification("OnClick >>Next<<"); // TODO
			}
			else
			{
				AddElement(btnNext = new HUDIconTextButton(2)
				{
					Alignment = HUDAlignment.BOTTOMRIGHT,
					RelativePosition = new FPoint(24, FOOTER_HEIGHT + 24),
					Size = new FSize(3.5f * GDConstants.TILE_WIDTH, 60),

					Text = "Again",
					TextColor = Color.White,
					Font = Textures.HUDFontRegular,
					FontSize = 55,
					TextAlignment = HUDAlignment.CENTER,
					TextPadding = 8,
					Icon = Textures.TexIconRedo,
					BackgoundType = HUDButtonBackground.Rounded,
					Color = FlatColors.Nephritis,
					ColorPressed = FlatColors.Emerald,
				});
				btnNext.ButtonClick += (s, a) => ((GDGameScreen) HUD.Screen).RestartLevel();
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
	}
}
