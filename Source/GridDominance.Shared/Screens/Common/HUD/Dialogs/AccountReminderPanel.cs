using System;
using System.Threading.Tasks;
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.Common.HUD.HUDOperations;
using GridDominance.Shared.Screens.OverworldScreen;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens.HUD;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using GridDominance.Shared.Screens.Common.HUD;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class AccountReminderPanel : HUDRoundedPanel
	{
		public const float WIDTH = 11 * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 7 * GDConstants.TILE_WIDTH;

		public const float FOOTER_HEIGHT = 2 * GDConstants.TILE_WIDTH;

		private HUDTextButton _buttonYes;

		public override int Depth => 0;

		public AccountReminderPanel()
		{
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
			Alignment = HUDAlignment.CENTER;
			Background = FlatColors.BackgroundHUD;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			AddElement(new HUDRectangle(0)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				Size = new FSize(WIDTH, FOOTER_HEIGHT),

				Definition = HUDBackgroundDefinition.CreateRounded(FlatColors.BackgroundHUD2, 16, false, false, true, true),
			});

			AddElement(new HUDLabel(1)
			{
				TextAlignment = HUDAlignment.CENTER,
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(WIDTH, 100),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText = L10NImpl.STR_AAP_HEADER,
				TextColor = FlatColors.Clouds,
			});

			AddElement(new HUDLabel(1)
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(20, 100),
				Size = new FSize(WIDTH - 40, 200),

				Font = Textures.HUDFontRegular,
				FontSize = 32,
				TextColor = ColorMath.Blend(FlatColors.Clouds, FlatColors.Background, 0.2f),
				WordWrap = HUDWordWrap.WrapByWordTrusted,
				MaxWidth = WIDTH - 40,
				TextAlignment = HUDAlignment.CENTER,

				L10NText = L10NImpl.STR_ACCOUNT_REMINDER,
			});

			AddElement(_buttonYes = new HUDTextButton(1)
			{
				Alignment = HUDAlignment.BOTTOMLEFT,
				RelativePosition = new FPoint(24, 32),
				Size = new FSize((WIDTH - 3 * 24) / 2, 64),

				L10NText = L10NImpl.STR_BTN_YES,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Emerald, 16),
				BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.BelizeHole, 16),

				Click = OnClickYes,
			});

			AddElement(new HUDTextButton(1)
			{
				Alignment = HUDAlignment.BOTTOMRIGHT,
				RelativePosition = new FPoint(24, 32),
				Size = new FSize((WIDTH - 3 * 24) / 2, 64),

				L10NText = L10NImpl.STR_BTN_NO,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 55,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				BackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.ButtonHUD, 16f),
				BackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.ButtonPressedHUD, 16f),

				Click = OnClickNo,
			});
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			base.DoUpdate(gameTime, istate);

			_buttonYes.BackgroundNormal = _buttonYes.BackgroundNormal.WithColor(ColorMath.Blend(FlatColors.Emerald, FlatColors.WetAsphalt, FloatMath.PercSin(gameTime.TotalElapsedSeconds * 3)));
		}

		private void OnClickYes(HUDTextButton sender, HUDButtonEventArgs e)
		{
			MainGame.Inst.Profile.AccountReminderShown = true;
			MainGame.Inst.SaveProfile();

			Remove();

			((ISettingsOwnerHUD) Owner.HUD).ShowAccountPanel();
		}

		private void OnClickNo(HUDTextButton sender, HUDButtonEventArgs e)
		{
			MainGame.Inst.Profile.AccountReminderShown = true;
			MainGame.Inst.SaveProfile();
			Remove();
		}
	}
}
