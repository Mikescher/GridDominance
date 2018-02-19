using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common.HUD.HUDOperations;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using System.Linq;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Presenter;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class UnlockPanel : HUDRoundedPanel
	{
		public const float WIDTH = 602;
		public const float HEIGHT = 9 * GDConstants.TILE_WIDTH;

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

			var gridDisplay = new HUDFixedUniformGrid
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 96),
				GridWidth = 8,
				GridHeight = 1,
				ColumnWidth = 64,
				RowHeight = 64,
				Padding = 10,
			};
			AddElement(gridDisplay);

			for (int i = 0; i < 8; i++)
			{
				CharDisp[i] = new HUDCharacterControl(1)
				{
					Background = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.Clouds, Color.Black, 4f),

					TextPadding = 2,
					TextColor = Color.Black
				};

				gridDisplay.AddElement(i, 0, CharDisp[i]);
			}

			var pad = new HUDKeypad(3, 4, 64, 24)
			{
				Alignment = HUDAlignment.CENTER,
				RelativePosition = new FPoint(0, 176 / 2f),

				ButtonTextAlignment = HUDAlignment.CENTER,

				ButtonFont = Textures.HUDFontBold,
				ButtonFontSize = 48,

				ButtonTextColor = FlatColors.Foreground,

				ButtonBackgroundNormal = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.ControlHighlight, 4f),
				ButtonBackgroundPressed = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Background, 4f),
			};
			AddElement(pad);

			pad.AddKey('1', 0, 0);
			pad.AddKey('2', 1, 0);
			pad.AddKey('3', 2, 0);

			pad.AddKey('4', 0, 1);
			pad.AddKey('5', 1, 1);
			pad.AddKey('6', 2, 1);

			pad.AddKey('7', 0, 2);
			pad.AddKey('8', 1, 2);
			pad.AddKey('9', 2, 2);

			pad.AddKey('#', 0, 3);
			pad.AddKey('0', 1, 3);
			pad.AddKey('*', 2, 3);

			pad.PadClick += DoClick;
		}

		private void DoClick(HUDKeypad source, HUDKeypad.HUDKeypadEventArgs args)
		{
			if (CharIndex >= 8) return;
			
			CharDisp[CharIndex].Character = args.Character;

			CharIndex++;
			if (CharIndex == 8)
			{
				var number = new string(Enumerable.Range(0, 8).Select(i => CharDisp[i].Character).ToArray());
				if (__Secrets.TestUnlockCode(number))
				{
					AddOperation(new UnlockSucessOperation());
				}
				else
				{
					MainGame.Inst.GDSound.PlayEffectError();
					AddOperation(new UnlockErrorOperation());
				}
			}
		}
	}
}