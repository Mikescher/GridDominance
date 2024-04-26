using MonoSAMFramework.Portable.GameMath;

namespace MonoSAMFramework.Portable.DebugTools
{
	public class FrequencyCounter
	{
		private readonly float updateTime;
		private readonly int updateCount;

		public float Frequency { get; private set; }

		private float lastUpdate;
		private int count;

		public FrequencyCounter(float limTime, int limCount)
		{
			Frequency = 1;
			lastUpdate = 0;
			count = 0;

			updateTime = limTime;
			updateCount = limCount;
		}

		public void Inc(float secs)
		{
			count++;

			Calc(secs);
		}

		private void Calc(float secs)
		{
			float delta = secs - lastUpdate;
			if (delta > updateTime && count > updateCount)
			{
				Frequency = FloatMath.Max(1, count / delta);


				count = 0;
				lastUpdate = secs;
			}
		}
	}
}
