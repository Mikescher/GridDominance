using Microsoft.Xna.Framework;

namespace MonoSAMFramework.Portable.DebugDisplay
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

		public RealtimeAPSCounter(float updateInterval = 2.5f)
		{
			interval = updateInterval;

			lastUpdate = new GameTime().TotalGameTime.TotalSeconds;
			MinimumAPS = 0;
		}

		public void Reset()
		{
			TotalActions = 0;

			bufferCount = 0;
			bufferSum = 0;
		}

		public void Update(GameTime gt)
		{
			double delta = gt.TotalGameTime.TotalSeconds - lastUpdate;
			lastUpdate = gt.TotalGameTime.TotalSeconds;

			CurrentDelta = delta;
			CurrentAPS = 1.0 / delta;
			MinimumAPS = System.Math.Min(CurrentAPS, MinimumAPS);

			bufferSum += delta;
			bufferCount++;

			if (bufferSum > interval)
			{
				AverageAPS = bufferCount / bufferSum;
				AverageDelta = bufferSum / bufferCount;

				bufferSum = 0;
				bufferCount = 0;

				MinimumAPS = CurrentAPS;
			}

			TotalActions++;
		}
	}
}
