using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using MonoSAMFramework.Portable.RenderHelper;

namespace GridDominance.Shared.Screens.NormalGameScreen.HUD
{
	class HUDMultiplayerScorePanel : HUDRoundedPanel
	{
		public const float WIDTH = 11 * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 7 * GDConstants.TILE_WIDTH;

		public const float FOOTER_HEIGHT = 2 * GDConstants.TILE_WIDTH;
		public const float FOOTER_COLBAR_HEIGHT = GDConstants.TILE_WIDTH / 4f;
		public const float ICON_MARGIN = GDConstants.TILE_WIDTH * (3/8f);
		public const float ICON_SIZE = GDConstants.TILE_WIDTH * 2;

		private GDGameScreen GDScreen => (GDGameScreen)HUD.Screen;

		public override int Depth => 0;

		private readonly bool successScreen;
		private readonly PlayerProfile profile;
		private readonly LevelBlueprint Level;
		
		public HUDMultiplayerScorePanel(LevelBlueprint lvl, PlayerProfile playerprofile, bool playerHasWon)
		{
			successScreen = playerHasWon;
			profile = playerprofile;
			Level = lvl;

			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		public override void OnInitialize() //TODO a little bit ... empty
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

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonHUD, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonPressedHUD, 16),

				Click = (s, a) => GDScreen.ExitToMap(),
			});

			#endregion

			#region Icons

			var finDiff0 = profile.GetLevelData(this.GDHUD().GDOwner.Blueprint.UniqueID).HasCompleted(FractionDifficulty.KI_EASY);
			var finDiff1 = profile.GetLevelData(this.GDHUD().GDOwner.Blueprint.UniqueID).HasCompleted(FractionDifficulty.KI_NORMAL);
			var finDiff2 = profile.GetLevelData(this.GDHUD().GDOwner.Blueprint.UniqueID).HasCompleted(FractionDifficulty.KI_HARD);
			var finDiff3 = profile.GetLevelData(this.GDHUD().GDOwner.Blueprint.UniqueID).HasCompleted(FractionDifficulty.KI_IMPOSSIBLE);

			var modeDiff0 =
				finDiff0 ?
					HUDDifficultyButton.HUDDifficultyButtonMode.ACTIVATED :
					HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED;

			var modeDiff1 =
				finDiff1 ?
					HUDDifficultyButton.HUDDifficultyButtonMode.ACTIVATED :
					HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED;

			var modeDiff2 =
				finDiff2 ?
					HUDDifficultyButton.HUDDifficultyButtonMode.ACTIVATED :
					HUDDifficultyButton.HUDDifficultyButtonMode.DEACTIVATED;

			var modeDiff3 =
				finDiff3 ?
					HUDDifficultyButton.HUDDifficultyButtonMode.ACTIVATED :
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
