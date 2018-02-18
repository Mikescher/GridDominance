using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace MonoSAMFramework.Portable.GameMath.Geometry
{
	[DataContract]
	[DebuggerDisplay("{" + nameof(DebugDisplayString) + ",nq}")]
	public struct FRotatedRectangle : IEquatable<FRotatedRectangle>, IFShape
	{
		public static readonly FRotatedRectangle Empty = new FRotatedRectangle(0, 0, 0, 0, FloatMath.RAD_000);

		[DataMember]
		public readonly float CenterX;
		[DataMember]
		public readonly float CenterY;
		[DataMember]
		public readonly float Width;
		[DataMember]
		public readonly float Height;
		[DataMember]
		public readonly float Rotation; //rads

		public FRotatedRectangle(float cx, float cy, float width, float height, float rot)
		{
			CenterX = cx;
			CenterY = cy;
			Width = width;
			Height = height;
			Rotation = rot;

			_cacheMostLeft   = null;
			_cacheMostRight  = null;
			_cacheMostTop    = null;
			_cacheMostBottom = null;
		}

		public FRotatedRectangle(FPoint c, float width, float height, float rot)
		{
			CenterX = c.X;
			CenterY = c.Y;
			Width = width;
			Height = height;
			Rotation = rot;

			_cacheMostLeft = null;
			_cacheMostRight = null;
			_cacheMostTop = null;
			_cacheMostBottom = null;
		}

		public FRotatedRectangle(FPoint c, FSize s, float rot)
		{
			CenterX = c.X;
			CenterY = c.Y;
			Width = s.Width;
			Height = s.Height;
			Rotation = rot;

			_cacheMostLeft = null;
			_cacheMostRight = null;
			_cacheMostTop = null;
			_cacheMostBottom = null;
		}

		[Pure]
		public static bool operator ==(FRotatedRectangle a, FRotatedRectangle b)
		{
			return
				FloatMath.EpsilonEquals(a.CenterX, b.CenterX) &&
				FloatMath.EpsilonEquals(a.CenterY, b.CenterY) &&
				FloatMath.EpsilonEquals(a.Width, b.Width) &&
				FloatMath.EpsilonEquals(a.Height, b.Height) &&
				FloatMath.EpsilonEquals(a.Rotation, b.Rotation);
		}

		[Pure]
		public static bool operator !=(FRotatedRectangle a, FRotatedRectangle b)
		{
			return !(a == b);
		}

		[Pure]
		public static FRotatedRectangle CreateByCenter(FPoint pos, float width, float height, float rotation)
		{
			return new FRotatedRectangle(pos, width, height, rotation);
		}

		[Pure]
		public static FRotatedRectangle CreateByCenter(FPoint pos, FSize size, float rotation)
		{
			return new FRotatedRectangle(pos, size, rotation);
		}

		public bool Contains(float x, float y)
		{
			return Contains(new FPoint(x, y));
		}

		[Pure]
		public bool Contains(FPoint p)
		{
			var rp = p.RotateAround(Center, -Rotation);
			
			return
				rp.X >= CenterX - Width/2f  &&
				rp.Y >= CenterY - Height/2f &&
				rp.X <  CenterX + Width/2f  &&
				rp.Y <  CenterY + Height/2f ;
		}

		public bool Contains(Vector2 v)
		{
			var rp = v.RotateAround(Center, -Rotation);

			return
				rp.X >= CenterX - Width / 2f &&
				rp.Y >= CenterY - Height / 2f &&
				rp.X < CenterX + Width / 2f &&
				rp.Y < CenterY + Height / 2f;
		}

		public IEnumerable<FPoint> EdgePoints => new[] {TopRight, TopLeft, BottomLeft, BottomRight};

		[Pure]
		public FRotatedRectangle AsTranslated(float offsetX, float offsetY)
		{
			if (FloatMath.IsZero(offsetX) && FloatMath.IsZero(offsetY)) return this;

			return new FRotatedRectangle(CenterX + offsetX, CenterY + offsetY, Width, Height, Rotation);
		}

		IFShape IFShape.AsTranslated(float offsetX, float offsetY) => AsTranslated(offsetX, offsetY);

		[Pure]
		public FRotatedRectangle AsTranslated(Vector2 offset)
		{
			if (offset.IsZero()) return this;

			return new FRotatedRectangle(CenterX + offset.X, CenterY + offset.Y, Width, Height, Rotation);
		}

		IFShape IFShape.AsTranslated(Vector2 offset) => AsTranslated(offset);

		[Pure]
		public bool Contains(FPoint p, float delta)
		{
			var rp = p.RotateAround(Center, -Rotation);

			return
				rp.X >= CenterX - Width/2f  - delta &&
				rp.Y >= CenterY - Height/2f - delta &&
				rp.X < (CenterX + Width/2f  + delta + delta) &&
				rp.Y < (CenterY + Height/2f + delta + delta);
		}

		[Pure]
		public override bool Equals(object obj)
		{
			return obj is FRotatedRectangle && this == (FRotatedRectangle)obj;
		}

		[Pure]
		public bool Equals(FRotatedRectangle other)
		{
			return this == other;
		}

		[Pure]
		public override int GetHashCode()
		{
			return Rotation.GetHashCode() ^ 7829 * CenterX.GetHashCode() ^ 7841 * CenterY.GetHashCode() ^ 7853 * Width.GetHashCode() ^ 7867 * Height.GetHashCode();
		}
		[Pure]
		public override string ToString()
		{
			return $"{{X:{CenterX} Y:{CenterY} Width:{Width} Height:{Height} Rotation:{FloatMath.ToDegree(Rotation)}°}}";
		}

		[Pure]
		public FRectangle WithNoRotation()
		{
			return new FRectangle(CenterX - Width / 2f, CenterY - Height / 2f, Width, Height);
		}

		[Pure]
		public FRectangle GetBoundingRectangle()
		{
			return new FRectangle(CenterX, CenterY, MostRight-MostLeft, MostBottom-MostTop);
		}

		public bool TryConvertToRect(out FRectangle rect)
		{
			if (FloatMath.EpsilonEquals(Rotation, FloatMath.RAD_POS_000))
			{
				rect = new FRectangle(CenterX - Width / 2f, CenterY - Height / 2f, Width, Height);
				return true;
			}

			if (FloatMath.EpsilonEquals(Rotation, FloatMath.RAD_POS_090))
			{
				rect = new FRectangle(CenterX - Height / 2f, CenterY - Width / 2f, Height, Width);
				return true;
			}

			if (FloatMath.EpsilonEquals(Rotation, FloatMath.RAD_POS_180))
			{
				rect = new FRectangle(CenterX - Width / 2f, CenterY - Height / 2f, Width, Height);
				return true;
			}

			if (FloatMath.EpsilonEquals(Rotation, FloatMath.RAD_POS_270))
			{
				rect = new FRectangle(CenterX - Height / 2f, CenterY - Width / 2f, Height, Width);
				return true;
			}

			rect = FRectangle.Empty;
			return false;
		}

		[Pure]
		public bool Overlaps(IFShape other)
		{
			return ShapeMath.Overlaps(this, other);
		}

		internal string DebugDisplayString => $"({CenterX}|{CenterY}):({Width}|{Height}):({FloatMath.ToDegree(Rotation)}°)";

		private float? _cacheMostLeft;
		private float? _cacheMostRight;
		private float? _cacheMostTop;
		private float? _cacheMostBottom;

		public float MostLeft   { get { if (_cacheMostLeft == null) CalcOuterCoords(); return _cacheMostLeft ?? 0; } }
		public float MostRight  { get { if (_cacheMostLeft == null) CalcOuterCoords(); return _cacheMostRight ?? 0; } }
		public float MostTop    { get { if (_cacheMostLeft == null) CalcOuterCoords(); return _cacheMostTop ?? 0; } }
		public float MostBottom { get { if (_cacheMostLeft == null) CalcOuterCoords(); return _cacheMostBottom ?? 0; } }

		public FSize OuterSize { get { if (_cacheMostLeft == null) CalcOuterCoords(); return new FSize(FloatMath.Abs((_cacheMostLeft ?? 0) - (_cacheMostRight ?? 0)), FloatMath.Abs((_cacheMostTop ?? 0) - (_cacheMostBottom ?? 0))); } }

		private void CalcOuterCoords()
		{
			var p1 = new Vector2(+Width/2f, -Height/2f).Rotate(Rotation);
			var p2 = new Vector2(-Width/2f, -Height/2f).Rotate(Rotation);

			_cacheMostLeft   = FloatMath.Min(CenterX - p1.X, CenterX + p1.X, CenterX - p2.X, CenterX + p2.X);
			_cacheMostRight  = FloatMath.Max(CenterX - p1.X, CenterX + p1.X, CenterX - p2.X, CenterX + p2.X);
			_cacheMostTop    = FloatMath.Min(CenterY - p1.Y, CenterY + p1.Y, CenterY - p2.Y, CenterY + p2.Y);
			_cacheMostBottom = FloatMath.Max(CenterY - p1.Y, CenterY + p1.Y, CenterY - p2.Y, CenterY + p2.Y);
		}

		public FPoint TopLeft     => new FPoint(CenterX - Width / 2, CenterY - Height / 2).RotateAround(Center, Rotation);
		public FPoint TopRight    => new FPoint(CenterX + Width / 2, CenterY - Height / 2).RotateAround(Center, Rotation);
		public FPoint BottomLeft  => new FPoint(CenterX - Width / 2, CenterY + Height / 2).RotateAround(Center, Rotation);
		public FPoint BottomRight => new FPoint(CenterX + Width / 2, CenterY + Height / 2).RotateAround(Center, Rotation);

		public bool IsEmpty => Math.Abs(Width) < FloatMath.EPSILON || Math.Abs(Height) < FloatMath.EPSILON;

		public FSize Size => new FSize(Width, Height);
		public float Area => Width * Height;
		public FPoint Center => new FPoint(CenterX, CenterY);
		public Vector2 VecCenter => new Vector2(CenterX, CenterY);
	}
}
