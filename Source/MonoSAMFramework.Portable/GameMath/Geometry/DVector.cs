using System;
using System.Runtime.Serialization;

namespace MonoSAMFramework.Portable.GameMath.Geometry
{
	[DataContract]
	public struct DVector : IEquatable<DVector>
	{
		[DataMember]
		public int X;

		[DataMember]
		public int Y;

		public static readonly DVector Zero = new DVector(0, 0);

		public static readonly DVector MaxValue = new DVector(int.MaxValue, int.MaxValue);

		public DVector(int x, int y) : this()
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

		public static bool operator ==(DVector a, DVector b)
		{
			return (a.X == b.X) && (a.Y == b.Y);
		}

		public static bool operator !=(DVector a, DVector b)
		{
			return !(a == b);
		}

		public static DVector operator +(DVector value1, DVector value2)
		{
			return new DVector(value1.X + value2.X, value1.Y + value2.Y);
		}
		
		public bool Equals(DVector other)
		{
			return (X == other.X) && (Y == other.Y);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is DVector vector && Equals(vector);
		}

		public override string ToString() => $"<{X},{Y}>";

		public bool IsZero()
		{
			return FloatMath.IsZero(X) && FloatMath.IsZero(Y);
		}
	}
}
