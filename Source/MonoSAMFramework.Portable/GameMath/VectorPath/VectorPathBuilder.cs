using MonoSAMFramework.Portable.GameMath.Geometry;
using System.Collections.Generic;

namespace MonoSAMFramework.Portable.GameMath.VectorPath
{
	public class VectorPathBuilder
	{
		private readonly List<VectorPathSegment> segments = new List<VectorPathSegment>();
		private readonly float scale;

		private FPoint current;

		private VectorPathBuilder(float builderScale)
		{
			current = new FPoint(0, 0);
			scale = builderScale;
		}

		public static VectorPathBuilder Start(float scale = 1f)
		{
			return new VectorPathBuilder(scale);
		}

		public VectorPath Build()
		{
			return new VectorPath(segments);
		}

		public VectorPathBuilder LineTo(float x, float y)
		{
			var next = new FPoint(x * scale, y * scale);

			segments.Add(new LineSegment(current, next));
			current = next;

			return this;
		}

		public VectorPathBuilder LineTo(FPoint p)
		{
			var next = new FPoint(p.X * scale, p.Y * scale);

			segments.Add(new LineSegment(current, next));
			current = next;

			return this;
		}

		public VectorPathBuilder MoveTo(float x, float y)
		{
			current = new FPoint(x * scale, y * scale);

			return this;
		}

		public VectorPathBuilder MoveTo(FPoint p)
		{
			current = new FPoint(p.X * scale, p.Y * scale);

			return this;
		}

		public VectorPathBuilder HalfEllipseDown(float rx, float ry)
		{
			segments.Add(new EllipsePieceSegment(new FPoint(current.X, current.Y + ry * scale), rx * scale, ry * scale, FloatMath.RAD_NEG_090, FloatMath.RAD_POS_090));

			current = new FPoint(current.X, current.Y + 2 * ry * scale);

			return this;
		}

		public VectorPathBuilder HalfCircleDown(float r)
		{
			segments.Add(new CirclePieceSegment(new FPoint(current.X, current.Y + r * scale), r * scale, FloatMath.RAD_NEG_090, FloatMath.RAD_POS_090));

			current = new FPoint(current.X, current.Y + 2 * r * scale);

			return this;
		}

		public VectorPathBuilder HalfCircleLeft(float r)
		{
			segments.Add(new CirclePieceSegment(new FPoint(current.X - r * scale, current.Y), r * scale, FloatMath.RAD_000, FloatMath.RAD_POS_180));

			current = new FPoint(current.X - 2 * r * scale, current.Y);

			return this;
		}

		public VectorPathBuilder HalfCircleRight(float r)
		{
			segments.Add(new CirclePieceSegment(new FPoint(current.X + r * scale, current.Y), r * scale, FloatMath.RAD_POS_180, FloatMath.RAD_000));

			current = new FPoint(current.X + 2 * r * scale, current.Y);

			return this;
		}

		public VectorPathBuilder Ellipse(float rx, float ry, float cx, float cy, float degStart, float deggEnd)
		{
			var ell = new EllipsePieceSegment(new FPoint(cx * scale, cy * scale), rx * scale, ry * scale, degStart * FloatMath.DegRad, deggEnd * FloatMath.DegRad);

			segments.Add(ell);

			current = ell.GetEnd();

			return this;
		}

		public VectorPathBuilder EllipseCCW(float rx, float ry, float cx, float cy, float degStart, float deggEnd)
		{
			var ell = new EllipsePieceSegment(new FPoint(cx * scale, cy * scale), rx * scale, ry * scale, degStart * FloatMath.DegRad, deggEnd * FloatMath.DegRad, CircularDirection.CCW);

			segments.Add(ell);

			current = ell.GetEnd();

			return this;
		}

		public VectorPathBuilder FullEllipse(float rx, float ry, float cx, float cy)
		{
			var ell = new EllipsePieceSegment(new FPoint(cx * scale, cy * scale), rx * scale, ry * scale);

			segments.Add(ell);

			current = ell.GetEnd();

			return this;
		}

		public VectorPathBuilder FullEllipseCCW(float rx, float ry, float cx, float cy)
		{
			var ell = new EllipsePieceSegment(new FPoint(cx * scale, cy * scale), rx * scale, ry * scale, FloatMath.TAU, 0, dir:CircularDirection.CCW);

			segments.Add(ell);

			current = ell.GetEnd();

			return this;
		}
	}
}
