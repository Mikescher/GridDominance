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

		public static FSize ToAbsSize(this Vector2 p)
		{
			return new FSize(FloatMath.Abs(p.X), FloatMath.Abs(p.Y));
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

		public static Vector2 RotateAround(this Vector2 vector2, FPoint anchor, float radians)
		{
			var cos = FloatMath.Cos(radians);
			var sin = FloatMath.Sin(radians);

			return new Vector2(anchor.X + (vector2.X - anchor.X) * cos - (vector2.Y - anchor.Y) * sin, anchor.Y + (vector2.X - anchor.X) * sin + (vector2.Y - anchor.Y) * cos);
		}

		public static Vector2 RotateWithLength(this Vector2 v, float radians, float len)
		{
			var vlen = v.Length();

			var cos = FloatMath.Cos(radians);
			var sin = FloatMath.Sin(radians);

			return new Vector2(((v.X * cos - v.Y * sin) * len) / vlen, ((v.X * sin + v.Y * cos) * len) / vlen);
		}

		public static Vector2 RotateDeg(this Vector2 vector2, float degree)
		{
			var cos = FloatMath.Cos(FloatMath.DegreesToRadians * degree);
			var sin = FloatMath.Sin(FloatMath.DegreesToRadians * degree);

			return new Vector2(vector2.X * cos - vector2.Y * sin, vector2.X * sin + vector2.Y * cos);
		}

		public static Vector2 Normalized(this Vector2 v)
		{
			return Vector2.Normalize(v);
		}
		
		public static float ToAngle(this Vector2 vector2)
		{
			//   +------------------------------->
			//   |
			//   |
			//   |          3*pi/2    
			//   |            |       
			//   |            |       
			//   |    pi -----+----- 0
			//   |            |       
			//   |            |       
			//   |            pi/2    
			//   |
			//   |
			//   v
			//
			// = Counterclockwise
			// = East : Zero
			// = Radians

			return (FloatMath.Atan2(vector2.Y, vector2.X) + FloatMath.RAD_POS_360) % FloatMath.RAD_POS_360;
		}

		public static Vector2 Truncate(this Vector2 vector2, float maxLength)
		{
			if (vector2.LengthSquared() > maxLength * maxLength)
				return vector2.Normalized() * maxLength;

			return vector2;
		}

		public static float ManhattenLength(this Vector2 v)
		{
			return FloatMath.Abs(v.X) + FloatMath.Abs(v.Y);
		}

		public static Vector2 WithLength(this Vector2 v, float l)
		{
			v = Vector2.Normalize(v);
			v.X *= l;
			v.Y *= l;
			return v;
		}

		public static Vector2 MirrorAtNormal(this Vector2 p, Vector2 linePoint, Vector2 normal)
		{
			//https://stackoverflow.com/a/6177788/1761622

			float x1 = linePoint.X - normal.Y;
			float y1 = linePoint.Y + normal.X;

			float x2 = linePoint.X + normal.Y;
			float y2 = linePoint.Y - normal.X;

			float x3 = p.X;
			float y3 = p.Y;

			float u = ((x3 - x1) * (x2 - x1) + (y3 - y1) * (y2 - y1)) / ((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));

			float xu = x1 + u * (x2 - x1);
			float yu = y1 + u * (y2 - y1);

			float dx = xu - p.X;
			float dy = yu - p.Y;

			float rx = p.X + 2 * dx;
			float ry = p.Y + 2 * dy;

			return new Vector2(rx, ry);
		}

		public static float ProjectOntoLine(this Vector2 p, Vector2 lineStart, Vector2 lineVec)
		{
			//https://stackoverflow.com/a/6177788/1761622

			float x1 = lineStart.X;
			float y1 = lineStart.Y;

			float x2 = lineStart.X + lineVec.X;
			float y2 = lineStart.Y + lineVec.Y;

			float x3 = p.X;
			float y3 = p.Y;

			float u = ((x3 - x1) * (x2 - x1) + (y3 - y1) * (y2 - y1)) / ((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));

			return u;
		}

		public static float ProjectOntoLine2(this Vector2 p, Vector2 lineStart, Vector2 lineEnd)
		{
			//https://stackoverflow.com/a/6177788/1761622

			float x1 = lineStart.X;
			float y1 = lineStart.Y;

			float x2 = lineEnd.X;
			float y2 = lineEnd.Y;

			float x3 = p.X;
			float y3 = p.Y;

			float u = ((x3 - x1) * (x2 - x1) + (y3 - y1) * (y2 - y1)) / ((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));

			return u;
		}
	}
}
