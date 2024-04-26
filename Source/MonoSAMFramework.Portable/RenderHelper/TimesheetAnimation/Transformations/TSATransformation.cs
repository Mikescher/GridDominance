
using System;

namespace MonoSAMFramework.Portable.RenderHelper.TimesheetAnimation.Transformations
{
	public abstract class TSATransformation
	{
		public readonly float TimeStart;
		public readonly float TimeEnd;

		public readonly Func<float, float> TimeTransform;

		protected TSATransformation(float start, float end, Func<float, float> tt)
		{
			TimeStart = start;
			TimeEnd = end;
			TimeTransform = tt;
		}
	}
}
