using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath;

namespace MonoSAMFramework.Portable.RenderHelper
{
	public static class DebugRenderHelper
	{
		public static void DrawCrossedCircle(IBatchRenderer sbatch, Color color, Vector2 pos, float radius, float thickness = 1)
		{
			sbatch.DrawCircle(pos, radius, 32, color, thickness);

			var offset = radius * FloatMath.SQRT_ONE_HALF;

			sbatch.DrawLine(pos.X - offset, pos.Y - offset, pos.X + offset, pos.Y + offset, color, thickness);
			sbatch.DrawLine(pos.X - offset, pos.Y + offset, pos.X + offset, pos.Y - offset, color, thickness);
		}

		public static void DrawHalfCrossedCircle(IBatchRenderer sbatch, Color color, Vector2 pos, float radius, float thickness = 1)
		{
			sbatch.DrawCircle(pos, radius, 32, color, thickness);

			var offset = radius * FloatMath.SQRT_ONE_HALF;

			sbatch.DrawLine(pos.X - offset, pos.Y + offset, pos.X + offset, pos.Y - offset, color, thickness);
		}
	}
}
