using System.Linq;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.LogProtocol;

namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	public class BluetoothNetworkMedium : INetworkMedium
	{
		private readonly IBluetoothAdapter _client;

		public string DebugDisplayString => $"XBT[State:{_client.State} RemoteDevice:<{_client.RemoteDevice?.Name}|{_client.RemoteDevice?.Address}|{_client.RemoteDevice?.DeviceClass}|{_client.RemoteDevice?.Type}|{_client.RemoteDevice?.IsBonded}|{_client.RemoteDevice?.IsBonding}> Founds:{_client.FoundDevices.Count}]";

		public bool IsP2PConnected => _client.State == BluetoothAdapterState.Connected;
		public bool IsP2PListening => _client.State == BluetoothAdapterState.Listen;

		private bool _isScanning = false;

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

		public void Update(out SAMNetworkConnection.ErrorType error)
		{
			if (_client.State == BluetoothAdapterState.AdapterNotFound)
			{
				error = SAMNetworkConnection.ErrorType.BluetoothAdapterNotFound;
				return;
			}
			else if (_client.State == BluetoothAdapterState.Error)
			{
				error = SAMNetworkConnection.ErrorType.BluetoothInternalError;
				return;
			}
			else if (_client.State == BluetoothAdapterState.NotEnabledByUser)
			{
				error = SAMNetworkConnection.ErrorType.BluetoothNotEnabled;
				return;
			}
			else if (_client.State == BluetoothAdapterState.ConnectionFailed)
			{
				error = SAMNetworkConnection.ErrorType.P2PConnectionFailed;
				return;
			}
			else if (_client.State == BluetoothAdapterState.ConnectionLost)
			{
				error = SAMNetworkConnection.ErrorType.P2PConnectionLost;
				return;
			}

			if (_isScanning && _client.State == BluetoothAdapterState.Active)
			{
				var devices = _client.FoundDevices;
				if (devices.Any())
				{
					foreach (var dd in devices) SAMLog.Debug($"Device found: {dd.Name} ({dd.Address}|{dd.DeviceClass}|{dd.Type})");

					var d = devices[FloatMath.GetRangedIntRandom(devices.Count)];

					_isScanning = false;
					_client.Connect(d);
				}
				else
				{
					_isScanning = true;
					_client.StartScan();
				}
			}

			error = SAMNetworkConnection.ErrorType.None;
		}

		public void StartServer()
		{
			if (_client.State == BluetoothAdapterState.Active)
			{
				_client.StartWaiting();
			}
		}

		public void StartClient()
		{
			if (_client.State == BluetoothAdapterState.Active && !_isScanning)
			{
				_isScanning = true;
				_client.StartScan();
			}
		}

		public byte[] RecieveOrNull()
		{
			var d = _client.RecieveOrNull();
			if (d == null) return null;
			return d.Item1.Take(d.Item2).ToArray();
		}

		public void Send(byte[] data)
		{
			if (data.Length == 0) return;

			if (_client.State != BluetoothAdapterState.Connected)
			{
				SAMLog.Error("BNM::Send", $"Trying to send data {data[0]} while in State {_client.State}");
				return;
			}

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
