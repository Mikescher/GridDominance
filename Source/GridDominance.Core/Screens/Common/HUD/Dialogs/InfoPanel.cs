using System;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Presenter;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.Common.HUD.Dialogs
{
	class InfoPanel : HUDRoundedPanel
	{
		public const float WIDTH = 7 * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 9 * GDConstants.TILE_WIDTH;

		public const float BFB_WIDTH = 2 * GDConstants.TILE_WIDTH;

		public const float ELEM_MARGIN = 0.25f * GDConstants.TILE_WIDTH;
		public const float BUTTON_HEIGHT = 65;

		public override int Depth => 0;

		private HUDTextButton btnUnlock;

		private int _clickCount = 0;

		public InfoPanel()
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			var imgWidth  = WIDTH - 2 * ELEM_MARGIN;
			var imgHeight = imgWidth * (Textures.TexLogo.Height * 1f / Textures.TexLogo.Width);

			AddElement(new HUDImage(1)
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, ELEM_MARGIN),
				Size = new FSize(imgWidth, imgHeight),

				Image = Textures.TexLogo,
			});

			var btn1y = ELEM_MARGIN + imgHeight;

			AddElement(new HUDTextButton(1)
			{
				TextAlignment = HUDAlignment.CENTER,
				Alignment = HUDAlignment.TOPCENTER,

				RelativePosition = new FPoint(0, btn1y),
				Size = new FSize(imgWidth, BUTTON_HEIGHT),

				Font = Textures.HUDFontRegular,
				FontSize = 52,

				L10NText = L10NImpl.STR_ATTRIBUTIONS,

				TextColor = Color.White,
				TextPadding = 16,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonHUD, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonPressedHUD, 16),
				
				ClickMode = HUDButton.HUDButtonClickMode.Single,
				Click = OnClickAttributions,
			});

			var btn2y = btn1y + BUTTON_HEIGHT + ELEM_MARGIN;

			AddElement(new HUDTextButton(1)
			{
				TextAlignment = HUDAlignment.CENTER,
				Alignment = HUDAlignment.TOPCENTER,

				RelativePosition = new FPoint(0, btn2y),
				Size = new FSize(imgWidth, BUTTON_HEIGHT),

				Font = Textures.HUDFontRegular,
				FontSize = 52,

				L10NText = L10NImpl.STR_CREDPNL_HEADER,

				TextColor = Color.White,
				TextPadding = 16,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonHUD, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonPressedHUD, 16),

				ClickMode = HUDButton.HUDButtonClickMode.Single,
				Click = OnClickAcknowledgements,
			});

			var btn3y = btn2y + BUTTON_HEIGHT + ELEM_MARGIN;

			AddElement(btnUnlock = new HUDTextButton(1)
			{
				TextAlignment = HUDAlignment.CENTER,
				Alignment = HUDAlignment.TOPCENTER,

				RelativePosition = new FPoint(0, btn3y),
				Size = new FSize(imgWidth, BUTTON_HEIGHT),

				Font = Textures.HUDFontRegular,
				FontSize = 52,

				L10NText = L10NImpl.STR_UNLOCK,

				TextColor = Color.White,
				TextPadding = 16,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonHUD, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonPressedHUD, 16),

				ClickMode = HUDButton.HUDButtonClickMode.Single,
				Click = OnClickUnlock,

				IsVisible = !MonoSAMGame.IsIOS(),
			});

			
			AddElement(new HUDTextButton(1)
			{
				TextAlignment = HUDAlignment.CENTER,
				Alignment = HUDAlignment.BOTTOMLEFT,

				RelativePosition = new FPoint(ELEM_MARGIN, ELEM_MARGIN),
				Size = new FSize(WIDTH - (BFB_WIDTH + 3 * ELEM_MARGIN), BFB_WIDTH - 10),

				Font = Textures.HUDFontRegular,
				FontSize = 52,

				L10NText = L10NImpl.STR_RESTOREPURCHASES,

				TextColor = Color.White,
				TextPadding = 16,

				BackgroundNormal = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonHUD, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRounded(FlatColors.ButtonPressedHUD, 16),

				ClickMode = HUDButton.HUDButtonClickMode.Single,
				Click = OnClickPurchaseRestore,

				IsVisible = MonoSAMGame.IsIOS() && MainGame.IsIAB(),
			});

			AddElement(new TimesheetAnimationPresenter(1)
			{
				Animation = Animations.AnimationBlackForestBytesLogo,

				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(ELEM_MARGIN, ELEM_MARGIN),
				Size = new FSize(BFB_WIDTH, BFB_WIDTH),

				Click = OnClickBFB,
			});
		}

		private void OnClickBFB(TimesheetAnimationPresenter sender, EventArgs e)
		{
			MainGame.Inst.GDBridge.OpenURL(GDConstants.BFB_URL);
		}

		private void OnClickUnlock(HUDTextButton sender, HUDButtonEventArgs e)
		{
			Remove();
			HUD.AddModal(new UnlockPanel(), true);
		}

		private void OnClickPurchaseRestore(HUDTextButton sender, HUDButtonEventArgs e)
		{
			MainGame.Inst.GDBridge.IAB.SynchronizePurchases(GDConstants.IABList);
			HUD.ShowToast(null, L10N.T(L10NImpl.STR_STARTSYNC), 32, FlatColors.Silver, FlatColors.Foreground, 3f);
		}

		private void OnClickAttributions(HUDTextButton sender, HUDButtonEventArgs e)
		{
			Remove();
			HUD.AddModal(new AttributionsPanel(), true);
		}

		private void OnClickAcknowledgements(HUDTextButton sender, HUDButtonEventArgs e)
		{
			Remove();
			HUD.AddModal(new AcknowledgementsPanel(), true);
		}
		
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate)
		{
			if (_clickCount++ == 16) btnUnlock.IsVisible = true;


			return base.OnPointerDown(relPositionPoint, istate);
		}
	}
}