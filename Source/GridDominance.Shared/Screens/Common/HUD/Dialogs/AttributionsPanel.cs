using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.RenderHelper;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class AttributionsPanel : HUDRoundedPanel
	{
		public const float WIDTH = 13 * GDConstants.TILE_WIDTH;
		public const float HEIGHT = 9 * GDConstants.TILE_WIDTH;

		public const float LINE_WIDTH  = 6 * GDConstants.TILE_WIDTH;
		public const float LINE_OFFSET = (19/6f) * GDConstants.TILE_WIDTH;

		public override int Depth => 0;

		public AttributionsPanel()
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

				L10NText = L10NImpl.STR_ATTRIBUTIONS,
				TextColor = FlatColors.Clouds,
			});

			for (int i = 0; i < Attributions.LINK_BACK.Length; i++)
			{
				var dd = Attributions.LINK_BACK[i];

				var btn = new HUDTextButton(1)
				{
					TextAlignment = HUDAlignment.CENTERLEFT,
					Alignment = HUDAlignment.TOPCENTER,
					RelativePosition = new FPoint(((i % 2) * 2 - 1) * LINE_OFFSET, 96 + (i / 2) * 48),
					Size = new FSize(LINE_WIDTH, 32),

					Font = Textures.HUDFontRegular,
					FontSize = 24,

					Text = dd.Item1,

					TextColor = FlatColors.Foreground,
					TextPadding = 16,

					BackgroundNormal  = HUDBackgroundDefinition.CreateSimpleBlur(FlatColors.TextHUD, 16),
					BackgroundPressed = HUDBackgroundDefinition.CreateSimpleBlur(FlatColors.ControlHighlight, 16),

					ClickMode = HUDButton.HUDButtonClickMode.Single,
				};

				btn.ButtonClick += (s, a) =>
				{
					HUD.ShowToast(null, dd.Item2, 32, FlatColors.Silver, FlatColors.Foreground, 3f);
					MainGame.Inst.Bridge.OpenURL(dd.Item2);
				};

				AddElement(btn);
			}
		}
	}
}