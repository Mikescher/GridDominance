using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MonoSAMFramework.Portable.Network.Multiplayer;
using GridDominance.Shared.Resources;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using MonoSAMFramework.Portable.LogProtocol;
using System.Collections.Concurrent;
using MonoSAMFramework.Portable.Language;
using System.Security.Cryptography;

namespace GridDominance.Android.Impl
{
	class BLEBluetoothAdapter : IBluetoothAdapter
	{
		public const string NAME = GDConstants.BLUETOOTH_NAME;
		public static readonly Guid SERVICE_UUID = Guid.Parse(GDConstants.BLUETOOTH_LE_SERVICE_UUID);
		public static readonly Guid CHARACTER_UUID = Guid.Parse(GDConstants.BLUETOOTH_LE_CHRCTR_UUID);

		public BluetoothAdapterState State { get; set; } = BluetoothAdapterState.Created;
		public IBluetoothDevice RemoteDevice { get; set; }

		public List<IBluetoothDevice> FoundDevices { get { lock (_foundDevices) { return _foundDevices.ToList(); } } }

		public string DebugThreadState => $"BT_LE";

		public string AdapterState => Convert.ToString(_ble.State);
		public string AdapterScanMode => Convert.ToString(_adapter.ScanMode);
		public string AdapterName => "?";

		public bool IsEnabled => _ble.IsOn;
		public bool IsDiscovering => _adapter.IsScanning;
		public bool IsDiscoverable => (State == BluetoothAdapterState.Listen);

		private readonly List<IBluetoothDevice> _foundDevices = new List<IBluetoothDevice>();

		private readonly IBluetoothLE _ble;
		private readonly IAdapter _adapter;

		private IDevice _connectedDevice;
		private IService _connectedService;
		private ICharacteristic _connectedCharactersistic;

		private byte[][] _dataRecieveBuffer = new byte[127][];

		private readonly ConcurrentQueue<byte[]> _dataToSend   = new ConcurrentQueue<byte[]>();
		private readonly ConcurrentQueue<byte[]> _dataRecieved = new ConcurrentQueue<byte[]>();

		public BLEBluetoothAdapter()
		{
			try
			{
				_ble = CrossBluetoothLE.Current;
				_adapter = CrossBluetoothLE.Current?.Adapter;

				if (_adapter != null)
				{
					_adapter.DeviceDiscovered += AdapterOnDeviceDiscovered;
					_adapter.DeviceConnectionLost += AdapterOnDeviceConnectionLost;
					_adapter.DeviceDisconnected += AdapterOnDeviceDisconnected;
					_adapter.DeviceConnected += AdapterOnDeviceConnected;
				}

				for (int i = 0; i < 127; i++)
				{
					_dataRecieveBuffer[i] = new byte[20];
				}
			}
			catch (Exception e)
			{
				SAMLog.Warning("ABA::CTRERR", e);

				_ble = null;
				_adapter = null;
			}
		}

		private void AdapterOnDeviceConnected(object sender, DeviceEventArgs e)
		{
			if (State != BluetoothAdapterState.Listen) return;

			try
			{
				_connectedService = e.Device.GetServiceAsync(SERVICE_UUID).Result;
				_connectedCharactersistic = _connectedService.GetCharacteristicAsync(CHARACTER_UUID).Result;

				_connectedCharactersistic.ValueUpdated += CharactersisticOnValueUpdated;

				RemoteDevice = new BLEDeviceWrapper(e.Device);
				State = BluetoothAdapterState.Connected;

				_connectedCharactersistic.StartUpdatesAsync();
				Task.Run(SendLoopTask);
			}
			catch (Exception ex)
			{
				SAMLog.Warning("ABA::DEVICECONN", ex);
			}
		}

		private void AdapterOnDeviceDisconnected(object sender, DeviceEventArgs e)
		{
			ResetInternal();

			State = BluetoothAdapterState.ConnectionLost;
		}

		private void AdapterOnDeviceConnectionLost(object sender, DeviceErrorEventArgs e)
		{
			ResetInternal();

			State = BluetoothAdapterState.ConnectionLost;
		}

		private void AdapterOnDeviceDiscovered(object sender, DeviceEventArgs args)
		{
			lock (_foundDevices) { _foundDevices.Add(new BLEDeviceWrapper(args.Device)); }
		}

		public void StartAdapter()
		{
			lock (_foundDevices) { _foundDevices.Clear(); }

			if (_ble == null || _adapter == null || !_ble.IsAvailable)
			{
				State = BluetoothAdapterState.AdapterNotFound;
				return;
			}

			State = BluetoothAdapterState.Created;

			if (_ble.IsOn)
			{
				State = BluetoothAdapterState.Active;
				return;
			}
			else
			{
				State = BluetoothAdapterState.NotEnabledByUser;
				return;
			}
		}

		public void StartScan()
		{
			Task.Run(TaskStartScan);
		}

