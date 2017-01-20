using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.Screens;
using System;

namespace MonoSAMFramework.Portable.DebugTools
{
	public class GCMonitor : ISAMUpdateable
	{
		private readonly FrequencyCounter freq = new FrequencyCounter(1f, 8); 

		private int collCount0 = 0;
		private int collCount1 = 0;
		private int collCount2 = 0;

		public float LastGC0 = 0;
		public float LastGC1 = 0;
		public float LastGC2 = 0;

		public float TimeSinceLastGC  => FloatMath.Min(TimeSinceLastGC0, TimeSinceLastGC1, TimeSinceLastGC2);
		public float TimeSinceLastGC0 => MonoSAMGame.CurrentTime.TotalElapsedSeconds - LastGC0;
		public float TimeSinceLastGC1 => MonoSAMGame.CurrentTime.TotalElapsedSeconds - LastGC1;
		public float TimeSinceLastGC2 => MonoSAMGame.CurrentTime.TotalElapsedSeconds - LastGC2;

		public float TotalMemory = 0; // Megabytes
		public float GCFrequency => freq.Frequency;

		public void Update(SAMTime gameTime, InputState istate)
		{
			float sec = gameTime.TotalElapsedSeconds;
			if (collCount0 != GC.CollectionCount(0)) { collCount0 = GC.CollectionCount(0); LastGC0 = sec; freq.Inc(sec); }
			if (collCount1 != GC.CollectionCount(1)) { collCount1 = GC.CollectionCount(1); LastGC1 = sec; freq.Inc(sec); }
			if (collCount2 != GC.CollectionCount(2)) { collCount2 = GC.CollectionCount(2); LastGC2 = sec; freq.Inc(sec); }

			TotalMemory = GC.GetTotalMemory(false) / (1024f * 1024f);
		}
	}
}
