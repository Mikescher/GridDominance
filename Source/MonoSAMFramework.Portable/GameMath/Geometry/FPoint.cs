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
		public readonly float X;

		[DataMember]
		public readonly float Y;

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

		public static FPoint operator +(FPoint value1, FPoint value2)
		{
			return new FPoint(value1.X + value2.X, value1.Y + value2.Y);
		}

		public static FPoint operator +(FPoint value1, Vector2 value2)
		{
			return new FPoint(value1.X + value2.X, value1.Y + value2.Y);
		}

		public static Vector2 operator -(FPoint value1, FPoint value2)
		{
			return new Vector2(value1.X - value2.X, value1.Y - value2.Y);
		}
		
		public static FPoint operator *(FPoint value1, FPoint value2)
		{
			return new FPoint(value1.X * value2.X, value1.Y * value2.Y);
		}
		
		public static FPoint operator /(FPoint source, FPoint divisor)
		{
			return new FPoint(source.X / divisor.X, source.Y / divisor.Y);
		}
		
		public static bool operator ==(FPoint a, FPoint b)
		{
			return a.Equals(b);
		}
		
		public static bool operator !=(FPoint a, FPoint b)
		{
			return !a.Equals(b);
		}

		public FPoint RelativeTo(FPoint value2)
		{
			return new FPoint(X - value2.X, Y - value2.Y);
		}

		public FPoint RelativeTo(float px, float py)
		{
			return new FPoint(X - px, Y - py);
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
		
		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}
		
		public override string ToString()
		{
			return $"{{X:{X} Y:{Y}}}";
		}

		public string DebugDisplayString => $"({X}|{Y})";

		public static implicit operator FSize(FPoint point)
		{
			return new FSize(point.X, point.Y);
		}

		public static implicit operator Vector2(FPoint point)
		{
			return new Vector2(point.X, point.Y);
		}

		public float LengthSquared()
		{
			return X * X + Y * Y;
		}
		
		[Pure]
		public FPoint AsRotated(float radians)
		{
			var cos = FloatMath.Cos(radians);
			var sin = FloatMath.Sin(radians);

			return new FPoint(X * cos - Y * sin, X * sin + Y * cos);
		}

		[Pure]
		public Vector2 AsRotatedAround(FPoint anchor, float radians)
		{
			var cos = FloatMath.Cos(radians);
			var sin = FloatMath.Sin(radians);

			return new FPoint(anchor.X + (X - anchor.X) * cos - (Y - anchor.Y) * sin, anchor.Y + (X - anchor.X) * sin + (Y - anchor.Y) * cos);
		}
	}
}
