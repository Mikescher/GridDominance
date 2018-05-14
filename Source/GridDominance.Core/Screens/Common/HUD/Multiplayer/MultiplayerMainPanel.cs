using System;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.Multiplayer;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.Common.HUD.Multiplayer
{
    class MultiplayerMainPanel : HUDRoundedPanel
	{
		public const float WIDTH = 14.0f * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 8.0f * GDConstants.TILE_WIDTH;

		public override int Depth => 0;

		private WorldUnlockState _ustate;

		public MultiplayerMainPanel()
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;

			_ustate = UnlockManager.IsUnlocked(Levels.WORLD_ID_MULTIPLAYER, true);
         
            MainGame.Inst.GDBridge.IAB.SynchronizePurchases(GDConstants.IABList);
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			AddElement(new HUDLabel
			{
				TextAlignment = HUDAlignment.CENTER,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(WIDTH, 100),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText = L10NImpl.STR_MENU_CAP_MULTIPLAYER,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new HUDSeperator(HUDOrientation.Horizontal)
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 100),
				Size = new FSize(WIDTH, 1),
				SeperatorWidth = 2,
				Color = FlatColors.BackgroundHUD2,
			});

			AddElement(new HUDSeperator(HUDOrientation.Vertical)
			{
				Alignment = HUDAlignment.CENTER,
				RelativePosition = new FPoint(0, 50),
				Size = new FSize(1, HEIGHT - 100),
				SeperatorWidth = 2,
				Color = FlatColors.BackgroundHUD2,
			});

			AddElement(new HUDLabel
			{
				TextAlignment = HUDAlignment.TOPLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(12, 112),
				Size = new FSize(300, 32),

				Font = Textures.HUDFontBold,
				FontSize = 32,

				L10NText = L10NImpl.STR_MENU_MP_LOCAL_CLASSIC,
				TextColor = Color.White,
			});

			AddElement(new HUDLabel
			{
				TextAlignment = HUDAlignment.TOPLEFT,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(WIDTH/2 + 12, 112),
				Size = new FSize(300, 32),

				Font = Textures.HUDFontBold,
				FontSize = 32,

				L10NText = L10NImpl.STR_MENU_MP_ONLINE,
				TextColor = Color.White,
			});

			AddElement(new HUDImage
			{
				Alignment = HUDAlignment.TOPCENTER,
				Image = Textures.TexIconBluetoothClassic,
				Size = new FSize(128, 128),
				Color = ColorMath.Blend(Color.White, FlatColors.BackgroundHUD, 0.5f),
				RelativePosition = new FPoint(-WIDTH / 4f, (100 + 12 + 32 + 12) + (HEIGHT - (100 + 12 + 32 + 12) - (32 + 64 + 32 + 64 + 32)) / 2 - 64)
			});

			AddElement(new HUDImage
			{
				Alignment = HUDAlignment.TOPCENTER,
				Image = Textures.TexIconInternet,
				Size = new FSize(128, 128),
				Color = ColorMath.Blend(Color.White, FlatColors.BackgroundHUD, 0.5f),
				RelativePosition = new FPoint(+WIDTH / 4f, (100 + 12 + 32 + 12) + (HEIGHT - (100 + 12 + 32 + 12) - (32 + 64 + 32 + 64 + 32)) / 2 - 64)
			});

			AddElement(new HUDTextButton
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint((WIDTH / 2 - 320) / 2, 128),
				Size = new FSize(320, 64),

				L10NText = L10NImpl.STR_MENU_MP_JOIN,
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.PeterRiver, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.BelizeHole, 16),

				Click = OnClickJoinBluetooth,
			});

			AddElement(new HUDTextButton
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint((WIDTH / 2 - 320) / 2, 128),
				Size = new FSize(320, 64),

				L10NText = L10NImpl.STR_MENU_MP_JOIN,
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.PeterRiver, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.BelizeHole, 16),

				Click = OnClickJoinOnline,
			});

			if (_ustate == WorldUnlockState.OpenAndUnlocked)
			{
				AddElement(new HUDTextButton
				{
					Alignment = HUDAlignment.BOTTOMLEFT,
					RelativePosition = new FPoint((WIDTH / 2 - 320) / 2, 32),
					Size = new FSize(320, 64),

					L10NText = L10NImpl.STR_MENU_MP_HOST,
					TextColor = Color.White,
					Font = Textures.HUDFontBold,
					FontSize = 55,
					TextAlignment = HUDAlignment.CENTER,
					TextPadding = 8,

					BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Wisteria, 16),
					BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Amethyst, 16),

					Click = OnClickHostBluetooth,
				});

				AddElement(new HUDTextButton
				{
					Alignment = HUDAlignment.BOTTOMRIGHT,
					RelativePosition = new FPoint((WIDTH / 2 - 320) / 2, 32),
					Size = new FSize(320, 64),

					L10NText = L10NImpl.STR_MENU_MP_HOST,
					TextColor = Color.White,
					Font = Textures.HUDFontBold,
					FontSize = 55,
					TextAlignment = HUDAlignment.CENTER,
					TextPadding = 8,

					BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Wisteria, 16),
					BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Amethyst, 16),

					Click = OnClickHostOnline,
				});
			}
			else
			{
				AddElement(new HUDTextButton
				{
					Alignment = HUDAlignment.BOTTOMLEFT,
					RelativePosition = new FPoint((WIDTH / 2 - 320) / 2, 32),
					Size = new FSize(320, 64),

					L10NText = L10NImpl.STR_MENU_MP_HOST,
					TextColor = Color.White,
					Font = Textures.HUDFontBold,
					FontSize = 55,
					TextAlignment = HUDAlignment.CENTER,
					TextPadding = 8,

					BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.Asbestos, 16),
					BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.WetAsphalt, 16),

					Click = OnClickUnlock,
				});

				AddElement(new HUDTextButton
				{
					Alignment = HUDAlignment.BOTTOMRIGHT,
					RelativePosition = new FPoint((WIDTH / 2 - 320) / 2, 32),
					Size = new FSize(320, 64),

					L10NText = L10NImpl.STR_MENU_MP_HOST,
					TextColor = Color.White,
					Font = Textures.HUDFontBold,
					FontSize = 55,
					TextAlignment = HUDAlignment.CENTER,
					TextPadding = 8,

					BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.Asbestos, 16),
					BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.WetAsphalt, 16),

					Click = OnClickUnlock,
				});
			}
		}
		
		private void OnClickJoinBluetooth(HUDTextButton sender, HUDButtonEventArgs e)
		{
			Remove();
			HUD.AddModal(new MultiplayerFindLobbyScreen(MultiplayerConnectionType.P2P), true, 0.5f);
		}

		private void OnClickHostBluetooth(HUDTextButton sender, HUDButtonEventArgs e)
		{
			Remove();
			HUD.AddModal(new MultiplayerHostPanel(MultiplayerConnectionType.P2P), true, 0.5f);
		}

		private void OnClickJoinOnline(HUDTextButton sender, HUDButtonEventArgs e)
		{
			Remove();
			HUD.AddModal(new MultiplayerJoinLobbyScreen(MultiplayerConnectionType.PROXY), true, 0.5f);
		}

		private void OnClickHostOnline(HUDTextButton sender, HUDButtonEventArgs e)
		{
			Remove();
			HUD.AddModal(new MultiplayerHostPanel(MultiplayerConnectionType.PROXY), true, 0.5f);
		}

		private void OnClickUnlock(HUDTextButton sender, HUDButtonEventArgs e)
		{
			try
			{
				var r = MainGame.Inst.GDBridge.IAB.StartPurchase(GDConstants.IAB_MULTIPLAYER);
				switch (r)
				{
					case PurchaseResult.ProductNotFound:
						SAMLog.Error("MMP::IAB-PNF", "Product not found", "_iabCode -> " + GDConstants.IAB_MULTIPLAYER);
						Owner.HUD.ShowToast("MMP:ERR1", L10N.T(L10NImpl.STR_IAB_BUYERR), 40, FlatColors.Pomegranate, FlatColors.Foreground, 2.5f);
						break;
					case PurchaseResult.NotConnected:
						Owner.HUD.ShowToast("MMP:ERR2", L10N.T(L10NImpl.STR_IAB_BUYNOCONN), 40, FlatColors.Orange, FlatColors.Foreground, 2.5f);
						break;
					case PurchaseResult.CurrentlyInitializing:
						Owner.HUD.ShowToast("MMP:ERR3", L10N.T(L10NImpl.STR_IAB_BUYNOTREADY), 40, FlatColors.Orange, FlatColors.Foreground, 2.5f);
						break;
					case PurchaseResult.PurchaseStarted:
						SAMLog.Info("MMP::IAB-BUY", "PurchaseStarted");
						break;
					default:
						SAMLog.Error("MMP::EnumSwitch_OCU", "OnClickBuy()", "r -> " + r);
						break;
				}
			}
			catch (Exception ex)
			{
				SAMLog.Error("MMP::IAB_CALL", ex);
			}
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			base.DoUpdate(gameTime, istate);

			if (!Alive) return;

			if (_ustate != WorldUnlockState.OpenAndUnlocked)
			{
				_ustate = UnlockManager.IsUnlocked(Levels.WORLD_ID_MULTIPLAYER, false);

				if (_ustate == WorldUnlockState.OpenAndUnlocked)
				{
					Remove();
					Owner.HUD.AddModal(new MultiplayerMainPanel(), true, 0.5f);
				}
			}
		}
	}
}
