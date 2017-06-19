using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.Contracts;
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

		public float Ratio => Width / Height;
		public FPoint Center => new FPoint(Width/2f, Height/2f);

		public override int GetHashCode()
		{
			unchecked
			{
				return Width.GetHashCode() + Height.GetHashCode();
			}
		}

		public Vector2 ToVec2D() => new Vector2(Width, Height);

		public static bool operator ==(FSize a, FSize b)
		{
			return FloatMath.EpsilonEquals(a.Width, b.Width) && FloatMath.EpsilonEquals(a.Height, b.Height);
		}

		public static bool operator !=(FSize a, FSize b)
		{
			return !(a == b);
		}

		public static FSize operator +(FSize value1, FSize value2)
		{
			return new FSize(value1.Width + value2.Width, value1.Height + value2.Height);
		}

		public static FSize operator -(FSize value1, FSize value2)
		{
			return new FSize(value1.Width - value2.Width, value1.Width - value2.Height);
		}

		public bool Equals(FSize other)
		{
			return FloatMath.EpsilonEquals(Width, other.Width) && FloatMath.EpsilonEquals(Height, other.Height);
		}

		//public static implicit operator FPoint(FSize size)
		//{
		//	return new FPoint(size.Width, size.Height);
		//}
		//
		//public static implicit operator Vector2(FSize size)
		//{
		//	return new Vector2(size.Width, size.Height);
		//}

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

		public FSize Underfit(FSize bounds, float padding = 0)
		{
			if (Width / Height < (bounds.Width - 2 * padding) / (Height - 2 * padding))
			{
				var h = bounds.Height - 2 * padding;
				var w = (Width / Height) * h;

				return new FSize(w, h);
			}
			else
			{
				var w = bounds.Width - 2 * padding;
				var h = (Height / Width) * w;

				return new FSize(w, h);
			}
		}

		public static FSize Diff(FPoint a, FPoint b)
		{
			return new FSize(FloatMath.Abs(a.X - b.X), FloatMath.Abs(a.Y - b.Y));
		}

		[Pure]
		public FSize AtLeast(float minwidth, float minheight)
		{
			return new FSize(Math.Max(Width, minwidth), Math.Max(Height, minheight));
		}

		[Pure]
		public DSize RoundToDSize()
		{
			return new DSize(FloatMath.Round(Width), FloatMath.Round(Height));
		}

		[Pure]
		public FSize ScaleToHeight(float h)
		{
			return new FSize(h * Ratio, h);
		}

		[Pure]
		public FSize ScaleToWidth(float w)
		{
			return new FSize(w, w / Ratio);
		}

		public string FormatAsResolution() => $"{(int) Width}x{(int) Height}";
	}
}
