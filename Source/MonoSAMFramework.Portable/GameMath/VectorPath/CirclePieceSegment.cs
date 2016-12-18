using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.GameMath.VectorPath
{
	public class CirclePieceSegment : VectorPathSegment
	{
		private readonly FPoint center;
		private readonly float radius;

		private readonly float angleStart;
		private readonly float angleEnd;

		private readonly Vector2 directionZero;

		public override float Length { get; }
		public override FRectangle Boundings { get; }

		public readonly CircularDirection direction;

		public CirclePieceSegment(FPoint center, float radius, float aStart = 0, float aEnd = FloatMath.TAU)
		{
			this.center = center;
			this.radius = radius;

			if (aStart < aEnd)
			{
				direction = CircularDirection.CW;

				angleStart = aStart;
				angleEnd = aEnd;
			}
			else
			{
				direction = CircularDirection.CCW;

				angleStart = aEnd;
				angleEnd = aStart;
			}

			Length = (2 * FloatMath.PI * radius) * (angleEnd - angleStart) / FloatMath.TAU;
			directionZero = new Vector2(radius, 0);
			
			Boundings = GeometryHelper.CalculateEllipseSegmentsBoundingBox(center.X, center.Y, radius, radius, angleStart, angleEnd);
		}

		public override FPoint Get(float len)
		{
			len = FloatMath.Clamp(len, 0, Length);

			if (direction == CircularDirection.CCW) len = Length - len;

			return (directionZero.Rotate(angleStart + (angleEnd - angleStart) * (len / Length)) + center).ToFPoint();
		}

		public override VectorPathSegment AsScaled(float scale)
		{
			if (direction == CircularDirection.CW)
				return new CirclePieceSegment(new FPoint(center.X * scale, center.Y * scale), radius * scale, angleStart, angleEnd);
			else
				return new CirclePieceSegment(new FPoint(center.X * scale, center.Y * scale), radius * scale, angleEnd, angleStart);
		}
	}
}
