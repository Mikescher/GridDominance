using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Dynamics;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.RenderHelper;

namespace GridDominance.Shared.Screens.Common
{
	public static class CommonObstacleRenderer
	{
		#region BlackHole

		public static void DrawBlackHole(IBatchRenderer sbatch, FPoint position, float animationTimer, float diameter, float power)
		{
			var fillColor = (power < 0) ? Color.Black : Color.White;

			sbatch.DrawCentered(Textures.TexCircle,  position, diameter, diameter, fillColor);
			sbatch.DrawCentered(Textures.TexVortex2, position, diameter, diameter, Color.Gray);
			sbatch.DrawCentered(Textures.TexVortex1, position, diameter * 0.666f, diameter * 0.666f, Color.Gray);
			sbatch.DrawCentered(Textures.TexVortex0, position, diameter * 0.333f, diameter * 0.333f, Color.Gray);

			float animProgress = (animationTimer % BlackHole.ANIMATION_DURATION) / BlackHole.ANIMATION_DURATION;

			if (power < 0)
			{
				if (animProgress < 2 / 4f)
					DrawBlackHoleAnimation0_In(sbatch, position, diameter, animProgress * 2 - 0);
				else if (animProgress < 3 / 4f)
					DrawBlackHoleAnimation1_In(sbatch, position, diameter, animProgress * 4 - 2);
				else if (animProgress < 4 / 4f)
					DrawBlackHoleAnimation2_In(sbatch, position, diameter, animProgress * 4 - 3);
			}
			else
			{
				if (animProgress < 1 / 3f)
					DrawBlackHoleAnimation0_Out(sbatch, position, diameter, animProgress * 3 - 0);
				else if (animProgress < 2 / 3f)
					DrawBlackHoleAnimation1_Out(sbatch, position, diameter, animProgress * 3 - 1);
				else if (animProgress < 3 / 3f)
					DrawBlackHoleAnimation2_Out(sbatch, position, diameter, animProgress * 3 - 2);
			}
		}

		private static void DrawBlackHoleAnimation0_Out(IBatchRenderer sbatch, FPoint position, float diameter, float progress)
		{
			var tfProgress = 1 + FloatMath.FunctionEaseOutQuad(progress);

			sbatch.DrawCentered(Textures.TexVortex0, position, diameter * 0.333f * tfProgress, diameter * 0.333f * tfProgress, Color.DarkGray * (1 - progress));
		}

		private static void DrawBlackHoleAnimation1_Out(IBatchRenderer sbatch, FPoint position, float diameter, float progress)
		{
			var tfProgress = 2 + FloatMath.FunctionEaseOutQuad(progress);

			sbatch.DrawCentered(Textures.TexVortex1, position, diameter * 0.333f * tfProgress, diameter * 0.333f * tfProgress, Color.DarkGray * (1 - progress));
		}

		private static void DrawBlackHoleAnimation2_Out(IBatchRenderer sbatch, FPoint position, float diameter, float progress)
		{
			var tfProgress = 3 + FloatMath.FunctionEaseOutQuad(progress);

			sbatch.DrawCentered(Textures.TexVortex2, position, diameter * 0.333f * tfProgress, diameter * 0.333f * tfProgress, Color.DarkGray * (1 - progress));
		}

		private static void DrawBlackHoleAnimation0_In(IBatchRenderer sbatch, FPoint position, float diameter, float progress)
		{
			var tfProgress = 5 - 2 * FloatMath.FunctionEaseInQuad(progress);

			sbatch.DrawCentered(Textures.TexVortex2, position, diameter * 0.333f * tfProgress, diameter * 0.333f * tfProgress, Color.LightGray * FloatMath.FunctionEaseInQuart(progress));
		}

		private static void DrawBlackHoleAnimation1_In(IBatchRenderer sbatch, FPoint position, float diameter, float progress)
		{
			var tfProgress = 3 - FloatMath.FunctionEaseInQuad(progress);

			sbatch.DrawCentered(Textures.TexVortex1, position, diameter * 0.333f * tfProgress, diameter * 0.333f * tfProgress, Color.LightGray * progress);
		}

		private static void DrawBlackHoleAnimation2_In(IBatchRenderer sbatch, FPoint position, float diameter, float progress)
		{
			var tfProgress = 2 - FloatMath.FunctionEaseInQuad(progress);

			sbatch.DrawCentered(Textures.TexVortex0, position, diameter * 0.333f * tfProgress, diameter * 0.333f * tfProgress, Color.LightGray * progress);
		}

		#endregion

		#region VoidCircle

		public static void DrawVoidCircle_BG(IBatchRenderer sbatch, FPoint position, float diameter)
		{
			sbatch.DrawCentered(Textures.TexVoidCircle_BG, position, diameter + 2 * VoidCircle.MARGIN_TEX, diameter + 2 * VoidCircle.MARGIN_TEX, Color.White);
		}

		public static void DrawVoidCircle_FG(IBatchRenderer sbatch, FPoint position, float diameter)
		{
			sbatch.DrawCentered(Textures.TexVoidCircle_FG, position, diameter + 2 * VoidCircle.MARGIN_TEX, diameter + 2 * VoidCircle.MARGIN_TEX, Color.White);
		}

		#endregion

		#region VoidCircle

		public static void DrawMirrorCircle(IBatchRenderer sbatch, FPoint position, float diameter)
		{
			sbatch.DrawCentered(
				diameter < 96 ? Textures.TexMirrorCircleSmall : Textures.TexMirrorCircleBig, 
				position, 
				diameter + 2 * MirrorCircle.MARGIN_TEX, 
				diameter + 2 * MirrorCircle.MARGIN_TEX, 
				Color.White);
		}

		#endregion

		#region MirrorBlock

		public static void DrawMirrorBlock(IBatchRenderer sbatch, FRotatedRectangle rect)
		{
			SimpleRenderHelper.Draw9Patch(
				sbatch,
				rect,
				Color.White, Color.White, FlatColors.Concrete,
				Textures.TexMirrorBlockEdge, Textures.TexMirrorBlockCorner, Textures.TexPixel,
				MirrorBlock.CORNER_SIZE);
		}

		#endregion

		#region GlassBlock

		public static void DrawGlassBlock(IBatchRenderer sbatch, FRotatedRectangle rect)
		{
			SimpleRenderHelper.Draw9Patch(
				sbatch,
				rect,
				Color.White, Color.White, Color.White,
				Textures.TexGlassEdge, Textures.TexGlassCorner, Textures.TexGlassFill,
				GlassBlock.CORNER_SIZE);
		}

		#endregion

	}
}
