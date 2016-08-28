using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoSAMFramework.Portable.MathHelper.FloatClasses;

namespace MonoSAMFramework.Portable.MathHelper.VectorPath
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

			direction = (end - start).NormalizedCopy();

			Length = (end - start).Length();
			Boundings = new FRectangle(FloatMath.Min(startPoint.X, endPoint.X), FloatMath.Min(startPoint.Y, endPoint.Y), FloatMath.Abs(startPoint.X - endPoint.X), FloatMath.Abs(startPoint.Y - endPoint.Y));
		}

		public override FPoint Get(float len)
		{
			len = FloatMath.Clamp(len, 0, Length);

			return startPoint + direction * len;
		}
	}
}
