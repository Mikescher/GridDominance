using MonoSAMFramework.Portable.GameMath.FloatClasses;
using System.Collections.Generic;

namespace MonoSAMFramework.Portable.GameMath.VectorPath
{
	public class VectorPathBuilder
	{
		private List<VectorPathSegment> segments = new List<VectorPathSegment>();
		private FPoint current;
		private float scale;

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

		public VectorPathBuilder MoveTo(float x, float y)
		{
			current = new FPoint(x * scale, y * scale);

			return this;
		}

		public VectorPathBuilder HalfEllipseDown(int rx, int ry)
		{
			segments.Add(new EllipsePieceSegment(new FPoint(current.X, current.Y + ry * scale), rx * scale, ry * scale, FloatMath.RAD_NEG_090, FloatMath.RAD_POS_090));

			current = new FPoint(current.X, current.Y + 2 * ry * scale);

			return this;
		}

		public VectorPathBuilder HalfCircleDown(int r)
		{
			segments.Add(new CirclePieceSegment(new FPoint(current.X, current.Y + r * scale), r * scale, FloatMath.RAD_NEG_090, FloatMath.RAD_POS_090));

			current = new FPoint(current.X, current.Y + 2 * r * scale);

			return this;
		}
	}
}
