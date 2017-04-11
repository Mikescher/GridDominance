using MonoSAMFramework.Portable.Screens;
using System;

namespace MonoSAMFramework.Portable.DebugTools
{
	public class RealtimeAPSCounter
	{
		private readonly float interval;

		private double lastUpdate;
		private int bufferCount = 0;
		private double bufferSum = 0;
		private int cycleStartTime = 0;
		private int cycleTimeSum = 0;

		public long TotalActions { get; private set; }

		public double CurrentAPS { get; private set; }
		public double AverageAPS { get; private set; }
		public double MinimumAPS { get; private set; }

		public double CurrentDelta { get; private set; }
		public double AverageDelta { get; private set; }
		public double MaximumDelta { get; private set; }

		public double CurrentCycleTime { get; private set; }
		public double AverageCycleTime { get; private set; }
		public double MaximumCycleTime { get; private set; }
		
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

		public void StartCycle(SAMTime gt)
		{
			double delta = gt.TotalElapsedSecondsDouble - lastUpdate;
			lastUpdate = gt.TotalElapsedSecondsDouble;

			CurrentDelta = delta;
			CurrentAPS = 1.0 / delta;
			MinimumAPS = System.Math.Min(CurrentAPS, MinimumAPS);
			MaximumDelta = System.Math.Max(CurrentDelta, MaximumDelta);

			bufferSum += delta;
			bufferCount++;

			cycleStartTime = Environment.TickCount;
		}

		public void EndCycle()
		{
			var ctime = Environment.TickCount - cycleStartTime;
			cycleTimeSum += ctime;
			MaximumCycleTime = Math.Max(MaximumCycleTime, ctime / 1000f);
			CurrentCycleTime = ctime / 1000f;

			if (ctime > 0)
			{
				System.Diagnostics.Debug.WriteLine("sad");
			}

			if (bufferSum > interval)
			{
				AverageAPS = bufferCount / bufferSum;
				AverageDelta = bufferSum / bufferCount;
				AverageCycleTime = cycleTimeSum / (1000f * bufferCount);

				bufferSum = 0;
				cycleTimeSum = 0;
				bufferCount = 0;

				MinimumAPS = CurrentAPS;
				MaximumDelta = CurrentDelta;
				MaximumCycleTime = AverageCycleTime;
			}

			TotalActions++;
		}
	}
}
