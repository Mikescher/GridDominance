using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Language;

namespace MonoSAMFramework.Portable.RenderHelper.TimesheetAnimation
{
	public sealed class TimesheetAnimation
	{
		private class TSAComparer : Comparer<TSAObject>
		{
			public override int Compare(TSAObject x, TSAObject y) => (x == null || y == null) ? 0 : x.Order.CompareTo(y.Order);
		}
		
		public float Duration = 1;
		public bool Loop = false;
		public float Width = 0;
		public float Height = 0;
		public float SpeedFactor = 1;

		public readonly List<TSAObject> Elements = new AlwaysSortList<TSAObject>(new TSAComparer());

		public void Draw(IBatchRenderer sbatch, FRectangle bounds, float time)
		{
			time *= SpeedFactor;

			if (Loop)
				time = time % Duration;
			else
				time = FloatMath.Min(time, Duration);

			var newbounds = bounds.SetRatioUnderfitKeepCenter(Width / Height);

			foreach (var e in Elements)
			{
				e.Draw(sbatch, newbounds.TopLeft, newbounds.Width / Width, time);
			}
		}

		public void AddElement(TSAObject obj)
		{
			Elements.Add(obj);
		}

		public TSAImageObject AddImageOffset(TextureRegion2D tex, float offx, float offy, float x, float y, float w, float h, Color c)
		{
			var obj = new TSAImageObject(tex, new FRectangle(offx+x, offy+y, w, h), c, 0f, Elements.Any() ? Elements.Last().Order + 1 : 1);
			AddElement(obj);
			return obj;
		}

		public TSAImageObject AddImage(TextureRegion2D tex, float x, float y, float w, float h, Color c)
		{
			var obj = new TSAImageObject(tex, new FRectangle(x, y, w, h), c, 0f, Elements.Any() ? Elements.Last().Order + 1 : 1);
			AddElement(obj);
			return obj;
		}

		public TSAImageObject AddImage(TextureRegion2D tex, FRectangle r, Color c)
		{
			var obj = new TSAImageObject(tex, r, c, 0f, Elements.Any() ? Elements.Last().Order + 1 : 1);
			AddElement(obj);
			return obj;
		}
	}
}