		private async Task TaskStartScan()
		{
			try
			{
				await _adapter.StopScanningForDevicesAsync();

				_adapter.ScanMode = ScanMode.LowLatency;
				lock (_foundDevices) { _foundDevices.Clear(); }

				await _adapter.StartScanningForDevicesAsync();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		public void CancelScan()
		{
			_adapter.StopScanningForDevicesAsync().RunSynchronously();
		}

		public void StartWaiting()
		{
			State = BluetoothAdapterState.Listen;
		}

		public void ContinueWaiting()
		{
			State = BluetoothAdapterState.Listen;
		}

		public void Connect(IBluetoothDevice d)
		{
			Task.Run(() => TaskConnect(d));
		}

		private async Task TaskConnect(IBluetoothDevice d)
		{
			try
			{
				_connectedDevice = ((BLEDeviceWrapper)d).InternalDevice;

				await _adapter.StopScanningForDevicesAsync();
				State = BluetoothAdapterState.Connecting;

				await _adapter.ConnectToDeviceAsync(((BLEDeviceWrapper)d).InternalDevice);
				_connectedService = await _connectedDevice.GetServiceAsync(SERVICE_UUID);
				_connectedCharactersistic = await _connectedService.GetCharacteristicAsync(CHARACTER_UUID);

				_connectedCharactersistic.ValueUpdated += CharactersisticOnValueUpdated;

				RemoteDevice = ((BTDeviceWrapper)d);
				State = BluetoothAdapterState.Connected;

				await _connectedCharactersistic.StartUpdatesAsync();
				await Task.Run(SendLoopTask);
			}
			catch (DeviceConnectionException e)
			{
				SAMLog.Warning("ABA::", e);
				State = BluetoothAdapterState.ConnectionFailed;
			}
		}

		private async Task SendLoopTask()
		{
			while (_connectedDevice != null && State == BluetoothAdapterState.Connected)
			{
				try
				{
					byte[] data;
					if (_dataToSend.TryDequeue(out data))
					{
						ushort datalen = (ushort)data.Length;
						byte cs1;
						byte cs2;
						using (var sha1 = new SHA1CryptoServiceProvider())
						{
							var cs = sha1.ComputeHash(data);
							cs1 = cs[0];
							cs2 = cs[1];
						}

						byte packindex = 0;
						int cindex = 0;
						while (cindex < datalen)
						{
							var packlen = packindex == 0 ? 15 : 19;
							var lastpack = cindex + packlen < datalen;
							byte[] d = new byte[20];
							d[0] = (byte)(packindex | (lastpack ? 0b1000_0000 : 0b0000_0000));

							if (packindex == 0)
							{
								d[1] = cs1;
								d[2] = cs2;
								NetworkDataTools.SetUInt16(out d[3], out d[4], datalen);
								for (int i = 0; i < 15; i++) d[5 + i] = data[cindex + i];
							}
							else
							{
								for (int i = 0; i < 19; i++) d[1 + i] = data[cindex + i];
							}

							await _connectedCharactersistic.WriteAsync(d);

							cindex += packlen;
						}
					}
					else
					{
						await Task.Delay(10);
					}
				}
				catch (Exception e)
				{
					SAMLog.Error("ABA::SLT", e);
				}
			}
		}

		private void CharactersisticOnValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
		{
			try
			{
				var value = e.Characteristic.Value;
				if (value.Length < 1) return;

				var idx = value[0] & 0b0111_1111;
				var fin = value[0] & 0b1000_0000;

				_dataRecieveBuffer[idx] = value;

				if (fin != 0)
				{
					List<byte> barr = new List<byte>(SAMNetworkConnection.MAX_PACKAGE_SIZE_BYTES);

					for (int i = 0; i <= idx; i++)
					{
						if (i == 0)
							barr.AddRange(_dataRecieveBuffer[i].Skip(5));
						else
							barr.AddRange(_dataRecieveBuffer[i].Skip(1));
					}

					var checksum1 = _dataRecieveBuffer[0][1];
					var checksum2 = _dataRecieveBuffer[0][2];
					var datalen = NetworkDataTools.GetUInt16(_dataRecieveBuffer[0][3], _dataRecieveBuffer[0][4]);

					if (barr.Count < datalen)
					{
						SAMLog.Warning("ABA:NED", $"Not enough data: {barr.Count} < {datalen}\r\nData:\r\n{ByteUtils.CompressBytesForStorage(barr.ToArray())}");
						return;
					}

					var data = barr.Take(datalen).ToArray();
					byte cs1;
					byte cs2;
					using (var sha1 = new SHA1CryptoServiceProvider())
					{
						var cs = sha1.ComputeHash(data);
						cs1 = cs[0];
						cs2 = cs[1];
					}

					if (cs1 != checksum1 || cs2 != checksum2)
					{
						SAMLog.Warning("ABA:NED", $"Checksum fail ({cs1} <> {checksum1} || {cs2} <> {checksum2})\r\nData:\r\n{ByteUtils.CompressBytesForStorage(barr.ToArray())}");
						return;
					}

					_dataRecieved.Enqueue(data);
					return;
				}
			}
			catch (Exception ex)
			{
				SAMLog.Error("ABA:COVA", ex);
			}
		}

		public byte[] RecieveOrNull()
		{
			byte[] d;
			if (_dataRecieved.TryDequeue(out d)) return d;
			return null;
		}

		public void Write(byte[] data)
		{
			_dataToSend.Enqueue(data.ToArray());
		}

		private void ResetInternal()
		{
			try
			{
				byte[] tmp;
				while (_dataToSend.TryDequeue(out tmp)) ;
				while (_dataRecieved.TryDequeue(out tmp)) ;

				RemoteDevice = null;
				_connectedDevice = null;
				_connectedService = null;
				if (_connectedCharactersistic != null)
				{
					_connectedCharactersistic.ValueUpdated -= CharactersisticOnValueUpdated;
					_connectedCharactersistic.StopUpdatesAsync();
				}
				_connectedCharactersistic = null;

				if (_adapter != null && _adapter.IsScanning) CancelScan();
			}
			catch (Exception e)
			{
				SAMLog.Error("ABA::IRESET", e);
			}
		}

		public void Reset()
		{
			ResetInternal();

			State = BluetoothAdapterState.Created;
		}
	}
}