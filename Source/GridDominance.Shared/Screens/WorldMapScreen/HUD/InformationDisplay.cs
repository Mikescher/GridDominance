using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.WorldMapScreen.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	public class InformationDisplay : HUDContainer
	{
		private const float SPEED_BLEND = 2.0f;
		private const float TAB_SWITCHTIME = 2.0f;

		public override int Depth => 0;

		private LevelNode node;
		private float progressDisplay = 0f;

		private float tabTimer = 0;
		private int tab = 0;

		private readonly int _header1 = L10NImpl.STR_INF_YOU;
		private readonly int _header2 = L10NImpl.STR_INF_GLOBAL;
		private readonly int _header3 = L10NImpl.STR_INF_HIGHSCORE;

		public InformationDisplay()
		{
			Alignment = HUDAlignment.BOTTOMRIGHT;
			RelativePosition = FPoint.Zero;
			Size = new FSize(350, 280);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			if (node == null) return;

			SimpleRenderHelper.DrawRoundedRect(sbatch, bounds, Color.Black * 0.8f * progressDisplay, true, false, false, false, 16);

			sbatch.DrawLine(Position + new Vector2(0, 32), Position + new Vector2(Width, 32), FlatColors.MidnightBlue * progressDisplay, 2);

			sbatch.DrawLine(Position + new Vector2(1 * (Width/3f), 0), Position + new Vector2(1 * (Width / 3f), 32), FlatColors.MidnightBlue * progressDisplay, 2);
			sbatch.DrawLine(Position + new Vector2(2 * (Width/3f), 0), Position + new Vector2(2 * (Width / 3f), 32), FlatColors.MidnightBlue * progressDisplay, 2);

			FontRenderHelper.DrawTextCentered(sbatch, (tab == 0 ? Textures.HUDFontBold : Textures.HUDFontRegular), 32, L10N.T(_header1), (tab == 0 ? FlatColors.TextHUD : FlatColors.Asbestos) * progressDisplay, Position + new Vector2(1 * (Width / 6f), 16));
			FontRenderHelper.DrawTextCentered(sbatch, (tab == 1 ? Textures.HUDFontBold : Textures.HUDFontRegular), 32, L10N.T(_header2), (tab == 1 ? FlatColors.TextHUD : FlatColors.Asbestos) * progressDisplay, Position + new Vector2(3 * (Width / 6f), 16));
			FontRenderHelper.DrawTextCentered(sbatch, (tab == 2 ? Textures.HUDFontBold : Textures.HUDFontRegular), 32, L10N.T(_header3), (tab == 2 ? FlatColors.TextHUD : FlatColors.Asbestos) * progressDisplay, Position + new Vector2(5 * (Width / 6f), 16));

			if (tab == 0)
			{
				// Best time local

				DrawInfoLine(sbatch, FractionDifficulty.DIFF_0, 0, node.LevelData.GetTimeString(FractionDifficulty.DIFF_0), true);
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_1, 1, node.LevelData.GetTimeString(FractionDifficulty.DIFF_1), true);
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_2, 2, node.LevelData.GetTimeString(FractionDifficulty.DIFF_2), true);
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_3, 3, node.LevelData.GetTimeString(FractionDifficulty.DIFF_3), true);
			}
			else if (tab == 1)
			{
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_0, 0, TimeExtension.FormatMilliseconds(node.LevelData.Data[FractionDifficulty.DIFF_0].GlobalBestTime), true);
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_1, 1, TimeExtension.FormatMilliseconds(node.LevelData.Data[FractionDifficulty.DIFF_1].GlobalBestTime), true);
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_2, 2, TimeExtension.FormatMilliseconds(node.LevelData.Data[FractionDifficulty.DIFF_2].GlobalBestTime), true);
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_3, 3, TimeExtension.FormatMilliseconds(node.LevelData.Data[FractionDifficulty.DIFF_3].GlobalBestTime), true);
			}
			else if (tab == 2)
			{
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_0, 0, PositiveOrZero(node.LevelData.Data[FractionDifficulty.DIFF_0].GlobalCompletionCount), false);
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_1, 1, PositiveOrZero(node.LevelData.Data[FractionDifficulty.DIFF_1].GlobalCompletionCount), false);
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_2, 2, PositiveOrZero(node.LevelData.Data[FractionDifficulty.DIFF_2].GlobalCompletionCount), false);
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_3, 3, PositiveOrZero(node.LevelData.Data[FractionDifficulty.DIFF_3].GlobalCompletionCount), false);
			}
		}

		private static string PositiveOrZero(int v) => v < 0 ? "0" : v.ToString();

		private void DrawInfoLine(IBatchRenderer sbatch, FractionDifficulty d, int idx, string strTime, bool colorize)
		{
			var p1 = Position + new Vector2(32,  72 + 56 * idx);
			var p2 = Position + new Vector2(64,  72 + 56 * idx);
			var p3 = Position + new Vector2(224, 72 + 56 * idx);

			var ic = (node.LevelData.HasCompleted(d) ? FractionDifficultyHelper.GetColor(d) : FlatColors.Concrete) * progressDisplay;
			var tc = (node.LevelData.HasCompleted(d) ? FlatColors.TextHUD : FlatColors.Asbestos) * progressDisplay;

			if (colorize && node.LevelData.Data[d].GlobalBestUserID >= 0 && node.LevelData.Data[d].GlobalBestUserID == MainGame.Inst.Profile.OnlineUserID)
				tc = FlatColors.SunFlower * progressDisplay;

			sbatch.DrawCentered(Textures.TexCircle, p1, 48, 48, FlatColors.WetAsphalt * progressDisplay);
			sbatch.DrawCentered(FractionDifficultyHelper.GetIcon(d), p1, 32, 32, ic);
			FontRenderHelper.DrawTextVerticallyCentered(sbatch, Textures.HUDFontRegular, 32, FractionDifficultyHelper.GetDescription(d), tc, p2);
			FontRenderHelper.DrawTextVerticallyCentered(sbatch, Textures.HUDFontRegular, 32, strTime, tc, p3);
		}

		public override void OnInitialize()
		{

		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			var sel = ((GDWorldHUD)HUD).SelectedNode;

			tabTimer += gameTime.ElapsedSeconds;
			tab = (int) (tabTimer / TAB_SWITCHTIME) % 3;

			if (sel != null && progressDisplay < 1)
			{
				tabTimer = 0;
				tab = 0;
				FloatMath.ProgressInc(ref progressDisplay, gameTime.ElapsedSeconds * SPEED_BLEND);
				node = sel;
			}
			else if (sel == null && progressDisplay > 0)
			{
				FloatMath.ProgressDec(ref progressDisplay, gameTime.ElapsedSeconds * SPEED_BLEND);
			}

			IsVisible = FloatMath.IsNotZero(progressDisplay);
		}


		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate)
		{
			return IsVisible;
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate)
		{
			return IsVisible;
		}

		protected override void OnPointerClick(FPoint relPositionPoint, InputState istate)
		{
			var tabNext = (int)(tabTimer  / TAB_SWITCHTIME + 1) % 3;
			tabTimer = tabNext * TAB_SWITCHTIME;
		}

		public void ResetCycle()
		{
			tabTimer = 0;
		}
	}
}
