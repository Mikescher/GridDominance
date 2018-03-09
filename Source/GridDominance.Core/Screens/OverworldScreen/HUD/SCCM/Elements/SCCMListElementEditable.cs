using GridDominance.Shared.Resources;
using GridDominance.Shared.SCCM;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM.Elements
{
	class SCCMListElementEditable : SCCMListElement
	{
		private readonly SCCMLevelData _data;

		public SCCMListElementEditable(SCCMLevelData d)
		{
			_data = d;
		}

		public override void OnInitialize()
		{
			AddElement(new HUDImage
			{
				RelativePosition = new FPoint(5, (Height - 25 - 32)/2),
				Size = new FSize(32, 32),
				Alignment = HUDAlignment.TOPLEFT,

				Image = Textures.CannonCog,
				ImageAlignment = HUDImageAlignmentAlgorithm.CENTER,
				ImageScale     = HUDImageScaleAlgorithm.STRETCH,
				RotationSpeed = 0.25f,
				Color = FlatColors.SunFlower,
			});

			AddElement(new HUDTextButton
			{
				RelativePosition = new FPoint(5, 5),
				Size = new FSize(192, 32),
				Alignment = HUDAlignment.TOPRIGHT,

				L10NText = L10NImpl.STR_LVLED_BTN_EDIT,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 28,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				Click = OnEdit,

				BackgroundNormal = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),
			});
		}

		private void OnEdit(HUDTextButton sender, HUDButtonEventArgs e)
		{
			MainGame.Inst.SetLevelEditorScreen(_data);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			var bottomrect = bounds.ToSubRectangleSouth(25);

			SimpleRenderHelper.DrawSimpleRect(sbatch, bounds, FlatColors.Clouds);
			SimpleRenderHelper.DrawSimpleRect(sbatch, bottomrect, FlatColors.Concrete);

			FontRenderHelper.DrawTextVerticallyCentered(sbatch, Textures.HUDFontBold, 32, _data.Name, FlatColors.Foreground, new FPoint(bounds.Left+5+32+5, bounds.Top+(bounds.Height-25)/2));
			FontRenderHelper.DrawTextVerticallyCentered(sbatch, Textures.HUDFontBold, 20, MainGame.Inst.Profile.OnlineUsername, FlatColors.Foreground, new FPoint(bounds.Left+5+32+5+16, bounds.Bottom-12.5f));

			sbatch.DrawCentered(Textures.TexHUDIconGenericUser, new FPoint(bottomrect.Left + 5 + 32 + 5, bottomrect.CenterY), 20, 20, FlatColors.WetAsphalt);

			if (_data.Size != SCCMLevelData.SIZES[0])
			{
				var rr = bounds.ToSubRectangleSouthWest(32, 20);
				SimpleRenderHelper.DrawSimpleRect(sbatch, rr, FlatColors.Amethyst);
				FontRenderHelper.DrawSingleLineInBox(sbatch, Textures.HUDFontRegular, "XL", rr, 0, true, FlatColors.Foreground);
			}

			SimpleRenderHelper.DrawSimpleRectOutline(sbatch, bounds, HUD.PixelWidth, Color.Black);
		}
	}
}
