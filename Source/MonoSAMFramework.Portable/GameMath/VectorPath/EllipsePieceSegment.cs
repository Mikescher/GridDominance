using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.GameMath.VectorPath
{
	public class EllipsePieceSegment : VectorPathSegment
	{
		private readonly FPoint center;
		private readonly float radiusX;
		private readonly float radiusY;

		private readonly float angleStart;
		private readonly float angleEnd;

		private CircularDirection direction = CircularDirection.CW;

		public override float Length { get; }
		public override FRectangle Boundings { get; }

		public EllipsePieceSegment(FPoint center, float radiusX, float radiusY, float aStart = 0, float aEnd = FloatMath.TAU, CircularDirection dir = CircularDirection.CW)
		{
			this.center = center;
			this.radiusX = radiusX;
			this.radiusY = radiusY;
			this.direction = dir;

			angleStart = aStart;
			angleEnd = aEnd;

			if (dir == CircularDirection.CW)
			{
				Length = FloatMath.TAU * FloatMath.Sqrt((radiusX * radiusX + radiusY * radiusY) / 2f) * (angleEnd - angleStart) / FloatMath.TAU;

				Boundings = GeometryHelper.CalculateEllipseSegmentsBoundingBox(center.X, center.Y, radiusX, radiusY, angleStart, angleEnd);
			}
			else
			{
				// inverted
				
				Length = FloatMath.TAU * FloatMath.Sqrt((radiusX * radiusX + radiusY * radiusY) / 2f) * (angleStart - angleEnd) / FloatMath.TAU;

				Boundings = GeometryHelper.CalculateEllipseSegmentsBoundingBox(center.X, center.Y, radiusX, radiusY, angleEnd, angleStart);
			}


		}

		public override FPoint Get(float len)
		{
			if (direction == CircularDirection.CW)
			{
				len = FloatMath.Clamp(len, 0, Length);

				var angle = angleStart + (angleEnd - angleStart) * (len / Length);

				return GeometryHelper.GetPointOnEllipse(center.X, center.Y, radiusX, radiusY, angle);
			}
			else
			{
				len = FloatMath.Clamp(len, 0, Length);

				var angle = angleStart + (angleEnd - angleStart) * (len / Length);

				return GeometryHelper.GetPointOnEllipse(center.X, center.Y, radiusX, radiusY, angle);
			}
		}

		public override VectorPathSegment AsScaled(float scale)
		{
			return new EllipsePieceSegment(new FPoint(center.X * scale, center.Y * scale), radiusX * scale, radiusY * scale, angleStart, angleEnd, direction);
		}
	}
}
