using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace MonoSAMFramework.Portable.MathHelper.FloatClasses
{
	[DataContract]
	[DebuggerDisplay("{DebugDisplayString,nq}")]
	public struct FSize : IEquatable<FSize>
	{
		public const float EPSILON = 1E-10f;

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

		public bool IsEmpty => Math.Abs(Width) < EPSILON && Math.Abs(Height) < EPSILON;

		public bool IsQuadratic => Math.Abs(Width - Height) < EPSILON;

		public override int GetHashCode()
		{
			unchecked
			{
				return Width.GetHashCode() + Height.GetHashCode();
			}
		}

		public static bool operator ==(FSize a, FSize b)
		{
			return Math.Abs(a.Width - b.Width) < EPSILON && Math.Abs(a.Height - b.Height) < EPSILON;
		}

		public static bool operator !=(FSize a, FSize b)
		{
			return !(a == b);
		}

		public bool Equals(FSize other)
		{
			return Math.Abs(Width - other.Width) < EPSILON && Math.Abs(Height - other.Height) < EPSILON;
		}

		public static implicit operator FPoint(FSize size)
		{
			return new FPoint(size.Width, size.Height);
		}

		public static implicit operator Vector2(FSize size)
		{
			return new Vector2(size.Width, size.Height);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is FSize && Equals((FSize)obj);
		}

		public override string ToString()
		{
			return $"Width: {Width}, Height: {Height}";
		}

		internal string DebugDisplayString => $"({Width}|{Height})";
	}
}
