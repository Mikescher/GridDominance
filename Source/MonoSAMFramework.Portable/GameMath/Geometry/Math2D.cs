using System;
using Microsoft.Xna.Framework;

namespace MonoSAMFramework.Portable.GameMath.Geometry
{
	public static class Math2D
	{
		/// <summary>
		/// Returns the intersection point of the given lines. 
		/// Returns Empty if the lines do not intersect.
		/// Source: http://mathworld.wolfram.com/Line-LineIntersection.html
		///         https://stackoverflow.com/a/385828/1761622
		/// </summary>
		public static bool LineIntersection(FPoint v1, FPoint v2, FPoint v3, FPoint v4, out FPoint intersec)
		{
			float a = Det2(v1.X - v2.X, v1.Y - v2.Y, v3.X - v4.X, v3.Y - v4.Y);
			if (FloatMath.Abs(a) < FloatMath.EPSILON) { intersec = FPoint.Zero; return false; } // Lines are parallel

			float d1 = Det2(v1.X, v1.Y,        v2.X, v2.Y);
			float d2 = Det2(v3.X, v3.Y,        v4.X, v4.Y);
			float x  = Det2(d1,   v1.X - v2.X, d2,   v3.X - v4.X) / a;
			float y  = Det2(d1,   v1.Y - v2.Y, d2,   v3.Y - v4.Y) / a;

			intersec = new FPoint(x, y);
			
			if (x < FloatMath.Min(v1.X, v2.X) - FloatMath.EPSILON || x > FloatMath.Max(v1.X, v2.X) + FloatMath.EPSILON) return false;
			if (y < FloatMath.Min(v1.Y, v2.Y) - FloatMath.EPSILON || y > FloatMath.Max(v1.Y, v2.Y) + FloatMath.EPSILON) return false;
			if (x < FloatMath.Min(v3.X, v4.X) - FloatMath.EPSILON || x > FloatMath.Max(v3.X, v4.X) + FloatMath.EPSILON) return false;
			if (y < FloatMath.Min(v3.Y, v4.Y) - FloatMath.EPSILON || y > FloatMath.Max(v3.Y, v4.Y) + FloatMath.EPSILON) return false;

			return true;
		}

		/// <summary>
		/// Returns the intersection point of the given lines. 
		/// Returns Empty if the lines do not intersect.
		/// Source: http://mathworld.wolfram.com/Line-LineIntersection.html
		///         https://stackoverflow.com/a/385828/1761622
		/// </summary>
		public static bool LineIntersection(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4, out FPoint intersec)
		{
			float a = Det2(v1.X - v2.X, v1.Y - v2.Y, v3.X - v4.X, v3.Y - v4.Y);
			if (FloatMath.Abs(a) < FloatMath.EPSILON) { intersec = FPoint.Zero; return false; } // Lines are parallel

			float d1 = Det2(v1.X, v1.Y, v2.X, v2.Y);
			float d2 = Det2(v3.X, v3.Y, v4.X, v4.Y);
			float x = Det2(d1, v1.X - v2.X, d2, v3.X - v4.X) / a;
			float y = Det2(d1, v1.Y - v2.Y, d2, v3.Y - v4.Y) / a;

			intersec = new FPoint(x, y);

			if (x < FloatMath.Min(v1.X, v2.X) - FloatMath.EPSILON || x > FloatMath.Max(v1.X, v2.X) + FloatMath.EPSILON) return false;
			if (y < FloatMath.Min(v1.Y, v2.Y) - FloatMath.EPSILON || y > FloatMath.Max(v1.Y, v2.Y) + FloatMath.EPSILON) return false;
			if (x < FloatMath.Min(v3.X, v4.X) - FloatMath.EPSILON || x > FloatMath.Max(v3.X, v4.X) + FloatMath.EPSILON) return false;
			if (y < FloatMath.Min(v3.Y, v4.Y) - FloatMath.EPSILON || y > FloatMath.Max(v3.Y, v4.Y) + FloatMath.EPSILON) return false;

			return true;
		}

		public static bool LineIntersectionExt(Vector2 v1s, Vector2 v1e, Vector2 v2s, Vector2 v2e, float epsilon, out FPoint intersec, out float u1, out float u2)
		{
			float a = Det2(v1s.X - v1e.X, v1s.Y - v1e.Y, v2s.X - v2e.X, v2s.Y - v2e.Y);
			if (FloatMath.Abs(a) < FloatMath.EPSILON) { intersec = FPoint.Zero; u1 = u2 = Single.NaN; return false; } // Lines are parallel

			float d1 = Det2(v1s.X, v1s.Y, v1e.X, v1e.Y);
			float d2 = Det2(v2s.X, v2s.Y, v2e.X, v2e.Y);
			float x = Det2(d1, v1s.X - v1e.X, d2, v2s.X - v2e.X) / a;
			float y = Det2(d1, v1s.Y - v1e.Y, d2, v2s.Y - v2e.Y) / a;

			intersec = new FPoint(x, y);

			var v1len = (v1e - v1s).Length();
			var v2len = (v2e - v2s).Length();
			
			u1 = ((x - v1s.X) * (v1e.X - v1s.X) + (y - v1s.Y) * (v1e.Y - v1s.Y)) / ((v1e.X - v1s.X) * (v1e.X - v1s.X) + (v1e.Y - v1s.Y) * (v1e.Y - v1s.Y));
			u2 = ((x - v2s.X) * (v2e.X - v2s.X) + (y - v2s.Y) * (v2e.Y - v2s.Y)) / ((v2e.X - v2s.X) * (v2e.X - v2s.X) + (v2e.Y - v2s.Y) * (v2e.Y - v2s.Y));

			if (u1 < (0 - (epsilon / v1len)) || u1 > (1 + (epsilon / v1len))) return false;
			if (u2 < (0 - (epsilon / v2len)) || u2 > (1 + (epsilon / v2len))) return false;
			
			return true;
		}

		/// <summary>
		/// Returns the determinant of the 2x2 matrix defined as
		/// <list>
		/// <item>| x1 x2 |</item>
		/// <item>| y1 y2 |</item>
		/// </list>
		/// Source: https://stackoverflow.com/a/385828/1761622
		/// </summary>
		public static float Det2(float x1, float x2, float y1, float y2)
		{
			return (x1 * y2 - y1 * x2);
		}

		public static float CrossProduct(Vector2 a, Vector2 b, Vector2 c) => (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);

		public static float LinePointDistance(Vector2 p1, Vector2 p2, Vector2 point)
		{
			return FloatMath.Abs(CrossProduct(p1, p2, point) / (p2 - p1).Length());
		}
	}
}
