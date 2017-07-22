using System.Collections.Generic;
using System.Linq;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.LogProtocol;

namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	public class BluetoothNetworkMedium : INetworkMedium
	{
		private readonly IBluetoothAdapter _client;

		public string DebugDisplayString => $"XBT[Enabled:{_client.IsEnabled} Discovering:{_client.IsDiscovering} State:{_client.State} AState:{_client.AdapterState} AScan:{_client.AdapterScanMode} RemoteDevice:<{_client.RemoteDevice?.Name}|{_client.RemoteDevice?.Address}|{_client.RemoteDevice?.DeviceClass}|{_client.RemoteDevice?.Type}|{_client.RemoteDevice?.IsBonded}|{_client.RemoteDevice?.IsBonding}> Threads:<{_client.DebugThreadState}> Founds:{_client.FoundDevices.Count} Name:{_client.AdapterName}]";

		public bool IsP2PConnected => _client.State == BluetoothAdapterState.Connected;
		public bool IsP2PListening => _client.State == BluetoothAdapterState.Listen;

		private bool _isScanning = false;

		private IBluetoothDevice _currentConnDevice;
		private List<IBluetoothDevice> _lastScanDevices = new List<IBluetoothDevice>();

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
			else if (_client.State == BluetoothAdapterState.AdapterNotFound)
			{
				error = SAMNetworkConnection.ErrorType.BluetoothAdapterNotPermission;
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
			else if (_client.State == BluetoothAdapterState.PermissionNotGranted)
			{
				error = SAMNetworkConnection.ErrorType.BluetoothAdapterNoPermission;
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
				if (_lastScanDevices.Any())
				{
					if (_currentConnDevice != null)
					{
						//TODO Show Toast [Connection to {0} failed]
					}

					_currentConnDevice = _lastScanDevices[FloatMath.GetRangedIntRandom(_lastScanDevices.Count)];
					_lastScanDevices.Remove(_currentConnDevice);

					_isScanning = false;
					_client.Connect(_currentConnDevice);

					error = SAMNetworkConnection.ErrorType.None;
				}
				else
				{
					_isScanning = true;
					_client.StartScan();

					error = SAMNetworkConnection.ErrorType.None;
				}
				return;
			}
			else if (_client.State == BluetoothAdapterState.ConnectionLost)
			{
				error = SAMNetworkConnection.ErrorType.P2PConnectionLost;
				return;
			}

			if (_isScanning && _client.State == BluetoothAdapterState.Active)
			{
				_lastScanDevices = _client.FoundDevices;

				SAMLog.Debug($"Scanning finished {_lastScanDevices.Count} devices found");

				if (_lastScanDevices.Any())
				{
					foreach (var dd in _lastScanDevices) SAMLog.Debug($"Device found: {dd.Name} ({dd.Address}|{dd.DeviceClass}|{dd.Type})");

					_currentConnDevice = _lastScanDevices[FloatMath.GetRangedIntRandom(_lastScanDevices.Count)];
					_lastScanDevices.Remove(_currentConnDevice);

					SAMLog.Debug($"Try connect to {_currentConnDevice.Name} ({_currentConnDevice.Address}|{_currentConnDevice.DeviceClass}|{_currentConnDevice.Type})");

					_isScanning = false;
					_client.Connect(_currentConnDevice);
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
