namespace MonoSAMFramework.Portable.Extensions
{
	public static class TimeExtension
	{
		public static string FormatMilliseconds(int v)
		{
			if (v < 0) return string.Empty; 

			var minutes = (int)(v / 1000f / 60f);
			var seconds = (int)((v - minutes * 1000 * 60) / 1000f);
			var millis = v - minutes * 1000 * 60 - seconds * 1000;

			return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, millis);
		}
	}
}
