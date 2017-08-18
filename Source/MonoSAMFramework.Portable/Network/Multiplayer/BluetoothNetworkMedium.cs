using System.Collections.Generic;
using System.Linq;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.LogProtocol;

namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	public class BluetoothNetworkMedium : INetworkMedium
	{
		private readonly IBluetoothAdapter _client;
		private BluetoothAdapterState _lastState = BluetoothAdapterState.Created;

		public BufferedLambdaString DebugDisplayString { get; }

		public bool IsP2PConnected => _client.State == BluetoothAdapterState.Connected;
		public bool IsP2PListening => _client.State == BluetoothAdapterState.Listen;

		private bool _isScanning = false;
		private float _scanStartTime = 0f;

		private IBluetoothDevice _currentConnDevice;
		private List<IBluetoothDevice> _lastScanDevices = new List<IBluetoothDevice>();

		public readonly Queue<BluetoothMediumEvent> Events = new Queue<BluetoothMediumEvent>();

		public BluetoothNetworkMedium()
		{
			_client = MonoSAMGame.CurrentInst.Bridge.Bluetooth;

			DebugDisplayString = new BufferedLambdaString(() => $"XBT[Enabled:{_client.IsEnabled} Discovering:{_client.IsDiscovering} MyState:{_client.State} (_scanning:{_isScanning}|{MonoSAMGame.CurrentTime.TotalElapsedSeconds - _scanStartTime:00}) AState:{_client.AdapterState} AScan:{_client.AdapterScanMode} Threads:<{_client.DebugThreadState}> Founds:{_client.FoundDevices.Count}\n" + 
			                                                    $"RemoteDevice:<{_client.RemoteDevice?.Name}|{_client.RemoteDevice?.Address}|{_client.RemoteDevice?.DeviceClass}|{_client.RemoteDevice?.Type}|{_client.RemoteDevice?.IsBonded}|{_client.RemoteDevice?.IsBonding}> Name:{_client.AdapterName}]", 1.5f);
		}

		public void Init(out SAMNetworkConnection.ErrorType error)
		{
			_client.StartAdapter();

			_lastState = _client.State;
			_currentConnDevice = null;
			_lastScanDevices.Clear();
			Events.Clear();
			_isScanning = false;
			_scanStartTime = MonoSAMGame.CurrentTime.TotalElapsedSeconds;

			if (_client.State == BluetoothAdapterState.AdapterNotFound)
			{
				error = SAMNetworkConnection.ErrorType.BluetoothAdapterNotFound;
			}
			else if (_client.State == BluetoothAdapterState.AdapterNotFound)
			{
				error = SAMNetworkConnection.ErrorType.BluetoothAdapterNoPermission;
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
			var xlastState = _lastState;
			_lastState = _client.State;

			if (_client.State == BluetoothAdapterState.AdapterNotFound)
			{
				error = SAMNetworkConnection.ErrorType.BluetoothAdapterNotFound;
				return;
			}

			if (_client.State == BluetoothAdapterState.PermissionNotGranted)
			{
				error = SAMNetworkConnection.ErrorType.BluetoothAdapterNoPermission;
				return;
			}

			if (_client.State == BluetoothAdapterState.Error)
			{
				error = SAMNetworkConnection.ErrorType.BluetoothInternalError;
				return;
			}

			if (_client.State == BluetoothAdapterState.NotEnabledByUser)
			{
				error = SAMNetworkConnection.ErrorType.BluetoothNotEnabled;
				return;
			}

			if (_client.State == BluetoothAdapterState.ConnectionFailed)
			{
				if (_lastScanDevices.Any())
				{
					if (_currentConnDevice != null)
					{
						SAMLog.Warning("BNM::ConnectionFailed", $"Connection to {_currentConnDevice.Name} failed");

						Events.Enqueue(new BluetoothMediumEvent(BluetoothMediumEvent.BTEvent.ConnectionFailed, _currentConnDevice.Name));
					}

					_currentConnDevice = _lastScanDevices.First();
					_lastScanDevices.Remove(_currentConnDevice);

					_isScanning = false;

					SAMLog.Debug($"Try connect to {_currentConnDevice.Name} ({_currentConnDevice.Address}|{_currentConnDevice.DeviceClass}|{_currentConnDevice.Type})");
					Events.Enqueue(new BluetoothMediumEvent(BluetoothMediumEvent.BTEvent.TryConnection, _currentConnDevice.Name));
					_client.Connect(_currentConnDevice);

					error = SAMNetworkConnection.ErrorType.None;
				}
				else
				{
					_isScanning = true;
					_client.StartScan();
					_scanStartTime = MonoSAMGame.CurrentTime.TotalElapsedSeconds;

					error = SAMNetworkConnection.ErrorType.None;
				}
				return;
			}

			if (_client.State == BluetoothAdapterState.ConnectionLost)
			{
				error = SAMNetworkConnection.ErrorType.P2PConnectionLost;
				return;
			}

			if (_client.State == BluetoothAdapterState.Active)
			{
				if (_isScanning)
				{
					_lastScanDevices = _client.FoundDevices;

					SAMLog.Debug($"Scanning finished {_lastScanDevices.Count} devices found");

					if (_lastScanDevices.Any())
					{
						foreach (var dd in _lastScanDevices) SAMLog.Debug($"Device found: {dd.Name} ({dd.Address}|{dd.DeviceClass}|{dd.Type})");

						_currentConnDevice = _lastScanDevices.First();
						_lastScanDevices.Remove(_currentConnDevice);

						SAMLog.Debug($"Try connect to {_currentConnDevice.Name} ({_currentConnDevice.Address}|{_currentConnDevice.DeviceClass}|{_currentConnDevice.Type})");

						_isScanning = false;
						_client.Connect(_currentConnDevice);
					}
					else
					{
						_isScanning = true;
						_client.StartScan();
						_scanStartTime = MonoSAMGame.CurrentTime.TotalElapsedSeconds;
					}
				}
			}

			if (_client.State == BluetoothAdapterState.Scanning)
			{
				if (_isScanning && !_client.IsDiscovering && MonoSAMGame.CurrentTime.TotalElapsedSeconds - _scanStartTime > 30)
				{
					error = SAMNetworkConnection.ErrorType.None;
					SAMLog.Warning("BNM::ForceScanCancel", "Cancel scan by force");
					_client.CancelScan();
					return;
				}
			}

			if (_client.State == BluetoothAdapterState.Listen)
			{
				error = SAMNetworkConnection.ErrorType.None;
				if (!_client.IsDiscoverable)
				{
					_client.ContinueWaiting();
				}
				return;
			}

			if (_client.State == BluetoothAdapterState.Connected)
			{
				if (xlastState == BluetoothAdapterState.Connecting && _currentConnDevice != null)
				{
					Events.Enqueue(new BluetoothMediumEvent(BluetoothMediumEvent.BTEvent.ConnectionSucceeded, _currentConnDevice.Name));
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
				_scanStartTime = MonoSAMGame.CurrentTime.TotalElapsedSeconds;
			}
		}

		public byte[] RecieveOrNull()
		{
			return _client.RecieveOrNull();
		}

		public void Send(byte[] data)
		{
			if (data.Length == 0) return;
			
			if (_client.State != BluetoothAdapterState.Connected)
			{
				SAMLog.Warning("BNM::Send_ERR", $"Trying to send data {data[0]} while in State {_client.State}");
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
