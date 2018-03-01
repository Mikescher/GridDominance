using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace MonoSAMFramework.Portable.GameMath.Geometry
{
	[DataContract]
	public struct DPoint : IEquatable<DPoint>
	{
		[DataMember]
		public readonly int X;

		[DataMember]
		public readonly int Y;

		public static readonly DPoint Zero = new DPoint(0, 0);

		public static readonly DPoint MaxValue = new DPoint(int.MaxValue, int.MaxValue);

		public DPoint(int x, int y) : this()
		{
			X = x;
			Y = y;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return X.GetHashCode() + Y.GetHashCode();
			}
		}

		public static bool operator ==(DPoint a, DPoint b)
		{
			return (a.X == b.X) && (a.Y == b.Y);
		}

		public static bool operator !=(DPoint a, DPoint b)
		{
			return !(a == b);
		}

		public static DPoint operator +(DPoint value1, DVector value2)
		{
			return new DPoint(value1.X + value2.X, value1.Y + value2.Y);
		}

		public static FPoint operator +(DPoint value1, Vector2 value2)
		{
			return new FPoint(value1.X + value2.X, value1.Y + value2.Y);
		}

		public static DPoint operator -(DPoint value1, DVector value2)
		{
			return new DPoint(value1.X - value2.X, value1.Y - value2.Y);
		}

		public static FPoint operator -(DPoint value1, Vector2 value2)
		{
			return new FPoint(value1.X - value2.X, value1.Y - value2.Y);
		}

		[Pure]
		public FPoint RelativeTo(FPoint value2)
		{
			return new FPoint(X - value2.X, Y - value2.Y);
		}

		[Pure]
		public DPoint RelativeTo(DPoint value2)
		{
			return new DPoint(X - value2.X, Y - value2.Y);
		}

		[Pure]
		public FPoint WithOrigin(FPoint origin)
		{
			return new FPoint(X + origin.X, Y + origin.Y);
		}

		[Pure]
		public DPoint WithOrigin(DPoint origin)
		{
			return new DPoint(X + origin.X, Y + origin.Y);
		}

		public bool Equals(DPoint other)
		{
			return (X == other.X) && (Y == other.Y);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is DPoint && Equals((DPoint)obj);
		}

		public override string ToString() => $"[{X},{Y}]";
	}
}
