using System;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SCCM;
using GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM.Dialogs;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM
{
	class SCCMListElementLocalPlayable : SCCMListElement
	{
		private SCCMLevelMeta _meta = null;
		private readonly LevelBlueprint _level;

		public SCCMListElementLocalPlayable(LevelBlueprint bp)
		{
			_level = bp;
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

				L10NText = L10NImpl.STR_LVLED_BTN_PLAY,
				TextColor = Color.White,
				Font = Textures.HUDFontRegular,
				FontSize = 28,
				TextAlignment = HUDAlignment.CENTER,
				TextPadding = 8,

				Click = OnPlay,

				BackgroundNormal = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),
			});
		}

		private void OnPlay(HUDTextButton sender, HUDButtonEventArgs e)
		{
			HUD.AddModal(new SCCMLevelPreviewDialog(_meta, _level), true, 0.5f, 0.5f);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			var bottomrect = bounds.ToSubRectangleSouth(25);

			{ // background
				SimpleRenderHelper.DrawSimpleRect(sbatch, bounds, FlatColors.Clouds);
				SimpleRenderHelper.DrawSimpleRect(sbatch, bottomrect, FlatColors.Concrete);
			}

			{ // name
				FontRenderHelper.DrawTextVerticallyCentered(sbatch, Textures.HUDFontBold, 32, _meta?.LevelName ?? _level?.FullName, FlatColors.Foreground, new FPoint(bounds.Left + 5 + 32 + 5, bounds.Top + (bounds.Height - 25) / 2));
			}

			{ // user
				sbatch.DrawCentered(Textures.TexHUDIconGenericUser, new FPoint(bottomrect.Left + 5 + 32 + 5, bottomrect.CenterY), 20, 20, FlatColors.WetAsphalt);
				FontRenderHelper.DrawTextVerticallyCentered(sbatch, Textures.HUDFontBold, 20, (_meta==null) ? L10N.T(L10NImpl.STR_INF_YOU) : (_meta.Username??"Unknown"), FlatColors.Foreground, new FPoint(bounds.Left + 5 + 32 + 5 + 16, bounds.Bottom - 12.5f));
			}


			if ((_meta != null) && _meta.GridSize != SCCMLevelData.SIZES[0]) // [XL] marker
			{
				var rr = bounds.ToSubRectangleSouthWest(32, 20);
				SimpleRenderHelper.DrawSimpleRect(sbatch, rr, FlatColors.Amethyst);
				FontRenderHelper.DrawSingleLineInBox(sbatch, Textures.HUDFontRegular, "XL", rr, 0, true, FlatColors.Foreground);
			}

			{ // star counter
				var pointPos = new FPoint(bottomrect.Right - 100, bottomrect.CenterY);
				sbatch.DrawCentered(Textures.TexIconStar, pointPos, 20, 20, FlatColors.SunFlower);
				FontRenderHelper.DrawTextVerticallyCentered(sbatch, Textures.HUDFontBold, 24, (_meta==null) ? "???" : _meta.Stars.ToString(), FlatColors.MidnightBlue, pointPos + new Vector2(16, 0));
			}

			SimpleRenderHelper.DrawSimpleRectOutline(sbatch, bounds, HUD.PixelWidth, Color.Black);
		}

		public void SetMeta(SCCMLevelMeta m)
		{
			_meta = m;
		}
	}
}
