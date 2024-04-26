using System.Linq;
using MonoSAMFramework.Portable.LogProtocol;

namespace MonoSAMFramework.Portable.GameMath.Geometry
{
	public static class ShapeMath
	{
		public static bool Overlaps(IFShape s1, IFShape s2)
		{
			if (s1 is FCircle s1_circ)
			{
				if (s2 is FCircle s2_circ)
				{
					return OverlapsSpecific(s1_circ, s2_circ);
				}
				else if (s2 is FRectangle s2_rect)
				{
					return OverlapsSpecific(s1_circ, s2_rect);
				}
				else if (s2 is FRotatedRectangle s2_rotrect)
				{
					return OverlapsSpecific(s1_circ, s2_rotrect);
				}
			}
			else if (s1 is FRectangle s1_rect)
			{
				if (s2 is FCircle s2_circ)
				{
					return OverlapsSpecific(s2_circ, s1_rect);
				}
				else if (s2 is FRectangle s2_rect)
				{
					return OverlapsSpecific(s1_rect, s2_rect);
				}
				else if (s2 is FRotatedRectangle s2_rotrect)
				{
					return OverlapsSpecific(s1_rect, s2_rotrect);
				}
			}
			else if (s1 is FRotatedRectangle s1_rotrect)
			{
				if (s2 is FCircle s2_circ)
				{
					return OverlapsSpecific(s2_circ, s1_rotrect);
				}
				else if (s2 is FRectangle s2_rect)
				{
					return OverlapsSpecific(s2_rect, s1_rotrect);
				}
				else if (s2 is FRotatedRectangle s2_rotrect)
				{
					return OverlapsSpecific(s2_rotrect, s1_rotrect);
				}
			}

			SAMLog.Error("GMGSM::EnumSwitch_Overlaps", $"s1.Type = {s1?.GetType()} | s2.Type = {s2?.GetType()}");

			return false;

		}

		private static bool OverlapsSpecific(FRotatedRectangle g1, FRotatedRectangle g2)
		{
			if (g1.TryConvertToRect(out var g1c) && g2.TryConvertToRect(out var g2c)) return OverlapsSpecific(g1c, g2c);

			return IsPolygonsIntersecting(g1.EdgePoints.ToArray(), g2.EdgePoints.ToArray());
		}

		private static bool OverlapsSpecific(FRectangle g1, FRotatedRectangle g2)
		{
			if (g2.TryConvertToRect(out var g2c)) return OverlapsSpecific(g1, g2c);

			return IsPolygonsIntersecting(g1.EdgePoints.ToArray(), g2.EdgePoints.ToArray());
		}

		private static bool OverlapsSpecific(FCircle g1, FRotatedRectangle g2)
		{
			if (g2.TryConvertToRect(out var g2c)) return OverlapsSpecific(g1, g2c);

			var c = g1.Center.RotateAround(g2.Center, -g2.Rotation);

			return OverlapsSpecific(new FCircle(c, g1.Radius), g2.WithNoRotation());
		}

		private static bool OverlapsSpecific(FRectangle g1, FRectangle g2)
		{
			return g2.Left < g1.Right && g1.Left < g2.Right && g2.Top < g1.Bottom && g1.Top < g2.Bottom;
		}

		private static bool OverlapsSpecific(FCircle a, FRectangle b)
		{
			if (a.Center.Y > b.Top && a.Center.Y < b.Bottom)
			{
				return a.Center.X > (b.Left - a.Radius) && a.Center.X < (b.Right + a.Radius);
			}
			else if (a.Center.X > b.Left && a.Center.X < b.Right)
			{
				return a.Center.Y > (b.Top - a.Radius) && a.Center.Y < (b.Bottom + a.Radius);
			}
			else
			{
				return FloatMath.Pow2(a.Center.X - b.Left)  + FloatMath.Pow2(a.Center.Y - b.Top)    < FloatMath.Pow2(a.Radius) ||
				       FloatMath.Pow2(a.Center.X - b.Right) + FloatMath.Pow2(a.Center.Y - b.Top)    < FloatMath.Pow2(a.Radius) ||
				       FloatMath.Pow2(a.Center.X - b.Right) + FloatMath.Pow2(a.Center.Y - b.Bottom) < FloatMath.Pow2(a.Radius) ||
				       FloatMath.Pow2(a.Center.X - b.Left)  + FloatMath.Pow2(a.Center.Y - b.Bottom) < FloatMath.Pow2(a.Radius);
			}
		}

		private static bool OverlapsSpecific(FCircle g1, FCircle g2)
		{
			return (g1.Center - g2.Center).LengthSquared() < (g1.Radius + g2.Radius) * (g1.Radius + g2.Radius);
		}

		// https://stackoverflow.com/a/10965077/1761622
		public static bool IsPolygonsIntersecting(FPoint[] a, FPoint[] b)
		{
			foreach (var polygon in new[] { a, b })
			{
				for (int i1 = 0; i1 < polygon.Length; i1++)
				{
					int i2 = (i1 + 1) % polygon.Length;
					var p1 = polygon[i1];
					var p2 = polygon[i2];

					var normal = new FPoint(p2.Y - p1.Y, p1.X - p2.X);

					double? minA = null, maxA = null;
					foreach (var p in a)
					{
						var projected = normal.X * p.X + normal.Y * p.Y;
						if (minA == null || projected < minA)
							minA = projected;
						if (maxA == null || projected > maxA)
							maxA = projected;
					}

					double? minB = null, maxB = null;
					foreach (var p in b)
					{
						var projected = normal.X * p.X + normal.Y * p.Y;
						if (minB == null || projected < minB)
							minB = projected;
						if (maxB == null || projected > maxB)
							maxB = projected;
					}

					if (maxA < minB || maxB < minA)
						return false;
				}
			}
			return true;
		}
	}
}
