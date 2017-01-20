using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.DebugTools
{
	public class RealtimeAPSCounter
	{
		private readonly float interval;

		private double lastUpdate;
		private int bufferCount = 0;
		private double bufferSum = 0;

		public long TotalActions { get; private set; }

		public double CurrentAPS { get; private set; }
		public double CurrentDelta { get; private set; }

		public double AverageAPS { get; private set; }
		public double AverageDelta { get; private set; }

		public double MinimumAPS { get; private set; }
		public double MaximumDelta { get; private set; }

		public RealtimeAPSCounter(float updateInterval = 2.5f)
		{
			interval = updateInterval;

			lastUpdate = 0;
			MinimumAPS = 0;
			MaximumDelta = 0;
		}

		public void Reset()
		{
			TotalActions = 0;

			bufferCount = 0;
			bufferSum = 0;
		}

		public void Update(SAMTime gt)
		{
			double delta = gt.TotalElapsedSecondsDouble - lastUpdate;
			lastUpdate = gt.TotalElapsedSecondsDouble;

			CurrentDelta = delta;
			CurrentAPS = 1.0 / delta;
			MinimumAPS = System.Math.Min(CurrentAPS, MinimumAPS);
			MaximumDelta = System.Math.Max(CurrentDelta, MaximumDelta);

			bufferSum += delta;
			bufferCount++;

			if (bufferSum > interval)
			{
				AverageAPS = bufferCount / bufferSum;
				AverageDelta = bufferSum / bufferCount;

				bufferSum = 0;
				bufferCount = 0;

				MinimumAPS = CurrentAPS;
				MaximumDelta = CurrentDelta;
			}

			TotalActions++;
		}
	}
}
