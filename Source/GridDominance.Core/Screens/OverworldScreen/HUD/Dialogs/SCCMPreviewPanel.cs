using System;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common;
using GridDominance.Shared.Screens.Common.HUD.Elements;
using GridDominance.Shared.Screens.NormalGameScreen;
using GridDominance.Shared.Screens.WorldMapScreen;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD
{
	class SCCMPreviewPanel : HUDRoundedPanel
	{
		public const float WIDTH =  14.0f * GDConstants.TILE_WIDTH;
		public const float HEIGHT =  9.0f * GDConstants.TILE_WIDTH;

		public override int Depth => 0;

		private HUDTextButton _button;

		public SCCMPreviewPanel()
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.Asbestos;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			AddElement(new HUDImage
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(WIDTH, 2f * GDConstants.TILE_WIDTH),

				Image = Textures.TexTitle_SCCM,
			});
			
			AddElement(new HUDImage
			{
				Alignment = HUDAlignment.CENTER,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(5.5f * GDConstants.TILE_WIDTH, 5.5f * GDConstants.TILE_WIDTH),

				Image = Textures.TexCircle,
				Color = Color.Black * 0.1f,
			});
			
			AddElement(new HUDTetroAnimation
			{
				Alignment = HUDAlignment.CENTER,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(5f * GDConstants.TILE_WIDTH, 5f * GDConstants.TILE_WIDTH),

				Foreground = FlatColors.Silver,
			});
			
			AddElement(new HUDImage
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(0.5f * GDConstants.TILE_WIDTH, 2f * GDConstants.TILE_WIDTH),
				Size = new FSize(WIDTH, 3f * GDConstants.TILE_WIDTH),

				Image = Textures.TexDescription_SCCM,
				ImageAlignment = HUDImageAlignmentAlgorithm.TOPLEFT,
				ImageScale     = HUDImageScaleAlgorithm.UNDERSCALE,
			});

			AddElement(_button = new HUDTextButton
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(0.5f * GDConstants.TILE_WIDTH, 0.5f * GDConstants.TILE_WIDTH),
				Size = new FSize(5.5f * GDConstants.TILE_WIDTH, 1.0f * GDConstants.TILE_WIDTH),

				L10NText = L10NImpl.STR_PREV_BUYNOW,
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.PeterRiver, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.BelizeHole, 16),

				Click = OnClickBuy,
			});

			AddElement(new HUDLabel
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, 0.5f * GDConstants.TILE_WIDTH),
				Size = new FSize(5.5f * GDConstants.TILE_WIDTH, 1.0f * GDConstants.TILE_WIDTH),

				L10NText = L10NImpl.STR_PREV_OR,
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
			});

			AddElement(new HUDTextButton
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0.5f * GDConstants.TILE_WIDTH, 0.5f * GDConstants.TILE_WIDTH),
				Size = new FSize(5.5f * GDConstants.TILE_WIDTH, 1.0f * GDConstants.TILE_WIDTH),

				Text = L10N.T(L10NImpl.STR_PREV_FINISHGAME),
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Turquoise, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.GreenSea, 16),

				Click = OnClickFinishPrev,
			});
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			base.DoUpdate(gameTime, istate);

			if (_button !=null) _button.BackgroundNormal = _button.BackgroundNormal.WithColor(ColorMath.Blend(FlatColors.BelizeHole, FlatColors.WetAsphalt, FloatMath.PercSin(gameTime.TotalElapsedSeconds * 5)));

			if (MainGame.Inst.Profile.PurchasedWorlds.Contains(Levels.WORLD_ID_ONLINE))
			{
				HUD.ShowToast("SCCMPP::SUCC1", L10N.T(L10NImpl.STR_IAB_BUYSUCESS), 40, FlatColors.Emerald, FlatColors.Foreground, 2.5f);
				MainGame.Inst.SetOverworldScreenCopy(HUD.Screen as GDOverworldScreen);
			}

			if (MainGame.Inst.GDBridge.IAB.IsPurchased(GDConstants.IAB_ONLINE) == PurchaseQueryResult.Purchased)
			{
				MainGame.Inst.Profile.PurchasedWorlds.Add(Levels.WORLD_ID_ONLINE);
				MainGame.Inst.SaveProfile();

				HUD.ShowToast("SCCMPP::SUCC2", L10N.T(L10NImpl.STR_IAB_BUYSUCESS), 40, FlatColors.Emerald, FlatColors.Foreground, 2.5f);
				MainGame.Inst.SetOverworldScreenCopy(HUD.Screen as GDOverworldScreen);
			}
		}

		private void OnClickBuy(HUDTextButton sender, HUDButtonEventArgs args)
		{
			try
			{
				var r = MainGame.Inst.GDBridge.IAB.StartPurchase(GDConstants.IAB_ONLINE);
				switch (r)
				{
					case PurchaseResult.ProductNotFound:
						SAMLog.Error("SCCMPP::IAB-PNF", "Product not found", "_iabCode -> " + GDConstants.IAB_ONLINE);
						Owner.HUD.ShowToast("SCCMPP::ERR1", L10N.T(L10NImpl.STR_IAB_BUYERR), 40, FlatColors.Pomegranate, FlatColors.Foreground, 2.5f);
						break;
					case PurchaseResult.NotConnected:
						Owner.HUD.ShowToast("SCCMPP::ERR2", L10N.T(L10NImpl.STR_IAB_BUYNOCONN), 40, FlatColors.Orange, FlatColors.Foreground, 2.5f);
						break;
					case PurchaseResult.CurrentlyInitializing:
						Owner.HUD.ShowToast("SCCMPP::ERR3", L10N.T(L10NImpl.STR_IAB_BUYNOTREADY), 40, FlatColors.Orange, FlatColors.Foreground, 2.5f);
						break;
					case PurchaseResult.PurchaseStarted:
						SAMLog.Info("SCCMPP::IAB-BUY", "PurchaseStarted");
						break;
					default:
						SAMLog.Error("SCCMPP::EnumSwitch_OCB", "OnClickBuy()", "r -> " + r);
						break;
				}
			}
			catch (Exception e)
			{
				SAMLog.Error("SCCMPP::IAB_CALL", e);
				
			}
		}

		private void OnClickFinishPrev(HUDTextButton sender, HUDButtonEventArgs args)
		{
			if (UnlockManager.IsUnlocked(Levels.WORLD_001, false) != WorldUnlockState.OpenAndUnlocked)
			{
				MainGame.Inst.SetTutorialLevelScreen();
				return;
			}

			if (UnlockManager.IsUnlocked(Levels.WORLD_002, false) != WorldUnlockState.OpenAndUnlocked)
			{
				MainGame.Inst.SetWorldMapScreenZoomedOut(Levels.WORLD_001);
				return;
			}

			if (UnlockManager.IsUnlocked(Levels.WORLD_003, false) != WorldUnlockState.OpenAndUnlocked)
			{
				MainGame.Inst.SetWorldMapScreenZoomedOut(Levels.WORLD_002);
				return;
			}

			if (UnlockManager.IsUnlocked(Levels.WORLD_004, false) != WorldUnlockState.OpenAndUnlocked)
			{
				MainGame.Inst.SetWorldMapScreenZoomedOut(Levels.WORLD_003);
				return;
			}
			
			MainGame.Inst.SetWorldMapScreenZoomedOut(Levels.WORLD_004);
		}
	}
}