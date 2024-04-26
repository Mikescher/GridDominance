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

		public static bool LineIntersectionExt(FPoint v1s, FPoint v1e, FPoint v2s, FPoint v2e, float epsilon, out FPoint intersec, out float u1, out float u2)
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
		public static float CrossProduct(FPoint a, FPoint b, FPoint c)    => (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);

		public static float LinePointDistance(FPoint p1, FPoint p2, FPoint point)
		{
			return FloatMath.Abs(CrossProduct(p1, p2, point) / (p2 - p1).Length());
		}

		/// <summary>
		/// https://stackoverflow.com/a/11427699/1761622
		/// </summary>
		/// <returns></returns>
		public static float PointSegmentDistanceSquared(FPoint p, FPoint p1, FPoint p2, out float t, out FPoint q)
		{
			Vector2 d = p2 - p1;
			Vector2 dp1 = p - p1;
			var segLenSquared = d.LengthSquared();
			
			if (FloatMath.IsZero(segLenSquared, FloatMath.EPSILON6))
			{
				// segment is a point.
				q = p1;
				t = 0f;
				return dp1.LengthSquared();
			}
			else
			{
				t = ((dp1.X * d.X) + (dp1.Y * d.Y)) / segLenSquared;
				if (t <= FloatMath.EPSILON7)
				{
					// intersects at or to the "left" of first segment vertex (p1x, p1y).  If t is approximately 0.0, then
					// intersection is at p1.  If t is less than that, then there is no intersection (i.e. p is not within
					// the 'bounds' of the segment)
					if (t >= -FloatMath.EPSILON7)
					{
						// intersects at 1st segment vertex
						t = 0f;
					}
					// set our 'intersection' point to p1.
					q = p1;
					// Note: If you wanted the ACTUAL intersection point of where the projected lines would intersect if
					// we were doing PointLineDistanceSquared, then qx would be (p1x + (t * dx)) and qy would be (p1y + (t * dy)).
				}
				else if (t >= (1.0 - FloatMath.EPSILON7))
				{
					// intersects at or to the "right" of second segment vertex (p2x, p2y).  If t is approximately 1.0, then
					// intersection is at p2.  If t is greater than that, then there is no intersection (i.e. p is not within
					// the 'bounds' of the segment)
					if (t <= (1.0 + FloatMath.EPSILON7))
					{
						// intersects at 2nd segment vertex
						t = 1f;
					}
					q = p2;
					// Note: If you wanted the ACTUAL intersection point of where the projected lines would intersect if
					// we were doing PointLineDistanceSquared, then qx would be (p1x + (t * dx)) and qy would be (p1y + (t * dy)).
				}
				else
				{
					// The projection of the point to the point on the segment that is perpendicular succeeded and the point
					// is 'within' the bounds of the segment.  Set the intersection point as that projected point.
					q = new FPoint(((1f - t) * p1.X) + (t * p2.X), ((1f - t) * p1.Y) + (t * p2.Y));
					// for debugging
					//ASSERT(AreValuesEqual(qx, p1x + (t * dx), EPSILON_TINY));
					//ASSERT(AreValuesEqual(qy, p1y + (t * dy), EPSILON_TINY));
				}
				// return the squared distance from p to the intersection point.
				var dpq = p - q;
				return dpq.LengthSquared();
			}
		}

		/// <summary>
		/// https://stackoverflow.com/a/11427699/1761622
		/// </summary>
		/// <returns></returns>
		public static float SegmentSegmentDistanceSquared(FPoint p1, FPoint p2, FPoint p3, FPoint p4, out FPoint q)
		{
			// check to make sure both segments are long enough (i.e. verts are farther apart than minimum allowed vert distance).
			// If 1 or both segments are shorter than this min length, treat them as a single point.
			float segLen12Squared = (p1 - p2).LengthSquared();
			float segLen34Squared = (p3 - p4).LengthSquared();
			float t = 0f;
			float minDist2 = float.MaxValue;
			if (segLen12Squared <= FloatMath.EPSILON6)
			{
				q = p1;
				if (segLen34Squared <= FloatMath.EPSILON6)
				{
					// point to point
					minDist2 = (p1 - p3).LengthSquared();
				}
				else
				{
					// point - seg
					minDist2 = PointSegmentDistanceSquared(p1, p3, p4, out _, out _);
				}
				return minDist2;
			}
			else if (segLen34Squared <= FloatMath.EPSILON6)
			{
				// seg - point
				minDist2 = PointSegmentDistanceSquared(p3, p1, p2, out t, out q);
				return minDist2;
			}

			// if you have a point class and/or methods to do cross products, you can use those here.
			// This is what we're actually doing:
			// Point2D delta43(p4x - p3x, p4y - p3y);    // dir of p3 -> p4
			// Point2D delta12(p1x - p2x, p1y - p2y);    // dir of p2 -> p1
			// double d = delta12.Cross2D(delta43);
			float d = ((p4.Y - p3.Y) * (p1.X - p2.X)) - ((p1.Y - p2.Y) * (p4.X - p3.X));
			bool bParallel = FloatMath.IsZero(d, FloatMath.EPSILON4);

			if (!bParallel)
			{
				// segments are not parallel.  Check for intersection.
				// Point2D delta42(p4x - p2x, p4y - p2y);    // dir of p2 -> p4
				// t = 1.0 - (delta42.Cross2D(delta43) / d);
				t = 1f - ((((p4.Y - p3.Y) * (p4.X - p2.X)) - ((p4.Y - p2.Y) * (p4.X - p3.X))) / d);
				float seg12TEps = FloatMath.Sqrt(FloatMath.EPSILON6 / segLen12Squared);
				if (t >= -seg12TEps && t <= (1.0 + seg12TEps))
				{
					// inside [p1,p2].   Segments may intersect.
					// double s = 1.0 - (delta12.Cross2D(delta42) / d);
					float s = 1f - ((((p4.Y - p2.Y) * (p1.X - p2.X)) - ((p1.Y - p2.Y) * (p4.X - p2.X))) / d);
					float seg34TEps = FloatMath.Sqrt(FloatMath.EPSILON6 / segLen34Squared);
					if (s >= -seg34TEps && s <= (1.0 + seg34TEps))
					{
						// segments intersect!
						minDist2 = 0f;
						q = new FPoint(((1f - t) * p1.X) + (t * p2.X), ((1f - t) * p1.Y) + (t * p2.Y));
						// for debugging
						//double qsx = ((1.0 - s) * p3x) + (s * p4x);
						//double qsy = ((1.0 - s) * p3y) + (s * p4y);
						//ASSERT(AreValuesEqual(qx, qsx, EPSILON_MIN_VERTEX_DISTANCE_SQUARED));
						//ASSERT(AreValuesEqual(qy, qsy, EPSILON_MIN_VERTEX_DISTANCE_SQUARED));
						return minDist2;
					}
				}
			}

			// Segments do not intersect.   Find closest point and return dist.   No other way at this
			// point except to just brute-force check each segment end-point vs opposite segment.  The
			// minimum distance of those 4 tests is the closest point.
			FPoint tmpQ;
			float tmpD2;
			minDist2 = PointSegmentDistanceSquared(p3, p1, p2, out t, out q);
			tmpD2 = PointSegmentDistanceSquared(p4, p1, p2, out t, out tmpQ);
			if (tmpD2 < minDist2)
			{
				q = tmpQ;
				minDist2 = tmpD2;
			}
			tmpD2 = PointSegmentDistanceSquared(p1, p3, p4, out t, out tmpQ);
			if (tmpD2 < minDist2)
			{
				q = p1;
				minDist2 = tmpD2;
			}
			tmpD2 = PointSegmentDistanceSquared(p2, p3, p4, out t, out tmpQ);
			if (tmpD2 < minDist2)
			{
				q = p2;
				minDist2 = tmpD2;
			}

			return minDist2;
		}

		public static bool EpsilonEquals(FPoint a, FPoint b, float eps)
		{
			return FloatMath.IsZero((a.X - b.X), eps) && FloatMath.IsZero((a.Y - b.Y), eps);
		}

		public static bool EpsilonEquals(Vector2 a, Vector2 b, float eps)
		{
			return FloatMath.IsZero((a.X - b.X), eps) && FloatMath.IsZero((a.Y - b.Y), eps);
		}

		public static FPoint PointOnLine(float u, FPoint start, FPoint end)
		{
			return start + u * (end - start);
		}

		public static FPoint PointOnLine(float u, FPoint start, Vector2 vec)
		{
			return start + u * vec;
		}
	}
}
