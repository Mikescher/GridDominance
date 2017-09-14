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
using MonoSAMFramework.Portable.LogProtocol;
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


		private const float WIDTH         = 350;
		private const float HEIGHT        = 280;
		private const float HEADER_HEIGHT = 32;
		private const float ROW_HEIGHT    = 48;

		private static FRectangle _rectHeader1 = new FRectangle(0 * WIDTH/3f, 0, WIDTH/3f, HEADER_HEIGHT);
		private static FRectangle _rectHeader2 = new FRectangle(1 * WIDTH/3f, 0, WIDTH/3f, HEADER_HEIGHT);
		private static FRectangle _rectHeader3 = new FRectangle(2 * WIDTH/3f, 0, WIDTH/3f, HEADER_HEIGHT);
		private static FRectangle _rectRow1    = new FRectangle(0, HEADER_HEIGHT + 16 + 0 * 56, WIDTH, ROW_HEIGHT);
		private static FRectangle _rectRow2    = new FRectangle(0, HEADER_HEIGHT + 16 + 1 * 56, WIDTH, ROW_HEIGHT);
		private static FRectangle _rectRow3    = new FRectangle(0, HEADER_HEIGHT + 16 + 2 * 56, WIDTH, ROW_HEIGHT);
		private static FRectangle _rectRow4    = new FRectangle(0, HEADER_HEIGHT + 16 + 3 * 56, WIDTH, ROW_HEIGHT);

		public override int Depth => 0;

		private LevelNode node;
		private float progressDisplay = 0f;

		private float tabTimer = 0;
		private int tab = 0;

		private readonly int _header1 = L10NImpl.STR_INF_YOU;
		private readonly int _header2 = L10NImpl.STR_INF_HIGHSCORE;
		private readonly int _header3 = L10NImpl.STR_INF_GLOBAL;

		public InformationDisplay()
		{
			Alignment = HUDAlignment.BOTTOMRIGHT;
			RelativePosition = FPoint.Zero;
			Size = new FSize(WIDTH, HEIGHT);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			if (node == null) return;

			SimpleRenderHelper.DrawRoundedRect(sbatch, bounds, Color.Black * 0.8f * progressDisplay, true, false, false, false, 16);

			sbatch.DrawLine(Position + new Vector2(0, HEADER_HEIGHT), Position + new Vector2(Width, HEADER_HEIGHT), FlatColors.MidnightBlue * progressDisplay, 2);

			sbatch.DrawLine(Position + new Vector2(1 * (Width/3f), 0), Position + new Vector2(1 * (Width / 3f), 32), FlatColors.MidnightBlue * progressDisplay, 2);
			sbatch.DrawLine(Position + new Vector2(2 * (Width/3f), 0), Position + new Vector2(2 * (Width / 3f), 32), FlatColors.MidnightBlue * progressDisplay, 2);

			FontRenderHelper.DrawTextCentered(sbatch, (tab == 0 ? Textures.HUDFontBold : Textures.HUDFontRegular), 32, L10N.T(_header1), (tab == 0 ? FlatColors.TextHUD : FlatColors.Asbestos) * progressDisplay, Position + new Vector2(1 * (Width / 6f), HEADER_HEIGHT / 2f));
			FontRenderHelper.DrawTextCentered(sbatch, (tab == 1 ? Textures.HUDFontBold : Textures.HUDFontRegular), 32, L10N.T(_header2), (tab == 1 ? FlatColors.TextHUD : FlatColors.Asbestos) * progressDisplay, Position + new Vector2(3 * (Width / 6f), HEADER_HEIGHT / 2f));
			FontRenderHelper.DrawTextCentered(sbatch, (tab == 2 ? Textures.HUDFontBold : Textures.HUDFontRegular), 32, L10N.T(_header3), (tab == 2 ? FlatColors.TextHUD : FlatColors.Asbestos) * progressDisplay, Position + new Vector2(5 * (Width / 6f), HEADER_HEIGHT / 2f));

			if (tab == 0)
			{
				// Best time local

				DrawInfoLine(sbatch, FractionDifficulty.DIFF_0, 0, node.LevelData.GetTimeString(FractionDifficulty.DIFF_0, true), true);
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_1, 1, node.LevelData.GetTimeString(FractionDifficulty.DIFF_1, true), true);
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_2, 2, node.LevelData.GetTimeString(FractionDifficulty.DIFF_2, true), true);
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_3, 3, node.LevelData.GetTimeString(FractionDifficulty.DIFF_3, true), true);
			}
			else if (tab == 1)
			{
				// Best time global
				
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_0, 0, TimeExtension.FormatMilliseconds(node.LevelData.Data[FractionDifficulty.DIFF_0].GlobalBestTime, true), true);
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_1, 1, TimeExtension.FormatMilliseconds(node.LevelData.Data[FractionDifficulty.DIFF_1].GlobalBestTime, true), true);
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_2, 2, TimeExtension.FormatMilliseconds(node.LevelData.Data[FractionDifficulty.DIFF_2].GlobalBestTime, true), true);
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_3, 3, TimeExtension.FormatMilliseconds(node.LevelData.Data[FractionDifficulty.DIFF_3].GlobalBestTime, true), true);
			}
			else if (tab == 2)
			{
				// global count
				
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_0, 0, PositiveOrZero(node.LevelData.Data[FractionDifficulty.DIFF_0].GlobalCompletionCount), false);
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_1, 1, PositiveOrZero(node.LevelData.Data[FractionDifficulty.DIFF_1].GlobalCompletionCount), false);
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_2, 2, PositiveOrZero(node.LevelData.Data[FractionDifficulty.DIFF_2].GlobalCompletionCount), false);
				DrawInfoLine(sbatch, FractionDifficulty.DIFF_3, 3, PositiveOrZero(node.LevelData.Data[FractionDifficulty.DIFF_3].GlobalCompletionCount), false);
			}
		}

		private static string PositiveOrZero(int v) => v < 0 ? "0" : v.ToString();

		private void DrawInfoLine(IBatchRenderer sbatch, FractionDifficulty d, int idx, string strTime, bool colorize)
		{
			var p1 = Position + new Vector2(32,  HEADER_HEIGHT + 40 + 56 * idx);
			var p2 = Position + new Vector2(64,  HEADER_HEIGHT + 40 + 56 * idx);
			var p3 = Position + new Vector2(224, HEADER_HEIGHT + 40 + 56 * idx);

			var ic = (node.LevelData.HasCompletedOrBetter(d) ? FractionDifficultyHelper.GetColor(d) : FlatColors.Concrete) * progressDisplay;
			var tc = (node.LevelData.HasCompletedOrBetter(d) ? FlatColors.TextHUD : FlatColors.Asbestos) * progressDisplay;

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
			else if (sel != null && node != null && sel != node)
			{
				node = sel;
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
			if (_rectHeader1.Contains(relPositionPoint))
			{
				var tabNext = 0;
				tabTimer = tabNext * TAB_SWITCHTIME;
			}
			else if (_rectHeader2.Contains(relPositionPoint))
			{
				var tabNext = 1;
				tabTimer = tabNext * TAB_SWITCHTIME;
			}
			else if (_rectHeader3.Contains(relPositionPoint))
			{
				var tabNext = 2;
				tabTimer = tabNext * TAB_SWITCHTIME;
			}
			else if (_rectRow1.Contains(relPositionPoint))
			{
				tabTimer = tab * TAB_SWITCHTIME;

				switch (tab)
				{
					case 0:
						if (node.LevelData.HasCompletedExact(FractionDifficulty.DIFF_0))
							Toast_1(L10N.TF(L10NImpl.STR_INFOTOAST_1, node.LevelData.GetTimeString(FractionDifficulty.DIFF_0, true)));
						else
							Toast_4(L10N.TF(L10NImpl.STR_INFOTOAST_4, FractionDifficultyHelper.GetDescription(FractionDifficulty.DIFF_0)));
						break;
						
					case 1:
						Toast_2(L10N.TF(L10NImpl.STR_INFOTOAST_2, TimeExtension.FormatMilliseconds(node.LevelData.Data[FractionDifficulty.DIFF_0].GlobalBestTime, true)));
						break;

					case 2:
						Toast_3(L10N.TF(L10NImpl.STR_INFOTOAST_3, PositiveOrZero(node.LevelData.Data[FractionDifficulty.DIFF_0].GlobalCompletionCount), FractionDifficultyHelper.GetDescription(FractionDifficulty.DIFF_0)));
						break;

					default:
						SAMLog.Error("INFD::EnumSwitch_OPC1", "value: " + tab);
						break;
				}
			}
			else if (_rectRow2.Contains(relPositionPoint))
			{
				tabTimer = tab * TAB_SWITCHTIME;

				switch (tab)
				{
					case 0:
						if (node.LevelData.HasCompletedExact(FractionDifficulty.DIFF_1))
							Toast_1(L10N.TF(L10NImpl.STR_INFOTOAST_1, node.LevelData.GetTimeString(FractionDifficulty.DIFF_1, true)));
						else
							Toast_4(L10N.TF(L10NImpl.STR_INFOTOAST_4, FractionDifficultyHelper.GetDescription(FractionDifficulty.DIFF_1)));
						break;
						
					case 1:
						Toast_2(L10N.TF(L10NImpl.STR_INFOTOAST_2, TimeExtension.FormatMilliseconds(node.LevelData.Data[FractionDifficulty.DIFF_1].GlobalBestTime, true)));
						break;

					case 2:
						Toast_3(L10N.TF(L10NImpl.STR_INFOTOAST_3, PositiveOrZero(node.LevelData.Data[FractionDifficulty.DIFF_1].GlobalCompletionCount), FractionDifficultyHelper.GetDescription(FractionDifficulty.DIFF_1)));
						break;


					default:
						SAMLog.Error("INFD::EnumSwitch_OPC2", "value: " + tab);
						break;
				}
			}
			else if (_rectRow3.Contains(relPositionPoint))
			{
				tabTimer = tab * TAB_SWITCHTIME;

				switch (tab)
				{
					case 0:
						if (node.LevelData.HasCompletedExact(FractionDifficulty.DIFF_2))
							Toast_1(L10N.TF(L10NImpl.STR_INFOTOAST_1, node.LevelData.GetTimeString(FractionDifficulty.DIFF_2, true)));
						else
							Toast_4(L10N.TF(L10NImpl.STR_INFOTOAST_4, FractionDifficultyHelper.GetDescription(FractionDifficulty.DIFF_2)));
						break;
						
					case 1:
						Toast_1(L10N.TF(L10NImpl.STR_INFOTOAST_2, TimeExtension.FormatMilliseconds(node.LevelData.Data[FractionDifficulty.DIFF_2].GlobalBestTime, true)));
						break;

					case 2:
						Toast_2(L10N.TF(L10NImpl.STR_INFOTOAST_3, PositiveOrZero(node.LevelData.Data[FractionDifficulty.DIFF_2].GlobalCompletionCount), FractionDifficultyHelper.GetDescription(FractionDifficulty.DIFF_2)));
						break;


					default:
						SAMLog.Error("INFD::EnumSwitch_OPC3", "value: " + tab);
						break;
				}
			}
			else if (_rectRow4.Contains(relPositionPoint))
			{
				tabTimer = tab * TAB_SWITCHTIME;

				switch (tab)
				{
					case 0:
						if (node.LevelData.HasCompletedExact(FractionDifficulty.DIFF_3))
							Toast_1(L10N.TF(L10NImpl.STR_INFOTOAST_1, node.LevelData.GetTimeString(FractionDifficulty.DIFF_3, true)));
						else
							Toast_4(L10N.TF(L10NImpl.STR_INFOTOAST_4, FractionDifficultyHelper.GetDescription(FractionDifficulty.DIFF_3)));
						break;
						
					case 1:
						Toast_2(L10N.TF(L10NImpl.STR_INFOTOAST_2, TimeExtension.FormatMilliseconds(node.LevelData.Data[FractionDifficulty.DIFF_3].GlobalBestTime, true)));
						break;

					case 2:
						Toast_3(L10N.TF(L10NImpl.STR_INFOTOAST_3, PositiveOrZero(node.LevelData.Data[FractionDifficulty.DIFF_3].GlobalCompletionCount), FractionDifficultyHelper.GetDescription(FractionDifficulty.DIFF_3)));
						break;


					default:
						SAMLog.Error("INFD::EnumSwitch_OPC4", "value: " + tab);
						break;
				}
			}
			else
			{
				var tabNext = (int)(tabTimer / TAB_SWITCHTIME + 1) % 3;
				tabTimer = tabNext * TAB_SWITCHTIME;
			}

		}

		private void Toast_1(string txt)
		{
			HUD.ShowToast("INFD::T1", txt, 32, FlatColors.Silver, FlatColors.Foreground, 3f);
		}

		private void Toast_2(string txt)
		{
			HUD.ShowToast("INFD::T2", txt, 32, FlatColors.Silver, FlatColors.Foreground, 3f);
		}

		private void Toast_3(string txt)
		{
			HUD.ShowToast("INFD::T3", txt, 32, FlatColors.Silver, FlatColors.Foreground, 3f);
		}

		private void Toast_4(string txt)
		{
			HUD.ShowToast("INFD::T4", txt, 32, FlatColors.Silver, FlatColors.Foreground, 3f);
		}

		public void ResetCycle()
		{
			tabTimer = 0;
		}
	}
}
