using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.GameMath.VectorPath
{
	public class LineSegment : VectorPathSegment
	{
		private readonly FPoint startPoint;
		private readonly FPoint endPoint;

		private readonly Vector2 direction;

		public override float Length { get; }
		public override FRectangle Boundings { get; }

		public LineSegment(FPoint start, FPoint end)
		{
			startPoint = start;
			endPoint = end;

			direction = (end - start).Normalized();

			Length = (end - start).Length();
			Boundings = new FRectangle(FloatMath.Min(startPoint.X, endPoint.X), FloatMath.Min(startPoint.Y, endPoint.Y), FloatMath.Abs(startPoint.X - endPoint.X), FloatMath.Abs(startPoint.Y - endPoint.Y));
		}

		public override FPoint Get(float len)
		{
			len = FloatMath.Clamp(len, 0, Length);

			return startPoint + direction * len;
		}

		public override VectorPathSegment AsScaled(float scale)
		{
			return new LineSegment(new FPoint(startPoint.X * scale, startPoint.Y * scale), new FPoint(endPoint.X * scale, endPoint.Y * scale));
		}
	}
}
