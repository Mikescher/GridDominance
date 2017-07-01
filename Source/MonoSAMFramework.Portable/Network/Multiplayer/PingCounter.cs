namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	public class PingCounter
	{
		private readonly int updateCount;

		public float Value { get; private set; }

		private float sum;
		private int count;

		public PingCounter(int limCount)
		{
			Value = 0;
			sum = 0;
			count = 0;

			updateCount = limCount;
		}

		public void Inc(float secs)
		{
			count++;
			sum += secs;

			if (count > updateCount)
			{
				Value = sum / count;

				count = 0;
			}
		}
	}
}
