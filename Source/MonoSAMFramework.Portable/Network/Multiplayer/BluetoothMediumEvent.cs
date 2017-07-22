namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	public class BluetoothMediumEvent
	{
		public enum BTEvent { TryConnection, ConnectionFailed, ConnectionSucceeded }

		public readonly BTEvent Type;
		public readonly string Param;

		public BluetoothMediumEvent(BTEvent t, string p)
		{
			Type = t;
			Param = p;
		}
	}
}
