using System;
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

		public static DPoint operator +(DPoint value1, DPoint value2)
		{
			return new DPoint(value1.X + value2.X, value1.Y + value2.Y);
		}

		public static DPoint operator -(DPoint value1, DPoint value2)
		{
			return new DPoint(value1.X - value2.X, value1.Y - value2.Y);
		}

		public static DPoint operator *(DPoint value1, int value2)
		{
			return new DPoint(value1.X * value2, value1.Y * value2);
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
