using Microsoft.Xna.Framework;
using System;

namespace MonoSAMFramework.Portable.RenderHelper.TimesheetAnimation.Transformations
{
	public sealed class TSAColorTransform : TSATransformation
	{
		public readonly Color ColorStart;
		public readonly Color ColorFinal;

		public TSAColorTransform(float start, Color colStart, float end, Color colEnd, Func<float, float> tt) : base(start, end, tt)
		{
			ColorStart = colStart;
			ColorFinal = colEnd;
		}
	}
}
