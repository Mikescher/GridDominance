namespace MonoSAMFramework.Portable.GameMath.Geometry
{
	public static class EllipseHelper
	{
		public static FRectangle CalculateEllipseSegmentsBoundingBox(float cx, float cy, float rx, float ry, float aStart, float aEnd)
		{
			float minX = 0;
			float minY = 0;
			float maxX = 0;
			float maxY = 0;

			if (FloatMath.ArcContainsAngle(aStart, aEnd, FloatMath.RAD_POS_270))
			{
				minY = -ry;
			}
			else
			{
				minY = FloatMath.Min(minY, GetPointYOnEllipse(0, 0, rx, ry, aStart));
				minY = FloatMath.Min(minY, GetPointYOnEllipse(0, 0, rx, ry, aEnd));
			}

			if (FloatMath.ArcContainsAngle(aStart, aEnd, FloatMath.RAD_POS_090))
			{
				maxY = +ry;
			}
			else
			{
				maxY = FloatMath.Max(maxY, GetPointYOnEllipse(0, 0, rx, ry, aStart));
				maxY = FloatMath.Max(maxY, GetPointYOnEllipse(0, 0, rx, ry, aEnd));
			}

			if (FloatMath.ArcContainsAngle(aStart, aEnd, FloatMath.RAD_POS_180))
			{
				minX = -rx;
			}
			else
			{
				minX = FloatMath.Min(minX, GetPointXOnEllipse(0, 0, rx, ry, aStart));
				minX = FloatMath.Min(minX, GetPointXOnEllipse(0, 0, rx, ry, aEnd));
			}

			if (FloatMath.ArcContainsAngle(aStart, aEnd, FloatMath.RAD_000))
			{
				maxX = +rx;
			}
			else
			{
				maxX = FloatMath.Max(maxX, GetPointXOnEllipse(0, 0, rx, ry, aStart));
				maxX = FloatMath.Max(maxX, GetPointXOnEllipse(0, 0, rx, ry, aEnd));
			}

			return new FRectangle(cx + minX, cy + minY, maxX-minX, maxY-minY);
		}

		public static FPoint GetPointOnEllipse(float cx, float cy, float rx, float ry, float angle)
		{
			var t = GetEllipseTheta(angle, rx, ry);

			var x = FloatMath.Cos(t) * rx;
			var y = FloatMath.Sin(t) * ry;

			return new FPoint(x + cx, y + cy);
		}

		public static float GetPointXOnEllipse(float cx, float cy, float rx, float ry, float angle)
		{
			var t = GetEllipseTheta(angle, rx, ry);

			return cx + FloatMath.Cos(t) * rx;
		}

		public static float GetPointYOnEllipse(float cx, float cy, float rx, float ry, float angle)
		{
			var t = GetEllipseTheta(angle, rx, ry);

			return cy + FloatMath.Sin(t) * ry;
		}

		private static float GetEllipseTheta(float angle, float radiusX, float radiusY)
		{
			angle = FloatMath.NormalizeAngle(angle);

			if (angle < FloatMath.RAD_POS_090)
				return FloatMath.Atan(radiusX * FloatMath.Tan(angle) / radiusY);
			else if (angle < FloatMath.RAD_POS_270)
				return FloatMath.Atan(radiusX * FloatMath.Tan(angle) / radiusY) - FloatMath.PI;
			else
				return FloatMath.Atan(radiusX * FloatMath.Tan(angle) / radiusY);
		}
	}
}
