using System.Diagnostics;
using MonoSAMFramework.Portable.GameMath;

namespace MonoSAMFramework.Portable.DebugTools
{
	public class TimingCounter
	{
		private const float LOWER_BORDER = 0.0001f;
		
		private readonly Stopwatch _stopwatch = new Stopwatch();

		private readonly int _historysize;
		private readonly long[] _history;

		private int _nextIdx = 0;

		public float AverageTiming;
		public float MinTiming;
		public float MaxTiming;
		public int TimingCount;

		public TimingCounter(int historysize)
		{
			_historysize = historysize;
			_history = new long[historysize];
			for (int i = 0; i < historysize; i++) _history[_nextIdx] = -1;
		}

		public void Start()
		{
			_stopwatch.Restart();
		}

		public void Stop()
		{
			_history[_nextIdx] = _stopwatch.ElapsedMilliseconds;
			_nextIdx = (_nextIdx + 1) % _historysize;


			Recalc();
		}

		private void Recalc()
		{
			MinTiming = +999999999;
			MaxTiming = -999999999;
			AverageTiming = 0;

			TimingCount = 0;

			for (int i = 0; i < _historysize; i++)
			{
				if (_history[i] < 0) continue;

				TimingCount++;
				MinTiming = FloatMath.Min(MinTiming, _history[i]);
				MaxTiming = FloatMath.Max(MaxTiming, _history[i]);

				AverageTiming += _history[i];
			}
			
			AverageTiming /= TimingCount;
		}

		public string Format() => $"{AverageTiming:0.0000}ms (min:{MinTiming:0.0000}s | max:{MaxTiming:0.0000}ms" + (TimingCount < _historysize ? $" | cnt:{TimingCount}/{_historysize}" : "") + ")";
	}
}
