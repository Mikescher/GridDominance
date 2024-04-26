using System.Collections.Generic;
using System.Linq;
using MonoSAMFramework.Portable.GameMath;

namespace MonoSAMFramework.Portable.DebugTools
{
	public class ExtendedFrequencyCounter<TKey>
	{
		private float HistoryDuration = 1f;
		private Dictionary<TKey, int> History = new Dictionary<TKey, int>();

		public string HistoryStr => "[" + string.Join(", ", History.OrderBy(p => p.Key).Select(p => $"{p.Key}: {(p.Value / HistoryDuration):0.0}")) + "]";

		private Dictionary<TKey, int> _historyNow = new Dictionary<TKey, int>();

		private readonly float updateTime;
		private readonly int updateCount;

		public float Frequency { get; private set; }

		private float lastUpdate;
		private int count;

		public ExtendedFrequencyCounter(float limTime, int limCount)
		{
			Frequency = 1;
			lastUpdate = 0;
			count = 0;

			updateTime = limTime;
			updateCount = limCount;
		}

		public void Inc(float secs, TKey hint)
		{
			count++;

			int c;
			if (_historyNow.TryGetValue(hint, out c)) _historyNow[hint] = c + 1; else _historyNow[hint] = 1;

			Calc(secs);
		}

		private void Calc(float secs)
		{
			float delta = secs - lastUpdate;
			if (delta > updateTime && count > updateCount)
			{
				History = _historyNow;
				_historyNow = new Dictionary<TKey, int>(History.Count);
				Frequency = FloatMath.Max(1, count / delta);
				HistoryDuration = delta;

				count = 0;
				lastUpdate = secs;
			}
		}
	}
}
