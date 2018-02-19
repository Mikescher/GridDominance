using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.RenderHelper;

namespace GridDominance.Shared.Screens.Common
{
	public static class CommonWallRenderer
	{
		#region VoidWall

		public static void DrawVoidWall_BG(IBatchRenderer sbatch, float len, float rotation, FRectangle[] rects)
		{
			var renderPartCount = FloatMath.Round(len / GDConstants.TILE_WIDTH);

			if (renderPartCount <= 1)
			{
				sbatch.DrawStretched(Textures.TexVoidWall_BG_L1, rects[0], Color.White, rotation);
			}
			else if (renderPartCount == 2)
			{
				sbatch.DrawStretched(Textures.TexVoidWall_BG_L2, rects[0], Color.White, rotation);
			}
			else
			{
				for (int i = 0; i < renderPartCount; i++)
				{
					if (i == 0)
					{
						sbatch.DrawStretched(Textures.TexVoidWall_BG_End, rects[i], Color.White, rotation + FloatMath.RAD_POS_180);
					}
					else if (i + 1 < renderPartCount)
					{
						sbatch.DrawStretched(Textures.TexVoidWall_BG_Middle, rects[i], Color.White, rotation);
					}
					else
					{
						sbatch.DrawStretched(Textures.TexVoidWall_BG_End, rects[i], Color.White, rotation);
					}
				}
			}
		}

		public static void DrawVoidWall_FG(IBatchRenderer sbatch, float len, float rotation, FRectangle[] rects)
		{
			var renderPartCount = FloatMath.Round(len / GDConstants.TILE_WIDTH);

			if (renderPartCount <= 1)
			{
				sbatch.DrawStretched(Textures.TexVoidWall_FG_L1, rects[0], Color.White, rotation);
			}
			else if (renderPartCount == 2)
			{
				sbatch.DrawStretched(Textures.TexVoidWall_FG_L2, rects[0], Color.White, rotation);
			}
			else
			{
				for (int i = 0; i < renderPartCount; i++)
				{
					if (i == 0)
					{
						sbatch.DrawStretched(Textures.TexVoidWall_FG_End, rects[i], Color.White, rotation + FloatMath.RAD_POS_180);
					}
					else if (i + 1 < renderPartCount)
					{
						sbatch.DrawStretched(Textures.TexVoidWall_FG_Middle, rects[i], Color.White, rotation);
					}
					else
					{
						sbatch.DrawStretched(Textures.TexVoidWall_FG_End, rects[i], Color.White, rotation);
					}
				}
			}
		}

		public static FRectangle[] CreateVoidWallRenderRects(FPoint pos, float len, float rotation)
		{
			var pc = FloatMath.Round(len / GDConstants.TILE_WIDTH);

			if (pc <= 2)
			{
				return new[]
				{
					FRectangle.CreateByCenter(pos, VoidWall.MARGIN_TEX + len + VoidWall.MARGIN_TEX, VoidWall.MARGIN_TEX + VoidWall.WIDTH + VoidWall.MARGIN_TEX)
				};
			}
			else
			{
				var partlen = len / pc;

				var dir = Vector2.UnitX.RotateWithLength(rotation, 1);

				var result = new FRectangle[pc];
				for (int i = 0; i < pc; i++)
				{
					var di = i - (pc - 1) / 2f;

					if (i == 0)
					{
						var off = dir * (di * partlen - VoidWall.MARGIN_TEX / 2f);
						result[i] = FRectangle.CreateByCenter(pos + off, VoidWall.MARGIN_TEX + partlen, VoidWall.MARGIN_TEX + VoidWall.WIDTH + VoidWall.MARGIN_TEX);
					}
					else if (i + 1 < pc)
					{
						var off = dir * di * partlen;
						result[i] = FRectangle.CreateByCenter(pos + off, partlen, VoidWall.MARGIN_TEX + VoidWall.WIDTH + VoidWall.MARGIN_TEX);
					}
					else
					{
						var off = dir * (di * partlen + VoidWall.MARGIN_TEX / 2f);
						result[i] = FRectangle.CreateByCenter(pos + off, partlen + VoidWall.MARGIN_TEX, VoidWall.MARGIN_TEX + VoidWall.WIDTH + VoidWall.MARGIN_TEX);
					}
				}
				return result;
			}
		}

		#endregion

		#region MirrorWall

		public static void DrawMirrorWall(IBatchRenderer sbatch, FRotatedRectangle rect)
		{
			SimpleRenderHelper.Draw9Patch(
				sbatch,
				rect,
				Color.White, Color.White, FlatColors.Concrete,
				Textures.TexMirrorBlockEdge, Textures.TexMirrorBlockCorner, Textures.TexPixel,
				MirrorBlock.CORNER_SIZE);
		}

		#endregion

		#region GlassWall

		public static void DrawGlassWall(IBatchRenderer sbatch, FRotatedRectangle rect)
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
