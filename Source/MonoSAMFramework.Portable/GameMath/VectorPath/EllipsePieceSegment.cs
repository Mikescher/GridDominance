using MonoSAMFramework.Portable.GameMath.FloatClasses;

namespace MonoSAMFramework.Portable.GameMath.VectorPath
{
	public class EllipsePieceSegment : VectorPathSegment
	{
		private readonly FPoint center;
		private readonly float radiusX;
		private readonly float radiusY;

		private readonly float angleStart;
		private readonly float angleEnd;

		public override float Length { get; }
		public override FRectangle Boundings { get; }

		public EllipsePieceSegment(FPoint center, float radiusX, float radiusY, float aStart = 0, float aEnd = FloatMath.TAU)
		{
			this.center = center;
			this.radiusX = radiusX;
			this.radiusY = radiusY;

			angleStart = aStart;
			angleEnd = aEnd;
			
			Length = FloatMath.TAU * FloatMath.Sqrt((radiusX * radiusX + radiusY * radiusY)/2f) * (angleEnd - angleStart)/FloatMath.TAU;

			Boundings = new FRectangle(center.X - radiusX, center.Y - radiusY, 2*radiusX, 2*radiusY); // TODO This is wrong - use correct calculations: http://mathoverflow.net/questions/93659
		}

		public override FPoint Get(float len)
		{
			len = FloatMath.Clamp(len, 0, Length);

			var angle = angleStart + (angleEnd - angleStart) * (len / Length);
			var omega = GetOmega(angle);

			var x = FloatMath.Cos(omega) * radiusX;
			var y = FloatMath.Sin(omega) * radiusY;

			return new FPoint(x + center.X, y + center.Y);
		}

		public override VectorPathSegment AsScaled(float scale)
		{
			return new EllipsePieceSegment(new FPoint(center.X * scale, center.Y * scale), radiusX * scale, radiusY * scale, angleStart, angleEnd);
		}

		private float GetOmega(float angle)
		{
			angle = FloatMath.PositiveModulo(angle, FloatMath.TAU);
			
			if (angle < FloatMath.RAD_POS_090)
				return FloatMath.Atan(radiusX * FloatMath.Tan(angle) / radiusY);
			else if (angle < FloatMath.RAD_POS_270)
				return FloatMath.Atan(radiusX * FloatMath.Tan(angle) / radiusY) - FloatMath.PI;
			else
				return FloatMath.Atan(radiusX * FloatMath.Tan(angle) / radiusY);
		}
	}
}
