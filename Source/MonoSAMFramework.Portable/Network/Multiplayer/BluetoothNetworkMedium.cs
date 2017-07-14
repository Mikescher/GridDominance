using System.Linq;

namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	public class BluetoothNetworkMedium : INetworkMedium
	{
		private readonly IBluetoothAdapter _client;
		
		public BluetoothNetworkMedium()
		{
			_client = MonoSAMGame.CurrentInst.Bridge.Bluetooth;
		}

		public void Init(out SAMNetworkConnection.ErrorType error)
		{
			_client.StartAdapter();

			if (_client.State == BluetoothAdapterState.AdapterNotFound)
			{
				error = SAMNetworkConnection.ErrorType.BluetoothAdapterNotFound;
			}
			else if (_client.State == BluetoothAdapterState.Error)
			{
				error = SAMNetworkConnection.ErrorType.BluetoothInternalError;
			}
			else
			{
				error = SAMNetworkConnection.ErrorType.None;
			}
		}

		public byte[] RecieveOrNull()
		{
			var d = _client.RecieveOrNull();
			return d.Item1.Take(d.Item2).ToArray();
		}

		public void Send(byte[] data)
		{
			_client.Write(data);
		}

		public void Send(byte[] data, int len)
		{
			if (data.Length == len)
				_client.Write(data);
			else
				_client.Write(data.Take(len).ToArray());
		}

		public void Dispose()
		{
			_client.Reset();
		}
	}
}
