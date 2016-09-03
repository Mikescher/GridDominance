using System.Collections.Generic;
using System.Linq;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.GameMath.VectorPath
{
	public class VectorPath
	{
		private readonly List<VectorPathSegment> segments;

		public IEnumerable<VectorPathSegment> Segments => segments;

		public float Length { get; }
		public FRectangle Boundings { get; }

		public VectorPath(List<VectorPathSegment> seg)
		{
			segments = seg;
			Length = segments.Sum(p => p.Length);

			float top = segments.Min(p => p.Boundings.Top);
			float bot = segments.Max(p => p.Boundings.Bottom);
			float lef = segments.Min(p => p.Boundings.Left);
			float rig = segments.Max(p => p.Boundings.Right);

			Boundings = new FRectangle(lef, top, rig-lef, bot-top);
		}

		public FPoint Get(float len)
		{
			len = FloatMath.Clamp(len, 0, Length);

			foreach (VectorPathSegment segment in segments)
			{
				if (len < segment.Length)
				{
					return segment.Get(len);
				}
				else
				{
					len -= segment.Length;
				}
			}

			var last = segments.Last();

			return last.Get(last.Length);
		}

		public VectorPath AsScaled(float scale)
		{
			return new VectorPath(segments.Select(p => p.AsScaled(scale)).ToList());
		}
	}
}
