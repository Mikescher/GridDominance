using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.WorldMapScreen.HUD
{
	class InformationDisplay : HUDContainer
	{
		private const float SPEED_BLEND = 2.0f;

		public override int Depth => 0;

		private float progressDisplay = 0f;

		public InformationDisplay()
		{
			Alignment = HUDAlignment.BOTTOMRIGHT;
			RelativePosition = FPoint.Zero;
			Size = new FSize(350, 280);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			SimpleRenderHelper.DrawRoundedRect(sbatch, bounds, Color.Black * 0.8f * progressDisplay, true, false, false, false, 16);

			sbatch.DrawLine(Position + new Vector2(0, 32), Position + new Vector2(Width, 32), FlatColors.MidnightBlue * progressDisplay, 2);
			sbatch.DrawLine(Position + new Vector2(1 * (Width/3f), 0), Position + new Vector2(1 * (Width / 3f), 32), FlatColors.MidnightBlue * progressDisplay, 2);
			sbatch.DrawLine(Position + new Vector2(2 * (Width/3f), 0), Position + new Vector2(2 * (Width / 3f), 32), FlatColors.MidnightBlue * progressDisplay, 2);

			FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontRegular, 32, "You", FlatColors.TextHUD * progressDisplay, Position + new Vector2(1 * (Width / 6f), 16));
			FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontRegular, 32, "Global", FlatColors.TextHUD * progressDisplay, Position + new Vector2(3 * (Width / 6f), 16));
			FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontRegular, 32, "???", FlatColors.TextHUD * progressDisplay, Position + new Vector2(5 * (Width / 6f), 16));


			sbatch.DrawCentered(Textures.TexCircle,      Position + new Vector2(32, 72 + 56 * 0), 48, 48, FlatColors.WetAsphalt * progressDisplay);
			sbatch.DrawCentered(Textures.TexDifficulty0, Position + new Vector2(32, 72 + 56 * 0), 32, 32, GDColors.COLOR_DIFFICULTY_0 * progressDisplay);
			FontRenderHelper.DrawTextVerticallyCentered(sbatch, Textures.HUDFontRegular, 32, "Easy", FlatColors.TextHUD * progressDisplay, Position + new Vector2(32 + 32, 72 + 56 * 0));

			sbatch.DrawCentered(Textures.TexCircle,      Position + new Vector2(32, 72 + 56 * 1), 48, 48, FlatColors.WetAsphalt * progressDisplay);
			sbatch.DrawCentered(Textures.TexDifficulty1, Position + new Vector2(32, 72 + 56 * 1), 32, 32, GDColors.COLOR_DIFFICULTY_1 * progressDisplay);
			FontRenderHelper.DrawTextVerticallyCentered(sbatch, Textures.HUDFontRegular, 32, "Normal", FlatColors.TextHUD * progressDisplay, Position + new Vector2(32 + 32, 72 + 56 * 1));

			sbatch.DrawCentered(Textures.TexCircle,      Position + new Vector2(32, 72 + 56 * 2), 48, 48, FlatColors.WetAsphalt * progressDisplay);
			sbatch.DrawCentered(Textures.TexDifficulty2, Position + new Vector2(32, 72 + 56 * 2), 32, 32, GDColors.COLOR_DIFFICULTY_2 * progressDisplay);
			FontRenderHelper.DrawTextVerticallyCentered(sbatch, Textures.HUDFontRegular, 32, "Hard", FlatColors.TextHUD * progressDisplay, Position + new Vector2(32 + 32, 72 + 56 * 2));

			sbatch.DrawCentered(Textures.TexCircle,      Position + new Vector2(32, 72 + 56 * 3), 48, 48, FlatColors.WetAsphalt * progressDisplay);
			sbatch.DrawCentered(Textures.TexDifficulty3, Position + new Vector2(32, 72 + 56 * 3), 32, 32, GDColors.COLOR_DIFFICULTY_3 * progressDisplay);
			FontRenderHelper.DrawTextVerticallyCentered(sbatch, Textures.HUDFontRegular, 32, "Realistic", FlatColors.TextHUD * progressDisplay, Position + new Vector2(32 + 32, 72 + 56 * 3));
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

			if (sel != null && progressDisplay < 1)
			{
				FloatMath.ProgressInc(ref progressDisplay, gameTime.ElapsedSeconds * SPEED_BLEND);
			}
			else if (sel == null && progressDisplay > 0)
			{
				FloatMath.ProgressDec(ref progressDisplay, gameTime.ElapsedSeconds * SPEED_BLEND);
			}

			IsVisible = FloatMath.IsNotZero(progressDisplay);
		}
	}
}
