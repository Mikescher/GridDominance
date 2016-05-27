using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoSAMFramework.Portable.ColorHelper;
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
			RelativePosition = Point.Zero;
			Size = new Size(WIDTH, HEIGHT);
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
				Size = new Size(WIDTH, FOOTER_HEIGHT - 10),
				RoundCornerTL = false,
				RoundCornerTR = false,
				RoundCornerBL = true,
				RoundCornerBR = true,
			});


			AddElement(new HUDRectangle(2)
			{
				Color = FlatColors.Nephritis,
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new Point(0, FOOTER_HEIGHT - FOOTER_COLBAR_HEIGHT),
				Size = new Size(WIDTH / 3, FOOTER_COLBAR_HEIGHT),
			});

			AddElement(new HUDRectangle(1)
			{
				Color = FlatColors.PeterRiver,
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new Point(0, FOOTER_HEIGHT - FOOTER_COLBAR_HEIGHT),
				Size = new Size(WIDTH / 2, FOOTER_COLBAR_HEIGHT),
			});

			AddElement(new HUDRectangle(2)
			{
				Color = FlatColors.Pomegranate,
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new Point(0, FOOTER_HEIGHT - FOOTER_COLBAR_HEIGHT),
				Size = new Size(WIDTH / 3, FOOTER_COLBAR_HEIGHT),
			});


			AddElement(new HUDSeperator(HUDOrientation.Vertical, 3)
			{
				Color = FlatColors.SeperatorHUD,
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new Point(WIDTH / 3, 0),
				Size = new Size(1, FOOTER_HEIGHT),
			});

			AddElement(new HUDSeperator(HUDOrientation.Vertical, 3)
			{
				Color = FlatColors.SeperatorHUD,
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new Point(WIDTH / 3, 0),
				Size = new Size(1, FOOTER_HEIGHT),
			});

			AddElement(new HUDLabel(2)
			{
				TextAlignment = HUDAlignment.BOTTOMCENTER,
				Text = "Points",
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 35,
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new Point(0, 77),
				Size = new Size(WIDTH / 3, 35),
			});

			AddElement(lblPoints = new HUDIncrementIndicatorLabel("236", "+20", 2)
			{
				TextAlignment = HUDAlignment.BOTTOMCENTER,
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 56,
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new Point(0, 15),
				Size = new Size(WIDTH / 3, 55),
			});
		}
	}
}
