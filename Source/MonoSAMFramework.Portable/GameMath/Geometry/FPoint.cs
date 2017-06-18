using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace MonoSAMFramework.Portable.GameMath.Geometry
{
	[DataContract]
	[DebuggerDisplay("{" + nameof(DebugDisplayString) + ",nq}")]
	public struct FPoint : IEquatable<FPoint>
	{
		public static readonly FPoint Zero = new FPoint(0, 0);

		[DataMember]
		public float X;

		[DataMember]
		public float Y;

		public FPoint(float x, float y)
		{
			X = x;
			Y = y;
		}

		public FPoint(Point p)
		{
			X = p.X;
			Y = p.Y;
		}

		public Vector2 ToVec2D() => new Vector2(X, Y);

		public bool IsOrigin() => Math.Abs(X) < FloatMath.EPSILON && Math.Abs(Y) < FloatMath.EPSILON;
		public bool IsValid => !float.IsInfinity(X) && !float.IsNaN(X) && !float.IsInfinity(Y) && !float.IsNaN(Y);

		public static FPoint operator +(FPoint value1, Vector2 value2)
		{
			return new FPoint(value1.X + value2.X, value1.Y + value2.Y);
		}

		public static FPoint operator -(FPoint value1, Vector2 value2)
		{
			return new FPoint(value1.X - value2.X, value1.Y - value2.Y);
		}

		public static FPoint operator +(FPoint value1, FSize value2)
		{
			return new FPoint(value1.X + value2.Width, value1.Y + value2.Height);
		}

		public static FPoint operator -(FPoint value1, FSize value2)
		{
			return new FPoint(value1.X - value2.Width, value1.Y - value2.Height);
		}

		public static Vector2 operator -(FPoint value1, FPoint value2)
		{
			return new Vector2(value1.X - value2.X, value1.Y - value2.Y);
		}

		public static bool operator ==(FPoint a, FPoint b)
		{
			return a.Equals(b);
		}
		
		public static bool operator !=(FPoint a, FPoint b)
		{
			return !a.Equals(b);
		}

		[Pure]
		public FPoint RelativeTo(FPoint value2)
		{
			return new FPoint(X - value2.X, Y - value2.Y);
		}

		[Pure]
		public FPoint RelativeTo(float px, float py)
		{
			return new FPoint(X - px, Y - py);
		}

		[Pure]
		public FPoint WithOrigin(FPoint origin)
		{
			return new FPoint(X + origin.X, Y + origin.Y);
		}

		public FPoint AsTranslated(float offx, float offy)
		{
			return new FPoint(X + offx, Y + offy);
		}

		public override bool Equals(object obj)
		{
			return obj is FPoint && Equals((FPoint)obj);
		}

		public bool Equals(FPoint other)
		{
			return FloatMath.EpsilonEquals(X, other.X) && FloatMath.EpsilonEquals(Y, other.Y);
		}

		public bool EpsilonEquals(FPoint other, float eps = FloatMath.EPSILON)
		{
			return FloatMath.Abs(X - other.X) <= eps && FloatMath.Abs(Y - other.Y) <= eps;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}
		
		public override string ToString()
		{
			return $"{{X:{X} Y:{Y}}}";
		}

		public string DebugDisplayString => $"({X}|{Y})";

		//public static implicit operator FSize(FPoint point)
		//{
		//	return new FSize(point.X, point.Y);
		//}
		//
		//public static implicit operator Vector2(FPoint point)
		//{
		//	return new Vector2(point.X, point.Y);
		//}

		public float LengthSquared()
		{
			return X * X + Y * Y;
		}
		
		[Pure]
		public FPoint Rotate(float radians)
		{
			var cos = FloatMath.Cos(radians);
			var sin = FloatMath.Sin(radians);

			return new FPoint(X * cos - Y * sin, X * sin + Y * cos);
		}

		[Pure]
		public FPoint RotateAround(FPoint anchor, float radians)
		{
			var cos = FloatMath.Cos(radians);
			var sin = FloatMath.Sin(radians);

			return new FPoint(anchor.X + (X - anchor.X) * cos - (Y - anchor.Y) * sin, anchor.Y + (X - anchor.X) * sin + (Y - anchor.Y) * cos);
		}

		/// <summary>
		///https://stackoverflow.com/a/6177788/1761622
		/// </summary>
		[Pure]
		public float ProjectOntoLine(FPoint lineStart, FPoint lineEnd)
		{

			float x1 = lineStart.X;
			float y1 = lineStart.Y;

			float x2 = lineEnd.X;
			float y2 = lineEnd.Y;

			float u = ((X - x1) * (x2 - x1) + (Y - y1) * (y2 - y1)) / ((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));

			return u;
		}

		/// <summary>
		///https://stackoverflow.com/a/6177788/1761622
		/// </summary>
		[Pure]
		public FPoint ProjectPointOntoLine(FPoint lineStart, FPoint lineEnd)
		{
			float u = ProjectOntoLine(lineStart, lineEnd);

			return lineStart + (lineEnd - lineStart) * u;
		}

		/// <summary>
		///https://stackoverflow.com/a/6177788/1761622
		/// </summary>
		[Pure]
		public float ProjectOntoLine(FPoint lineStart, Vector2 lineVec)
		{

			float x1 = lineStart.X;
			float y1 = lineStart.Y;

			float x2 = lineStart.X + lineVec.X;
			float y2 = lineStart.Y + lineVec.Y;

			float u = ((X - x1) * (x2 - x1) + (Y - y1) * (y2 - y1)) / ((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));

			return u;
		}

		/// <summary>
		///https://stackoverflow.com/a/6177788/1761622
		/// </summary>
		[Pure]
		public FPoint ProjectPointOntoLine(FPoint lineStart, Vector2 lineVec)
		{
			return lineStart + lineVec * ProjectOntoLine(lineStart, lineVec);
		}
		
		[Pure]
		public float DistanceTo(FPoint value2)
		{
			float v1 = X - value2.X, v2 = Y - value2.Y;
			return (float)Math.Sqrt((v1 * v1) + (v2 * v2));
		}

		public static float Distance(FPoint value1, FPoint value2)
		{
			float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
			return (float)Math.Sqrt((v1 * v1) + (v2 * v2));
		}

		public static FPoint MiddlePoint(FPoint a, FPoint b)
		{
			return new FPoint((a.X + b.X) / 2f, (a.Y + b.Y) / 2f);
		}

		[Pure]
		public FPoint Negate()
		{
			return new FPoint(-X, -Y);
		}

		public static FPoint Lerp(FPoint value1, FPoint value2, float amount)
		{
			return new FPoint(MathHelper.Lerp(value1.X, value2.X, amount), MathHelper.Lerp(value1.Y, value2.Y, amount));
		}

		// @see Vector2Extension.ToAngle()
		public float ToAngle()
		{
			return (FloatMath.Atan2(Y, X) + FloatMath.RAD_POS_360) % FloatMath.RAD_POS_360;
		}

		// @see Vector2Extension.ToAngle()
		public float ToAngle(FPoint origin)
		{
			return (FloatMath.Atan2(Y - origin.Y, X - origin.X) + FloatMath.RAD_POS_360) % FloatMath.RAD_POS_360;
		}

		[Pure]
		public FPoint MirrorAtNormal(FPoint linePoint, Vector2 normal)
		{
			//https://stackoverflow.com/a/6177788/1761622

			float x1 = linePoint.X - normal.Y;
			float y1 = linePoint.Y + normal.X;

			float x2 = linePoint.X + normal.Y;
			float y2 = linePoint.Y - normal.X;

			float x3 = X;
			float y3 = Y;

			float u = ((x3 - x1) * (x2 - x1) + (y3 - y1) * (y2 - y1)) / ((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));

			float xu = x1 + u * (x2 - x1);
			float yu = y1 + u * (y2 - y1);

			float dx = xu - X;
			float dy = yu - Y;

			float rx = X + 2 * dx;
			float ry = Y + 2 * dy;

			return new FPoint(rx, ry);
		}

		[Pure]
		public FPoint MirrorAt(FPoint mirrorPoint)
		{
			return new FPoint(2 * mirrorPoint.X - X, 2 * mirrorPoint.Y - Y);
		}
	}
}
