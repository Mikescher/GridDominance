using Microsoft.Xna.Framework;

namespace MonoSAMFramework.Portable.Screens
{
	public sealed class SAMTime
	{
		public readonly long   TotalElapsedTicks;
		public readonly double TotalElapsedSecondsDouble;
		public readonly float  TotalElapsedSeconds;

		public readonly long   RealtimeElapsedTicks;
		public readonly double RealtimeElapsedSecondsDouble;
		public readonly float  RealtimeElapsedSeconds;

		public readonly long   ElapsedTicks;
		public readonly double ElapsedSecondsDouble;
		public readonly float  ElapsedSeconds;

		public readonly bool IsRunningSlowly;


		public SAMTime()
		{
			TotalElapsedTicks = RealtimeElapsedTicks = ElapsedTicks = 0;
			TotalElapsedSecondsDouble = RealtimeElapsedSecondsDouble = ElapsedSecondsDouble = 0;
			TotalElapsedSeconds = RealtimeElapsedSeconds = ElapsedSeconds = 0;
			IsRunningSlowly = false;
		}

		public SAMTime(GameTime source)
		{
			TotalElapsedTicks         = source.TotalGameTime.Ticks;
			TotalElapsedSecondsDouble = source.TotalGameTime.TotalSeconds;
			TotalElapsedSeconds       = (float)TotalElapsedSecondsDouble;

			RealtimeElapsedTicks         = source.ElapsedGameTime.Ticks;
			RealtimeElapsedSecondsDouble = source.ElapsedGameTime.TotalSeconds;
			RealtimeElapsedSeconds       = (float)RealtimeElapsedSecondsDouble;

			ElapsedTicks         = RealtimeElapsedTicks;
			ElapsedSecondsDouble = RealtimeElapsedSecondsDouble;
			ElapsedSeconds       = RealtimeElapsedSeconds;

			IsRunningSlowly = source.IsRunningSlowly;
		}

		public SAMTime(float delta, float time)
		{
			TotalElapsedTicks = (long)(time * 1000);
			TotalElapsedSecondsDouble = time;
			TotalElapsedSeconds = time;

			RealtimeElapsedTicks         = ElapsedTicks         = (long)(delta * 1000);
			RealtimeElapsedSecondsDouble = ElapsedSecondsDouble = delta;
			RealtimeElapsedSeconds       = ElapsedSeconds       = delta;

			IsRunningSlowly = false;
		}

		private SAMTime(SAMTime source, float multVirtual, float multReal)
		{
			TotalElapsedTicks         = source.TotalElapsedTicks;
			TotalElapsedSecondsDouble = source.TotalElapsedSecondsDouble;
			TotalElapsedSeconds       = source.TotalElapsedSeconds;

			RealtimeElapsedTicks         = (long)(multReal * source.RealtimeElapsedTicks);
			RealtimeElapsedSecondsDouble = multReal * source.RealtimeElapsedSecondsDouble;
			RealtimeElapsedSeconds       = multReal * source.RealtimeElapsedSeconds;

			ElapsedTicks         = (long)(multVirtual * source.ElapsedTicks);
			ElapsedSecondsDouble = multVirtual * source.ElapsedSecondsDouble;
			ElapsedSeconds       = multVirtual * source.ElapsedSeconds;

			IsRunningSlowly = source.IsRunningSlowly;
		}

		public SAMTime Stretch(float multVirtual, float multReal)
		{
			return new SAMTime(this, multVirtual, multReal);
		}
	}
}
