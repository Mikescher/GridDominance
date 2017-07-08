using System;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
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
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD
{
	class WorldPreviewPanel : HUDRoundedPanel
	{
		public const float WIDTH =  14.0f * GDConstants.TILE_WIDTH;
		public const float HEIGHT =  6.0f * GDConstants.TILE_WIDTH + 2.5f * GDConstants.TILE_WIDTH;

		public const float INNER_WIDTH  = 0.6f * GDConstants.VIEW_WIDTH;
		public const float INNER_HEIGHT = 0.6f * GDConstants.VIEW_HEIGHT;

		public override int Depth => 0;

		private readonly LevelBlueprint[] _blueprints;
		private readonly Guid _id;
		private readonly string _iabCode;

		private HUDSubScreenProxyRenderer _proxy;
		private HUDTextButton _button;

		private readonly int _worldNumber;
		private readonly GraphBlueprint _prevWorld;

		public WorldPreviewPanel(LevelBlueprint[] bps, Guid unlockID, string iab, int worldnumber, GraphBlueprint prevWorld)
		{
			_blueprints = bps;
			_id = unlockID;
			_iabCode = iab;
			_worldNumber = worldnumber;
			_prevWorld = prevWorld;

			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.Asbestos;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			var prev = new GDGameScreen_Preview(MainGame.Inst, MainGame.Inst.Graphics, this, _blueprints, 0, _worldNumber);

			AddElement(_proxy = new HUDSubScreenProxyRenderer(prev)
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 0.5f * GDConstants.TILE_WIDTH),
				Size = new FSize(INNER_WIDTH, INNER_HEIGHT),
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
				BackgoundType = HUDBackgroundType.RoundedBlur,
				Color = FlatColors.PeterRiver,
				ColorPressed = FlatColors.BelizeHole,

				Click = OnClickBuy,
			});

			AddElement(new HUDTextButton
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(0.5f * GDConstants.TILE_WIDTH, 0.5f * GDConstants.TILE_WIDTH),
				Size = new FSize(5.5f * GDConstants.TILE_WIDTH, 1.0f * GDConstants.TILE_WIDTH),

				Text = L10N.TF(L10NImpl.STR_PREV_FINISHWORLD, _worldNumber - 1),
				TextColor = Color.White,
				Font = Textures.HUDFontBold,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,
				BackgoundType = HUDBackgroundType.RoundedBlur,
				Color = FlatColors.Turquoise,
				ColorPressed = FlatColors.GreenSea,

				Click = OnClickFinishPrev,
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
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			base.DoUpdate(gameTime, istate);

			_button.Color = ColorMath.Blend(FlatColors.BelizeHole, FlatColors.WetAsphalt, FloatMath.PercSin(gameTime.TotalElapsedSeconds * 5));

			if (MainGame.Inst.Profile.PurchasedWorlds.Contains(_id))
			{
				HUD.ShowToast(L10N.T(L10NImpl.STR_IAB_BUYSUCESS), 40, FlatColors.Emerald, FlatColors.Foreground, 2.5f);
				MainGame.Inst.SetOverworldScreenCopy(HUD.Screen as GDOverworldScreen);
			}

			if (MainGame.Inst.Bridge.IAB.IsPurchased(_iabCode) == PurchaseQueryResult.Purchased)
			{
				MainGame.Inst.Profile.PurchasedWorlds.Add(_id);
				MainGame.Inst.SaveProfile();

				HUD.ShowToast(L10N.T(L10NImpl.STR_IAB_BUYSUCESS), 40, FlatColors.Emerald, FlatColors.Foreground, 2.5f);
				MainGame.Inst.SetOverworldScreenCopy(HUD.Screen as GDOverworldScreen);
			}
		}

		public void SetNextScreen(int idx)
		{
			var prev = new GDGameScreen_Preview(MainGame.Inst, MainGame.Inst.Graphics, this, _blueprints, idx, _worldNumber);

			_proxy.ChangeScreen(prev);
		}

		private void OnClickBuy(HUDTextButton sender, HUDButtonEventArgs args)
		{
			try
			{
				var r = MainGame.Inst.Bridge.IAB.StartPurchase(_iabCode);
				switch (r)
				{
					case PurchaseResult.ProductNotFound:
						SAMLog.Error("IAB-PNF", "Product not found", "_iabCode -> " + _iabCode);
						Owner.HUD.ShowToast(L10N.T(L10NImpl.STR_IAB_BUYERR), 40, FlatColors.Pomegranate, FlatColors.Foreground, 2.5f);
						break;
					case PurchaseResult.NotConnected:
						Owner.HUD.ShowToast(L10N.T(L10NImpl.STR_IAB_BUYNOCONN), 40, FlatColors.Orange, FlatColors.Foreground, 2.5f);
						break;
					case PurchaseResult.CurrentlyInitializing:
						Owner.HUD.ShowToast(L10N.T(L10NImpl.STR_IAB_BUYNOTREADY), 40, FlatColors.Orange, FlatColors.Foreground, 2.5f);
						break;
					case PurchaseResult.PurchaseStarted:
						SAMLog.Info("IAB-BUY", "PurchaseStarted");
						break;
					default:
						SAMLog.Error("EnumSwitch-OCB", "OnClickBuy()", "r -> " + r);
						break;
				}
			}
			catch (Exception e)
			{
				SAMLog.Error("IAB_CALL", e);
				
			}
		}

		private void OnClickFinishPrev(HUDTextButton sender, HUDButtonEventArgs args)
		{
			int missPoints = 0;
			int missLevel = 0;
			BlueprintAnalyzer.ListUnfinishedCount(_prevWorld, out missPoints, out missLevel);
			
			HUD.ShowToast(L10N.TF(L10NImpl.STR_PREV_MISS_TOAST, missPoints, missLevel, _worldNumber), 32, FlatColors.Orange, FlatColors.Foreground, 4f);

			MainGame.Inst.SetWorldMapScreenZoomedOut(_prevWorld);
		}
	}
}