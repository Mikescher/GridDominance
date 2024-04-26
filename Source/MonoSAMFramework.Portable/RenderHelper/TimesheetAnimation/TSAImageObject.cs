using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Language;
using MonoSAMFramework.Portable.RenderHelper.TimesheetAnimation.Transformations;

namespace MonoSAMFramework.Portable.RenderHelper.TimesheetAnimation
{
	public class TSAImageObject : TSAObject
	{
		private class BoundComp : Comparer<TSABoundingTransform>
		{
			public override int Compare(TSABoundingTransform x, TSABoundingTransform y) => (x == null || y == null) ? 0 : x.TimeStart.CompareTo(y.TimeStart);
		}

		private class ColComp : Comparer<TSAColorTransform>
		{
			public override int Compare(TSAColorTransform x, TSAColorTransform y) => (x == null || y == null) ? 0 : x.TimeStart.CompareTo(y.TimeStart);
		}

		private class RotComp : Comparer<TSARotationTransform>
		{
			public override int Compare(TSARotationTransform x, TSARotationTransform y) => (x == null || y == null) ? 0 : x.TimeStart.CompareTo(y.TimeStart);
		}

		private readonly TextureRegion2D _texture;

		private readonly FRectangle _rect0;
		private readonly Color _col0;
		private readonly float _rot0;

		private readonly List<TSABoundingTransform> transformationsBounding = new AlwaysSortList<TSABoundingTransform>(new BoundComp());
		private readonly List<TSAColorTransform> transformationsColor = new AlwaysSortList<TSAColorTransform>(new ColComp());
		private readonly List<TSARotationTransform> transformationsRotation = new AlwaysSortList<TSARotationTransform>(new RotComp());

		public TSAImageObject(TextureRegion2D tex, FRectangle rectInitial, Color colInitial, float rotInitial, int order) : base(order)
		{
			_texture = tex;
			_rect0 = rectInitial;
			_col0 = colInitial;
			_rot0 = rotInitial;
		}

		public override void Draw(IBatchRenderer sbatch, FPoint offset, float scale, float time)
		{
			FRectangle stateBounds;
			Color stateColor;
			float stateRotation;

			GetState(time, out stateBounds, out stateColor, out stateRotation);

			stateBounds = stateBounds.AsScaledAndTranslated(scale, offset);

			if (stateBounds.IsEmpty) return;
			if (stateColor.A == 0) return;

			sbatch.DrawStretched(_texture, stateBounds, stateColor, stateRotation);
		}

		public void AddBoundingTransformation(float tstart, FRectangle vstart, float tend, FRectangle vend, Func<float, float> tt)
		{
			transformationsBounding.Add(new TSABoundingTransform(tstart, vstart, tend, vend, tt));
		}

		public void AddColorTransformation(float tstart, Color vstart, float tend, Color vend, Func<float, float> tt)
		{
			transformationsColor.Add(new TSAColorTransform(tstart, vstart, tend, vend, tt));
		}

		public void AddRotationTransformation(float tstart, float vstart, float tend, float vend, Func<float, float> tt)
		{
			transformationsRotation.Add(new TSARotationTransform(tstart, vstart, tend, vend, tt));
		}

		private void GetState(float time, out FRectangle rect, out Color col, out float rot)
		{
			rect = _rect0;
			foreach (var tf in transformationsBounding)
			{
				if (time < tf.TimeStart)
				{
					// nothing
				}
				else if (time > tf.TimeEnd)
				{
					rect = tf.BoundingsFinal;
				}
				else
				{
					var p = tf.TimeTransform((time - tf.TimeStart) / (tf.TimeEnd - tf.TimeStart));
					rect = FRectangle.Lerp(tf.BoundingsStart, tf.BoundingsFinal, p);
				}
			}
			
			col = _col0;
			foreach (var tf in transformationsColor)
			{
				if (time < tf.TimeStart)
				{
					// nothing
				}
				else if (time > tf.TimeEnd)
				{
					col = tf.ColorFinal;
				}
				else
				{
					var p = tf.TimeTransform((time - tf.TimeStart) / (tf.TimeEnd - tf.TimeStart));
					col = ColorMath.Blend(tf.ColorStart, tf.ColorFinal, p);
				}
			}
			
			rot = _rot0;
			foreach (var tf in transformationsRotation)
			{
				if (time < tf.TimeStart)
				{
					// nothing
				}
				else if (time > tf.TimeEnd)
				{
					rot = tf.RotationFinal;
				}
				else
				{
					var p = tf.TimeTransform((time - tf.TimeStart) / (tf.TimeEnd - tf.TimeStart));
					rot = FloatMath.Lerp(tf.RotationStart, tf.RotationFinal, p);
				}
			}
		}
	}
}
