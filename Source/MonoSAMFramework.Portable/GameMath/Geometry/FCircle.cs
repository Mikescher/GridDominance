using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace MonoSAMFramework.Portable.GameMath.Geometry
{
	[DataContract]
	[DebuggerDisplay("{" + nameof(DebugDisplayString) + ",nq}")]
	public struct FCircle : IEquatable<FCircle>, IFShape
	{
		public static readonly FCircle Empty = new FCircle(0, 0, 0);

		private Vector2? _center;
		public Vector2 Center => (_center ?? (_center = new Vector2(X, Y))).Value;

		[DataMember]
		public readonly float X;
		[DataMember]
		public readonly float Y;
		[DataMember]
		public readonly float Radius;

		public FCircle(float x, float y, float radius)
		{
			_center = null;
			X = x;
			Y = y;
			Radius = radius;
		}

		public FCircle(FPoint location, float radius)
		{
			_center = null;
			X = location.X;
			Y = location.Y;
			Radius = radius;
		}

		public FCircle(FCircle r)
		{
			_center = null;
			X = r.X;
			Y = r.Y;
			Radius = r.Radius;
		}

		public FCircle(Vector2 location, float radius)
		{
			_center = null;
			X = location.X;
			Y = location.Y;
			Radius = radius;
		}

		[Pure]
		public static bool operator ==(FCircle a, FCircle b)
		{
			return
				FloatMath.EpsilonEquals(a.X, b.X) &&
				FloatMath.EpsilonEquals(a.Y, b.Y) &&
				FloatMath.EpsilonEquals(a.Radius, b.Radius);
		}

		[Pure]
		public static bool operator !=(FCircle a, FCircle b)
		{
			return !(a == b);
		}

		[Pure]
		public static FCircle operator +(FCircle value1, FPoint value2)
		{
			return new FCircle(value1.X + value2.X, value1.Y + value2.Y, value1.Radius);
		}

		[Pure]
		public static FCircle operator +(FCircle value1, Vector2 value2)
		{
			return new FCircle(value1.X + value2.X, value1.Y + value2.Y, value1.Radius);
		}

		[Pure]
		public bool Contains(float px, float py)
		{
			return (X - px) * (X - px) + (Y - py) * (Y - py) <= Radius * Radius;
		}

		[Pure]
		public bool Contains(FPoint p)
		{
			return (X - p.X) * (X - p.X) + (Y - p.Y) * (Y - p.Y) <= Radius * Radius;
		}

		[Pure]
		public bool Contains(FPoint p, float delta)
		{
			return (X - p.X) * (X - p.X) + (Y - p.Y) * (Y - p.Y) <= (Radius+delta) * (Radius+delta);
		}

		[Pure]
		public bool Contains(Vector2 v)
		{
			return (X - v.X) * (X - v.X) + (Y - v.Y) * (Y - v.Y) <= Radius * Radius;
		}

		[Pure]
		public override bool Equals(object obj)
		{
			return obj is FCircle && this == (FCircle)obj;
		}

		[Pure]
		public bool Equals(FCircle other)
		{
			return this == other;
		}

		[Pure]
		public override int GetHashCode()
		{
			return X.GetHashCode() ^ 7867 * Y.GetHashCode() ^ 7873 * Radius.GetHashCode();
		}

		[Pure]
		public override string ToString()
		{
			return $"{{X:{X} Y:{Y} Radius:{Radius}}}";
		}

		internal string DebugDisplayString => $"({X}|{Y}):({Radius})";

		[Pure]
		public FCircle AsTranslated(float offsetX, float offsetY)
		{
			if (FloatMath.IsZero(offsetX) && FloatMath.IsZero(offsetY)) return this;

			return new FCircle(X + offsetX, Y + offsetY, Radius);
		}

		IFShape IFShape.AsTranslated(float offsetX, float offsetY) => AsTranslated(offsetX, offsetY);

		[Pure]
		public FCircle AsTranslated(Vector2 offset)
		{
			if (offset.IsZero()) return this;

			return new FCircle(X + offset.X, Y + offset.Y, Radius);
		}

		IFShape IFShape.AsTranslated(Vector2 offset) => AsTranslated(offset);
	}
}
