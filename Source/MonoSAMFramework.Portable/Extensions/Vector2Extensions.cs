using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.Extensions
{
	public static class Vector2Extensions
	{
		public static FSize ToSize(this Vector2 p)
		{
			return new FSize(p.X, p.Y);
		}

		public static FPoint ToFPoint(this Vector2 p)
		{
			return new FPoint(p.X, p.Y);
		}

		public static bool IsZero(this Vector2 p)
		{
			return FloatMath.IsZero(p.X) && FloatMath.IsZero(p.Y);
		}

		public static bool EpsilonEquals(this Vector2 vector2, Vector2 other, float eps = 0.00001f)
		{
			return FloatMath.Abs(vector2.X - other.X) <= eps && FloatMath.Abs(vector2.Y - other.Y) <= eps;
		}

		public static Vector2 Rotate(this Vector2 vector2, float radians)
		{
			var cos = FloatMath.Cos(radians);
			var sin = FloatMath.Sin(radians);

			return new Vector2(vector2.X * cos - vector2.Y * sin, vector2.X * sin + vector2.Y * cos);
		}

		public static Vector2 Normalized(this Vector2 v)
		{
			return Vector2.Normalize(v);
		}
		
		public static float ToAngle(this Vector2 vector2)
		{
			return FloatMath.Atan2(vector2.X, -vector2.Y);
		}

		public static Vector2 Truncate(this Vector2 vector2, float maxLength)
		{
			if (vector2.LengthSquared() > maxLength * maxLength)
				return vector2.Normalized() * maxLength;

			return vector2;
		}
	}
}
