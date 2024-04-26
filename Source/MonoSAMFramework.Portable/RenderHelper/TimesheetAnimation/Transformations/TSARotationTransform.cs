using System;

namespace MonoSAMFramework.Portable.RenderHelper.TimesheetAnimation.Transformations
{
	public sealed class TSARotationTransform : TSATransformation
	{
		public readonly float RotationStart;
		public readonly float RotationFinal;

		public TSARotationTransform(float start, float rotStart, float end, float rotEnd, Func<float, float> tt) : base(start, end, tt)
		{
			RotationStart = rotStart;
			RotationFinal = rotEnd;
		}
	}
}
