using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common.HUD.HUDOperations;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using System;
using System.Linq;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Presenter;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class UnlockPanel : HUDRoundedPanel
	{
		public const float DISP_WIDTH = 64;
		public const float DISP_PAD   = 10;

		public const float WIDTH = 8 * DISP_WIDTH + 9 * DISP_PAD;
		public const float HEIGHT = 9 * GDConstants.TILE_WIDTH;

		public const float BTN_WIDTH    = 64;
		public const float BTN_PADY     = (HEIGHT - 84 - DISP_WIDTH - 4 * BTN_WIDTH) / 5;
		public const float BTN_PADX     = BTN_PADY;
		public const float BTN_OFFSET_X = (WIDTH - (3 * BTN_WIDTH + 2 * BTN_PADX)) / 2;
		public const float BTN_OFFSET_Y = BTN_PADY;

		public static readonly char[] BTN_TXT = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '#', '0', '*', };

		public override int Depth => 0;

		public int CharIndex = 0;
		public readonly HUDCharacterControl[] CharDisp = new HUDCharacterControl[8];

		public UnlockPanel()
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
				Size = new FSize(WIDTH, 96),

				Font = Textures.HUDFontBold,
				FontSize = 64,

				L10NText = L10NImpl.STR_UNLOCK,
				TextColor = FlatColors.Clouds,
			});

			for (int i = 0; i < 8; i++)
			{
				CharDisp[i] = new HUDCharacterControl(1)
				{
					Alignment = HUDAlignment.TOPLEFT,
					RelativePosition = new FPoint(DISP_PAD + i * (DISP_PAD + DISP_WIDTH), 84),
					Size = new FSize(DISP_WIDTH, DISP_WIDTH),

					BackgoundType = HUDBackgroundType.Simple,
					BackgroundColor = FlatColors.Clouds,

					BorderWidth = 4,
					BorderColor = Color.Black,

					TextPadding = 2,
					TextColor = Color.Black
				};
				AddElement(CharDisp[i]);
			}

			for (int x = 0; x < 3; x++)
			{
				for (int y = 0; y < 4; y++)
				{
					int idx = x + y * 3;

					AddElement(new HUDTextButton(1)
					{
						TextAlignment = HUDAlignment.CENTER,
						Alignment = HUDAlignment.BOTTOMLEFT,
						RelativePosition = new FPoint(BTN_OFFSET_X + (BTN_WIDTH + BTN_PADX) * x, BTN_OFFSET_Y + (BTN_WIDTH + BTN_PADY) * (3-y)),
						Size = new FSize(BTN_WIDTH, BTN_WIDTH),

						Font = Textures.HUDFontBold,
						FontSize = 48,

						Text = BTN_TXT[x+y*3].ToString(),
						TextColor = FlatColors.Foreground,

						Color = FlatColors.ControlHighlight,
						ColorPressed = FlatColors.Background,
						BackgoundType = HUDBackgroundType.RoundedBlur,
						BackgoundCornerSize = 4f,

						Click = (s,e) => DoClick(BTN_TXT[idx]),
					});
				}
			}
		}

		private void DoClick(char c)
		{
			if (CharIndex >= 8) return;
			
			CharDisp[CharIndex].Character = c;

			CharIndex++;
			if (CharIndex == 8)
			{
				var number = new string(Enumerable.Range(0, 8).Select(i => CharDisp[i].Character).ToArray());
				if (__Secrets.TestUnlockCode(number))
				{
					AddHUDOperation(new UnlockSucessOperation());
				}
				else
				{
					MainGame.Inst.GDSound.PlayEffectError();
					AddHUDOperation(new UnlockErrorOperation());
				}
			}
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => true;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;
	}
}