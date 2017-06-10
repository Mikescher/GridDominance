using System;
using System.Runtime.Serialization;

namespace MonoSAMFramework.Portable.GameMath.Geometry
{
	[DataContract]
	public struct DSize : IEquatable<DSize>
	{
		[DataMember]
		public readonly int Width;

		[DataMember]
		public readonly int Height;

		public static readonly DSize Empty = new DSize(0, 0);

		public static readonly DSize MaxValue = new DSize(int.MaxValue, int.MaxValue);

		public DSize(int width, int height)
			: this()
		{
			Width = width;
			Height = height;
		}

		public bool IsEmpty => Width == 0 || Height == 0;

		public bool IsQuadratic => Width == Height;

		public override int GetHashCode()
		{
			unchecked
			{
				return Width.GetHashCode() + Height.GetHashCode();
			}
		}

		public static bool operator ==(DSize a, DSize b)
		{
			return (a.Width == b.Width) && (a.Height == b.Height);
		}

		public static bool operator !=(DSize a, DSize b)
		{
			return !(a == b);
		}

		public static DSize operator +(DSize value1, DSize value2)
		{
			return new DSize(value1.Width + value2.Width, value1.Height + value2.Height);
		}

		public static DSize operator -(DSize value1, DSize value2)
		{
			return new DSize(value1.Width - value2.Width, value1.Width - value2.Height);
		}

		public bool Equals(DSize other)
		{
			return (Width == other.Width) && (Height == other.Height);
		}
		
		public static DSize operator *(DSize value1, int value2)
		{
			return new DSize(value1.Width * value2, value1.Height * value2);
		}
		
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is DSize && Equals((DSize)obj);
		}

		public override string ToString() => $"[{Width}|{Height}]";
	}
}
