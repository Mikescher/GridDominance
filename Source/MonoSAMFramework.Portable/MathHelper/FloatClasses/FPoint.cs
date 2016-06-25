using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace MonoSAMFramework.Portable.MathHelper.FloatClasses
{
	[DataContract]
	[DebuggerDisplay("{DebugDisplayString,nq}")]
	public struct FPoint : IEquatable<FPoint>
	{
		public const float EPSILON = 1E-10f;

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

		public override bool Equals(object obj)
		{
			return obj is FPoint && Equals((FPoint)obj);
		}
		
		public bool Equals(FPoint other)
		{
			return Math.Abs(X - other.X) < EPSILON && Math.Abs(Y - other.Y) < EPSILON;
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
	}
}
