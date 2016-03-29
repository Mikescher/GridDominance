using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace MonoSAMFramework.Portable.MathHelper
{
	public class DeltaLimitedFloat
	{
		private readonly float deltaSpeed; // 1/sec

		public float ActualValue { get; private set; }
		public float TargetValue { get; private set; }

		public DeltaLimitedFloat(float initialValue, float maxDelta)
		{
			deltaSpeed = maxDelta;
		}
		
		public void Update(GameTime gameTime)
		{
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (ActualValue < TargetValue)
			{
				ActualValue += deltaSpeed * gameTime.GetElapsedSeconds();
				if (ActualValue > TargetValue) ActualValue = TargetValue;
			}
			else if (ActualValue > TargetValue)
			{
				ActualValue -= deltaSpeed * gameTime.GetElapsedSeconds();
				if (ActualValue < TargetValue) ActualValue = TargetValue;
			}
		}

		public void Set(float v)
		{
			TargetValue = v;
		}

		public void SetForce(float v)
		{
			TargetValue = v;
			ActualValue = v;
		}

		public void Inc(float vadd)
		{
			TargetValue += vadd;
		}

		public void Dec(float vsub)
		{
			TargetValue -= vsub;
		}

		public bool IsDecreasing() => ActualValue > TargetValue;
		public bool IsIncreasing() => ActualValue < TargetValue;

		/// <summary>
		/// Result:
		/// -1 => lower limit forced
		/// 00 => no changes
		/// +1 => upper limit forced
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public int Limit(float min, float max)
		{
			int r = 0;

			if (TargetValue < min)
			{
				TargetValue = min;
				r = -1;
			}
			if (TargetValue > max)
			{
				TargetValue = max;
				r = +1;
			}

			return r;
		}

	}
}
