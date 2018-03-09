using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM.Dialogs;
using GridDominance.Shared.SCCM;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM.Elements
{
	class SCCMListElementOnlinePlayable : SCCMListElement
	{
		private readonly SCCMLevelMeta _meta;

		private FractionDifficulty? PersonalBest = null;

		public SCCMListElementOnlinePlayable(SCCMLevelMeta m)
		{
			_meta = m;
			PersonalBest = MainGame.Inst.Profile.GetCustomLevelPB(m.OnlineID);
		}

		public override void OnInitialize()
		{
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
			HUD.AddModal(new SCCMLevelPreviewDialog(_meta), true, 0.5f, 0.5f);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			var bottomrect = bounds.ToSubRectangleSouth(25);

			{ // background
				SimpleRenderHelper.DrawSimpleRect(sbatch, bounds, FlatColors.Clouds);
				SimpleRenderHelper.DrawSimpleRect(sbatch, bottomrect, FlatColors.Concrete);
			}

			{ // difficulty
				var tex = (PersonalBest==null) ? Textures.TexDifficultyLineNone : FractionDifficultyHelper.GetIcon(PersonalBest.Value);
				var col = (PersonalBest == null) ? FlatColors.Silver : FractionDifficultyHelper.GetColor(PersonalBest.Value);
				sbatch.DrawCentered(tex, new FPoint(bounds.Left + 5 + 16, bounds.Top + (Height - 25 - 32) / 2 + 16), 32, 32, col);
			}

			{ // name
				FontRenderHelper.DrawTextVerticallyCentered(sbatch, Textures.HUDFontBold, 32, _meta.LevelName, FlatColors.Foreground, new FPoint(bounds.Left + 5 + 32 + 5, bounds.Top + (bounds.Height - 25) / 2));
			}

			{ // user
				sbatch.DrawCentered(Textures.TexHUDIconGenericUser, new FPoint(bottomrect.Left + 5 + 32 + 5, bottomrect.CenterY), 20, 20, FlatColors.WetAsphalt);
				FontRenderHelper.DrawTextVerticallyCentered(sbatch, Textures.HUDFontBold, 20, _meta.Username??"Unknown", FlatColors.Foreground, new FPoint(bounds.Left + 5 + 32 + 5 + 16, bounds.Bottom - 12.5f));
			}


			if (_meta.GridSize != SCCMLevelData.SIZES[0]) // [XL] marker
			{
				var rr = bounds.ToSubRectangleSouthWest(32, 20);
				SimpleRenderHelper.DrawSimpleRect(sbatch, rr, FlatColors.Amethyst);
				FontRenderHelper.DrawSingleLineInBox(sbatch, Textures.HUDFontRegular, "XL", rr, 0, true, FlatColors.Foreground);
			}

			{ // star counter
				var pointPos = new FPoint(bottomrect.Right - 100, bottomrect.CenterY);
				sbatch.DrawCentered(Textures.TexIconStar, pointPos, 20, 20, FlatColors.SunFlower);
				FontRenderHelper.DrawTextVerticallyCentered(sbatch, Textures.HUDFontBold, 24, _meta.Stars.ToString(), FlatColors.MidnightBlue, pointPos + new Vector2(16, 0));
			}

			SimpleRenderHelper.DrawSimpleRectOutline(sbatch, bounds, HUD.PixelWidth, Color.Black);
		}
	}
}
