using System;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.RenderHelper.TimesheetAnimation.Transformations
{
	public sealed class TSABoundingTransform : TSATransformation
	{
		public readonly FRectangle BoundingsStart;
		public readonly FRectangle BoundingsFinal;

		public TSABoundingTransform(float start, FRectangle boundsStart, float end, FRectangle boundsEnd, Func<float, float> tt) : base(start, end, tt)
		{
			BoundingsStart = boundsStart;
			BoundingsFinal = boundsEnd;
		}
	}
}
