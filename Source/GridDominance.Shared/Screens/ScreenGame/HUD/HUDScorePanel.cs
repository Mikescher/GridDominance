using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.MathHelper;
using MonoSAMFramework.Portable.MathHelper.FloatClasses;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Text;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

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
		private HUDIconTextButton btnMenu;
		private HUDIconTextButton btnNext;

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

				Text = "4 - 4",
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

			AddElement(lblPoints = new HUDIncrementIndicatorLabel("236", "+20", 2)
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

				Text = "2 / 4",
				TextAlignment = HUDAlignment.BOTTOMCENTER,
				TextColor = FlatColors.TextHUD,
				Font = Textures.HUDFontBold,
				FontSize = 57,
			});


			AddElement(btnMenu = new HUDIconTextButton(2)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(24, FOOTER_HEIGHT + 24),
				Size = new FSize(4.5f * GDGameScreen.TILE_WIDTH, 70),

				Text = "Back",
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 65,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				Icon = Textures.TexIconBack,
				BackgoundType = HUDButtonBackground.Rounded,
				Color = FlatColors.ButtonHUD,
				ColorPressed = FlatColors.ButtonPressedHUD,
			});
			btnMenu.ButtonClick += (s, a) => HUD.Screen.PushNotification("OnClick >>Prev<<");


			AddElement(btnNext = new HUDIconTextButton(2)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(24, FOOTER_HEIGHT + 24),
				Size = new FSize(4.5f * GDGameScreen.TILE_WIDTH, 70),

				Text = "Next",
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 65,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				Icon = Textures.TexIconNext,
				BackgoundType = HUDButtonBackground.Rounded,
				Color = FlatColors.Nephritis,
				ColorPressed = FlatColors.Emerald,
			});
			btnNext.ButtonClick += (s, a) => HUD.Screen.PushNotification("OnClick >>Next<<");
		}
	}
}
