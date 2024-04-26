namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	public class NetworkUserConn
	{
		public readonly PingCounter InGamePing = new PingCounter(3);
		public float LastSendPingTime = 0f;
		public byte PingSeq = 0;

		public readonly float[] SendPingTime = new float[256];
		public readonly bool[]  SendPingAck  = new bool[256];

		public float LastResponse = 0f;

		public byte LastRecievedSeq = 0;

		public void SetPingSequenceCounterAndInvalidateLastPing(ref byte target)
		{
			var now = MonoSAMGame.CurrentTime.TotalElapsedSeconds;

			var invID = (PingSeq - 3 + 256) % 256;
			if (!SendPingAck[invID])
			{
				SendPingAck[invID] = true;
				if (SendPingTime[invID] > 0) InGamePing.Inc(now - SendPingTime[invID]);
			}

			target = PingSeq;

			SendPingAck[PingSeq] = false;
			SendPingTime[PingSeq] = now;

			PingSeq++;

			LastSendPingTime = now;
		}

		public void RecievePingAck(byte seq)
		{
			SendPingAck[seq] = true;

			InGamePing.Inc(MonoSAMGame.CurrentTime.TotalElapsedSeconds - SendPingTime[seq]);
		}
	}
}
