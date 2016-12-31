using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace MonoSAMFramework.Portable.GameMath.Geometry
{
	[DataContract]
	[DebuggerDisplay("{DebugDisplayString,nq}")]
	public struct FRectangle : IEquatable<FRectangle>, IFShape
	{
		public static readonly FRectangle Empty = new FRectangle(0, 0, 0, 0);

		[DataMember]
		public readonly float X;
		[DataMember]
		public readonly float Y;
		[DataMember]
		public readonly float Width;
		[DataMember]
		public readonly float Height;

		public FRectangle(float x, float y, float width, float height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public FRectangle(FPoint location, FPoint size)
		{
			X = location.X;
			Y = location.Y;
			Width = size.X;
			Height = size.Y;
		}

		public FRectangle(FPoint location, FSize size)
		{
			X = location.X;
			Y = location.Y;
			Width = size.Width;
			Height = size.Height;
		}

		public FRectangle(Rectangle r)
		{
			X = r.X;
			Y = r.Y;
			Width = r.Width;
			Height = r.Height;
		}

		[Pure]
		public static FRectangle CreateByCenter(Vector2 origin, float cx, float cy, float width, float height)
		{
			return new FRectangle(origin.X + cx - width/2, origin.Y + cy - height/2, width, height);
		}

		[Pure]
		public static bool operator ==(FRectangle a, FRectangle b)
		{
			return
				FloatMath.EpsilonEquals(a.X, b.X) &&
				FloatMath.EpsilonEquals(a.Y, b.Y) &&
				FloatMath.EpsilonEquals(a.Width, b.Width) &&
				FloatMath.EpsilonEquals(a.Height, b.Height);
		}

		[Pure]
		public static bool operator !=(FRectangle a, FRectangle b)
		{
			return !(a == b);
		}

		[Pure]
		public static FRectangle operator +(FRectangle value1, FPoint value2)
		{
			return new FRectangle(value1.X + value2.X, value1.Y + value2.Y, value1.Width, value1.Height);
		}

		[Pure]
		public static FRectangle operator +(FRectangle value1, Vector2 value2)
		{
			return new FRectangle(value1.X + value2.X, value1.Y + value2.Y, value1.Width, value1.Height);
		}

		[Pure]
		public static FRectangle operator -(FRectangle value1, Vector2 value2)
		{
			return new FRectangle(value1.X - value2.X, value1.Y - value2.Y, value1.Width, value1.Height);
		}

		[Pure]
		public static FRectangle operator *(FRectangle value1, float value2)
		{
			return new FRectangle(value1.X * value2, value1.Y * value2, value1.Width * value2, value1.Height * value2);
		}

		// http://stackoverflow.com/a/306332/1761622
		[Pure]
		internal bool Contains(Vector2 center, FSize size)
		{
			return
				X <= (center.X + size.Width  / 2) &&
				Y <= (center.Y + size.Height / 2) &&
				(X + Width)  >= (center.X - size.Width / 2) &&
				(Y + Height) >= (center.Y - size.Height / 2);
		}

		[Pure]
		public bool Contains(float x, float y)
		{
			return 
				x >= X && 
				y >= Y && 
				x < (X + Width) && 
				y < (Y + Height);
		}

		[Pure]
		public bool Contains(FPoint p)
		{
			return 
				p.X >= X && 
				p.Y >= Y && 
				p.X < (X + Width) && 
				p.Y < (Y + Height);
		}

		// http://stackoverflow.com/a/306332/1761622
		[Pure]
		public bool Contains(Vector2 v)
		{
			return 
				v.X >= X && 
				v.Y >= Y && 
				v.X < (X + Width) && 
				v.Y < (Y + Height);
		}

		[Pure]
		public bool Contains(Rectangle value)
		{
			return 
				Left   <= value.Right  && 
				Top    <= value.Bottom && 
				Right  >= value.Left   && 
				Bottom >= value.Top;
		}

		[Pure]
		public override bool Equals(object obj)
		{
			return obj is FRectangle && this == (FRectangle)obj;
		}

		[Pure]
		public bool Equals(FRectangle other)
		{
			return this == other;
		}

		[Pure]
		public override int GetHashCode()
		{
			return X.GetHashCode() ^ 7841 * Y.GetHashCode() ^ 7853 * Width.GetHashCode() ^ 7867 * Height.GetHashCode();
		}

		[Pure]
		public bool Intersects(FRectangle value)
		{
			return value.Left < Right && Left < value.Right && value.Top < Bottom && Top < value.Bottom;
		}

		[Pure]
		public override string ToString()
		{
			return $"{{X:{X} Y:{Y} Width:{Width} Height:{Height}}}";
		}

		internal string DebugDisplayString => $"({X}|{Y}):({Width}|{Height})";

		[Pure]
		public FRectangle Union(FRectangle value2)
		{
			float num = FloatMath.Min(X, value2.X);
			float num2 = FloatMath.Min(Y, value2.Y);
			return new FRectangle(num, num2, System.Math.Max(Right, value2.Right) - num, System.Math.Max(Bottom, value2.Bottom) - num2);
		}

		[Pure]
		public FRectangle AsDeflated(int horizontalAmount, int verticalAmount)
		{
			if (FloatMath.IsZero(horizontalAmount) && FloatMath.IsZero(verticalAmount)) return this;

			return new FRectangle(
				X + horizontalAmount,
				Y + verticalAmount,
				Width - horizontalAmount * 2,
				Height - verticalAmount * 2);
		}

		[Pure]
		public FRectangle AsDeflated(float horizontalAmount, float verticalAmount)
		{
			if (FloatMath.IsZero(horizontalAmount) && FloatMath.IsZero(verticalAmount)) return this;

			return new FRectangle(
				X + horizontalAmount,
				Y + verticalAmount,
				Width - horizontalAmount * 2,
				Height - verticalAmount * 2);
		}

		[Pure]
		public FRectangle AsDeflated(float vNorth, float vEast, float vSouth, float vWest)
		{
			return new FRectangle(
				X + vWest,
				Y + vNorth,
				Width - (vEast + vWest),
				Height - (vNorth + vSouth));
		}

		[Pure]
		public FRectangle AsInflated(int horizontalAmount, int verticalAmount)
		{
			if (FloatMath.IsZero(horizontalAmount) && FloatMath.IsZero(verticalAmount)) return this;

			return new FRectangle(
				X - horizontalAmount,
				Y - verticalAmount,
				Width + horizontalAmount * 2,
				Height + verticalAmount * 2);
		}

		[Pure]
		public FRectangle AsInflated(float horizontalAmount, float verticalAmount)
		{
			if (FloatMath.IsZero(horizontalAmount) && FloatMath.IsZero(verticalAmount)) return this;

			return new FRectangle(
				X - horizontalAmount,
				Y - verticalAmount,
				Width + horizontalAmount * 2,
				Height + verticalAmount * 2);
		}

		[Pure]
		public FRectangle ToSquare(float squaresize, FlatAlign9 align)
		{
			switch (align)
			{
				case FlatAlign9.TOP:
					return new FRectangle(X + (Width - squaresize) / 2f, Y,                              squaresize, squaresize);
				case FlatAlign9.TOPRIGHT:
					return new FRectangle(X + (Width - squaresize),      Y,                              squaresize, squaresize);
				case FlatAlign9.RIGHT:
					return new FRectangle(X + (Width - squaresize),      Y + (Height - squaresize) / 2f, squaresize, squaresize);
				case FlatAlign9.BOTTOMRIGHT:
					return new FRectangle(X + (Width - squaresize),      Y + (Height - squaresize),      squaresize, squaresize);
				case FlatAlign9.BOTTOM:
					return new FRectangle(X + (Width - squaresize) / 2f, Y + (Height - squaresize),      squaresize, squaresize);
				case FlatAlign9.BOTTOMLEFT:
					return new FRectangle(X,                             Y + (Height - squaresize),      squaresize, squaresize);
				case FlatAlign9.LEFT:
					return new FRectangle(X,                             Y + (Height - squaresize) / 2f, squaresize, squaresize);
				case FlatAlign9.TOPLEFT:
					return new FRectangle(X,                             Y,                              squaresize, squaresize);
				case FlatAlign9.CENTER:
					return new FRectangle(X + (Width - squaresize) / 2f, Y + (Height - squaresize) / 2f, squaresize, squaresize);
				default:
					throw new ArgumentOutOfRangeException(nameof(align), align, null);
			}
		}

		[Pure]
		public FRectangle AsTranslated(float offsetX, float offsetY)
		{
			if (FloatMath.IsZero(offsetX) && FloatMath.IsZero(offsetY)) return this;

			return new FRectangle(X + offsetX, Y + offsetY, Width, Height);
		}

		IFShape IFShape.AsTranslated(float offsetX, float offsetY) => AsTranslated(offsetX, offsetY);

		[Pure]
		public FRectangle AsTranslated(Vector2 offset)
		{
			if (offset.IsZero()) return this;

			return new FRectangle(X + offset.X, Y + offset.Y, Width, Height);
		}

		IFShape IFShape.AsTranslated(Vector2 offset) => AsTranslated(offset);

		[Pure]
		public FRectangle AsResized(FSize size)
		{
			return new FRectangle(X + Width / 2 - size.Width / 2, Y + Height / 2 - size.Height / 2, size.Width, size.Height);
		}

		[Pure]
		public FRectangle AsResized(float newWidth, float newHeight)
		{
			return new FRectangle(X + Width / 2 - newWidth / 2, Y + Height / 2 - newHeight / 2, newWidth, newHeight);
		}

		[Pure]
		public FRectangle AsScaled(float scale)
		{
			float w = Width * scale;
			float h = Height * scale;

			return new FRectangle(X + Width / 2 - w / 2, Y + Height / 2 - h / 2, w, h);
		}

		[Pure]
		public FRectangle AsCenteredTo(FPoint center)
		{
			return new FRectangle(center.X - Width / 2, center.Y - Height / 2, Width, Height);
		}

		public float Left => X;
		public float Right => X + Width;
		public float Top => Y;
		public float Bottom => Y + Height;

		public bool IsEmpty => Math.Abs(Width) < FloatMath.EPSILON && Math.Abs(Height) < FloatMath.EPSILON && Math.Abs(X) < FloatMath.EPSILON && Math.Abs(Y) < FloatMath.EPSILON;

		public FPoint TopLeft => new FPoint(Left, Top);
		public FPoint TopRight => new FPoint(Right, Top);
		public FPoint BottomLeft => new FPoint(Left, Bottom);
		public FPoint BottomRight => new FPoint(Right, Bottom);

		public Vector2 VectorTopLeft => new Vector2(Left, Top);
		public Vector2 VectorTopRight => new Vector2(Right, Top);
		public Vector2 VectorBottomLeft => new Vector2(Left, Bottom);
		public Vector2 VectorBottomRight => new Vector2(Right, Bottom);

		public FPoint Location => new FPoint(X, Y);
		public FSize Size => new FSize(Width, Height);
		public FPoint Center => new FPoint(X + Width / 2, Y + Height / 2);
		public Vector2 VecCenter => new Vector2(X + Width / 2, Y + Height / 2);

		[Pure]
		public Rectangle Truncate()
		{
			return new Rectangle((int) X, (int) Y, (int) Width, (int) Height);
		}

		[Pure]
		public Rectangle Round()
		{
			return new Rectangle(FloatMath.Round(X), FloatMath.Round(Y), FloatMath.Round(Width), FloatMath.Round(Height));
		}

		[Pure]
		public FRectangle LimitSingleCoordNorth(float coordY)
		{
			if (Top >= coordY && Bottom >= coordY) return this;
			if (Top <= coordY && Bottom <= coordY) return Empty;

			return new FRectangle(X, coordY, Width, Bottom - coordY);
		}

		[Pure]
		public FRectangle LimitSingleCoordEast(float coordX)
		{
			if (Left <= coordX && Right <= coordX) return this;
			if (Left >= coordX && Right >= coordX) return Empty;

			return new FRectangle(X, Y, coordX - X, Height);
		}

		[Pure]
		public FRectangle LimitSingleCoordSouth(float coordY)
		{
			if (Top <= coordY && Bottom <= coordY) return this;
			if (Top >= coordY && Bottom >= coordY) return Empty;

			return new FRectangle(X, Y, Width, coordY - Y);
		}

		[Pure]
		public FRectangle LimitSingleCoordWest(float coordX)
		{
			if (Left >= coordX && Right >= coordX) return this;
			if (Left <= coordX && Right <= coordX) return Empty;

			return new FRectangle(coordX, Y, Right - coordX, Height);
		}
	}
}
