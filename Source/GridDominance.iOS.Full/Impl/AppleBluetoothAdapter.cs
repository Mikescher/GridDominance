using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using GridDominance.Android.Impl;
using MonoSAMFramework.Portable.Network.Multiplayer;
using UIKit;
using GridDominance.Shared.Resources;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;

namespace GridDominance.iOS.Impl
{
	class AppleBluetoothAdapter : IBluetoothAdapter
	{
		public const string NAME = GDConstants.BLUETOOTH_NAME;
		public static readonly string UUID = GDConstants.BLUETOOTH_UUID;

		public BluetoothAdapterState State { get; set; } = BluetoothAdapterState.Created;
		public IBluetoothDevice RemoteDevice { get; set; }

		public List<IBluetoothDevice> FoundDevices { get { lock (_foundDevices) { return _foundDevices.ToList(); } } }

		public string DebugThreadState => $"BT";

		public string AdapterState => Convert.ToString(_ble.State);

		public string AdapterScanMode => Convert.ToString(_adapter.ScanMode);

		public string AdapterName => "?";

		public bool IsEnabled => _ble.IsOn;

		public bool IsDiscovering => _adapter.IsScanning;

		public bool IsDiscoverable => _ble.


		private readonly List<IBluetoothDevice> _foundDevices = new List<IBluetoothDevice>();
		private readonly IBluetoothLE _ble;
		private readonly IAdapter _adapter;

		public AppleBluetoothAdapter()
		{
			_ble = CrossBluetoothLE.Current;
			_adapter = CrossBluetoothLE.Current.Adapter;

			if (_adapter != null)
			{
				_adapter.DeviceDiscovered += AdapterOnDeviceDiscovered;
			}
		}

		private void AdapterOnDeviceDiscovered(object sender, DeviceEventArgs args)
		{
			lock (_foundDevices) { _foundDevices.Add(new BTDeviceWrapper(args.Device)); }
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
			Task.Run(TaskStartScyn);
		}

		private async Task TaskStartScyn()
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
			throw new NotImplementedException();
		}

		public void ContinueWaiting()
		{
			throw new NotImplementedException();
		}

		public void Connect(IBluetoothDevice d)
		{
			Task.Run(() => TaskConnect(d));
		}

		private async Task TaskConnect(IBluetoothDevice d)
		{
			try
			{
				await _adapter.StopScanningForDevicesAsync();
				State = BluetoothAdapterState.Connecting;
				await _adapter.ConnectToDeviceAsync(((BTDeviceWrapper)d).InternalDevice);



				State = BluetoothAdapterState.Connected;
			}
			catch (DeviceConnectionException e)
			{
				State = BluetoothAdapterState.ConnectionFailed;
			}
		}

		public byte[] RecieveOrNull()
		{
			throw new NotImplementedException();
		}

		public void Write(byte[] data)
		{
			throw new NotImplementedException();
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}
	}
}