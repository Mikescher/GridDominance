using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.MathHelper;

namespace MonoSAMFramework.Portable.Extensions
{
	public static class PointExtension
	{
		public static float Length(this Point p)
		{
			return FloatMath.Sqrt(p.X * p.X + p.Y * p.Y);
		}

		public static float LengthSquared(this Point p)
		{
			return p.X * p.X + p.Y * p.Y;
		}
	}
}
