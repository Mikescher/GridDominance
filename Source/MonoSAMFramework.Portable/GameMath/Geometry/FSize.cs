using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace MonoSAMFramework.Portable.GameMath.Geometry
{
	[DataContract]
	public struct FSize : IEquatable<FSize>
	{
		[DataMember]
		public readonly float Width;

		[DataMember]
		public readonly float Height;

		public static readonly FSize Empty = new FSize(0, 0);

		public static readonly FSize MaxValue = new FSize(float.MaxValue, float.MaxValue);

		public FSize(float width, float height)
			: this()
		{
			Width = width;
			Height = height;
		}

		public bool IsEmpty => Math.Abs(Width) < FloatMath.EPSILON && Math.Abs(Height) < FloatMath.EPSILON;

		public bool IsQuadratic => FloatMath.EpsilonEquals(Width, Height);

		public override int GetHashCode()
		{
			unchecked
			{
				return Width.GetHashCode() + Height.GetHashCode();
			}
		}

		public static bool operator ==(FSize a, FSize b)
		{
			return FloatMath.EpsilonEquals(a.Width, b.Width) && FloatMath.EpsilonEquals(a.Height, b.Height);
		}

		public static bool operator !=(FSize a, FSize b)
		{
			return !(a == b);
		}

		public bool Equals(FSize other)
		{
			return FloatMath.EpsilonEquals(Width, other.Width) && FloatMath.EpsilonEquals(Height, other.Height);
		}

		public static implicit operator FPoint(FSize size)
		{
			return new FPoint(size.Width, size.Height);
		}

		public static implicit operator Vector2(FSize size)
		{
			return new Vector2(size.Width, size.Height);
		}

		public static FSize operator *(FSize value1, float value2)
		{
			return new FSize(value1.Width * value2, value1.Height * value2);
		}

		public static FSize operator *(FSize value1, int value2)
		{
			return new FSize(value1.Width * value2, value1.Height * value2);
		}

		public static FSize operator /(FSize value1, float value2)
		{
			return new FSize(value1.Width / value2, value1.Height / value2);
		}

		public static FSize operator /(FSize value1, int value2)
		{
			return new FSize(value1.Width / value2, value1.Height / value2);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is FSize && Equals((FSize)obj);
		}

		public override string ToString() => $"[{Width}|{Height}]";
	}
}
