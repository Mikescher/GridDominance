using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.MathHelper;
using MonoSAMFramework.Portable.MathHelper.FloatClasses;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.Screens.ScreenGame.HUD
{
	class HUDScorePanel : HUDRoundedPanel
	{
		public const int WIDTH = 11 * GDGameScreen.TILE_WIDTH;
		public const int HEIGHT = 7 * GDGameScreen.TILE_WIDTH;

		public const int FOOTER_HEIGHT = 2 * GDGameScreen.TILE_WIDTH;
		public const int FOOTER_COLBAR_HEIGHT = GDGameScreen.TILE_WIDTH / 4;

		public override int Depth => 0;

		private HUDLabel lblPoints;

		public HUDScorePanel()
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			AddElement(new HUDRoundedRectangle(0)
			{
				Color = FlatColors.BackgroundHUD2,
				Alignment = HUDAlignment.BOTTOMRIGHT,
				Size = new FSize(WIDTH, FOOTER_HEIGHT - 10),
				RoundCornerTL = false,
				RoundCornerTR = false,
				RoundCornerBL = true,
				RoundCornerBR = true,
			});


			AddElement(new HUDRectangle(2)
			{
				Color = FlatColors.Nephritis,
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0, FOOTER_HEIGHT - FOOTER_COLBAR_HEIGHT),
				Size = new FSize(WIDTH / 3f, FOOTER_COLBAR_HEIGHT),
			});

			AddElement(new HUDRectangle(1)
			{
				Color = FlatColors.PeterRiver,
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, FOOTER_HEIGHT - FOOTER_COLBAR_HEIGHT),
				Size = new FSize(WIDTH / 2f, FOOTER_COLBAR_HEIGHT),
			});

			AddElement(new HUDRectangle(2)
			{
				Color = FlatColors.Pomegranate,
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(0, FOOTER_HEIGHT - FOOTER_COLBAR_HEIGHT),
				Size = new FSize(WIDTH / 3f, FOOTER_COLBAR_HEIGHT),
			});


			AddElement(new HUDSeperator(HUDOrientation.Vertical, 3)
			{
				Color = FlatColors.SeperatorHUD,
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(WIDTH / 3f, 0),
				Size = new FSize(1, FOOTER_HEIGHT),
			});

			AddElement(new HUDSeperator(HUDOrientation.Vertical, 3)
			{
				Color = FlatColors.SeperatorHUD,
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(WIDTH / 3f, 0),
				Size = new FSize(1, FOOTER_HEIGHT),
			});

			AddElement(new HUDLabel(2)
			{
				TextAlignment = HUDAlignment.BOTTOMCENTER,
				Text = "Level",
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontRegular,
				FontSize = 35,
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0, 77),
				Size = new FSize(WIDTH / 3f, 40),
			});

			AddElement(lblPoints = new HUDLabel(2)
			{
				Text = "4 - 4",
				TextAlignment = HUDAlignment.BOTTOMCENTER,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontBold,
				FontSize = 57,
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0, 15),
				Size = new FSize(WIDTH / 3f, 60),
			});

			AddElement(new HUDLabel(2)
			{
				TextAlignment = HUDAlignment.BOTTOMCENTER,
				Text = "Points",
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontRegular,
				FontSize = 35,
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, 77),
				Size = new FSize(WIDTH / 3f, 40),
			});

			AddElement(lblPoints = new HUDIncrementIndicatorLabel("236", "+20", 2)
			{
				TextAlignment = HUDAlignment.BOTTOMCENTER,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontBold,
				FontSize = 57,
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, 15),
				Size = new FSize(WIDTH / 3f, 60),
			});

			AddElement(new HUDLabel(2)
			{
				TextAlignment = HUDAlignment.BOTTOMCENTER,
				Text = "Progress",
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontRegular,
				FontSize = 35,
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(0, 77),
				Size = new FSize(WIDTH / 3f, 40),
			});

			AddElement(lblPoints = new HUDLabel(2)
			{
				Text = "2 / 4",
				TextAlignment = HUDAlignment.BOTTOMCENTER,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontBold,
				FontSize = 57,
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(0, 15),
				Size = new FSize(WIDTH / 3f, 60),
			});
		}
	}
}
