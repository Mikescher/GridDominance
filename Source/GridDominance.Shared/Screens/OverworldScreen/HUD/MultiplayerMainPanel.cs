using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD
{
    class MultiplayerMainPanel : HUDRoundedPanel
	{
		public const float WIDTH = 14.0f * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 8.0f * GDConstants.TILE_WIDTH;

		public override int Depth => 0;

		public MultiplayerMainPanel()
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			AddElement(new HUDLabel(1)
			{
				TextAlignment = HUDAlignment.CENTER,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(WIDTH, 100),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				Text = "**Multiplayer**",
				TextColor = FlatColors.Clouds,
			});
			
			AddElement(new HUDTextButton
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(32, 128),
				Size = new FSize(320, 64),

				Text = "**Join**",
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				BackgoundType = HUDBackgroundType.RoundedBlur,
				Color = FlatColors.PeterRiver,
				ColorPressed = FlatColors.Wisteria,

				Click = OnClickJoinBluetooth,
			});

			AddElement(new HUDTextButton
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(32, 32),
				Size = new FSize(320, 64),

				Text = "**Host**",
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				BackgoundType = HUDBackgroundType.RoundedBlur,
				Color = FlatColors.PeterRiver,
				ColorPressed = FlatColors.Wisteria,

				Click = OnClickHostBluetooth,
			});
			
			AddElement(new HUDTextButton
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(32, 128),
				Size = new FSize(320, 64),

				Text = "**Join**",
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				BackgoundType = HUDBackgroundType.RoundedBlur,
				Color = FlatColors.PeterRiver,
				ColorPressed = FlatColors.BelizeHole,

				Click = OnClickJoinOnline,
			});
			
			AddElement(new HUDTextButton
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(32, 32),
				Size = new FSize(320, 64),

				Text = "**Host**",
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				BackgoundType = HUDBackgroundType.RoundedBlur,
				Color = FlatColors.PeterRiver,
				ColorPressed = FlatColors.BelizeHole,

				Click = OnClickHostOnline,
			});
		}

		private void OnClickJoinBluetooth(HUDTextButton sender, HUDButtonEventArgs e)
		{
			//TODO
		}

		private void OnClickHostBluetooth(HUDTextButton sender, HUDButtonEventArgs e)
		{
			//TODO
		}

		private void OnClickJoinOnline(HUDTextButton sender, HUDButtonEventArgs e)
		{
			Remove();
			HUD.AddModal(new MultiplayerJoinLobbyScreen(), true, 0.5f);
		}
		private void OnClickHostOnline(HUDTextButton sender, HUDButtonEventArgs e)
		{
			Remove();
			HUD.AddModal(new MultiplayerHostPanel(), true, 0.5f);
		}
	}
}
