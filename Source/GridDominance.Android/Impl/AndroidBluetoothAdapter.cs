using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using GridDominance.Android.Impl;
using Java.Util;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.Multiplayer;
using Exception = Java.Lang.Exception;

namespace GridDominance.Android
{
	class AndroidBluetoothAdapter : IBluetoothAdapter
	{
		private const int REQUEST_ENABLE_BT = 2;

		public const string NAME = "GridDominance.BluetoothSocket";
		public static readonly UUID UUID = UUID.FromString("4748a5f0-0ac7-44f7-be49-30f5fd2a08ed");

		public BluetoothAdapterState State { get; set; } = BluetoothAdapterState.Created;
		public IBluetoothDevice RemoteDevice { get; set; }
		public List<IBluetoothDevice> FoundDevices { get { lock(_foundDevices) { return _foundDevices.ToList(); } } }

		private readonly MainActivity _activity;
		public readonly BluetoothAdapter Adapter;

		private readonly ConcurrentQueue<Tuple<byte[], int>> _messageQueue = new ConcurrentQueue<Tuple<byte[], int>>();
		private readonly List<IBluetoothDevice> _foundDevices = new List<IBluetoothDevice>();

		private BTAcceptThread _acceptThread;
		private BTConnectThread _connectThread;
		private BTConnectedThread _connectedThread;
		private int _btstate;

		private BTScanReciever _scanReciever;

		public AndroidBluetoothAdapter(MainActivity a)
		{
			Adapter = BluetoothAdapter.DefaultAdapter;
			_activity = a;

			_scanReciever = new BTScanReciever(this);
			_activity.RegisterReceiver(_scanReciever, new IntentFilter(BluetoothDevice.ActionFound));
			_activity.RegisterReceiver(_scanReciever, new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished));

			if (Adapter == null) State = BluetoothAdapterState.AdapterNotFound;
		}

		public void StartAdapter()
		{
			CancelAllThreads();
			lock (_foundDevices) { _foundDevices.Clear(); }
			if (Adapter != null && Adapter.IsDiscovering) Adapter.CancelDiscovery();
			State = BluetoothAdapterState.Created;

			if (Adapter.IsEnabled)
			{
				State = BluetoothAdapterState.Active;
				StartInternal();
			}
			else
			{
				State = BluetoothAdapterState.RequestingEnable;

				Intent enableIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
				_activity.StartActivityForResult(enableIntent, REQUEST_ENABLE_BT);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void StartInternal()
		{
			CancelAllThreads();

			_acceptThread = new BTAcceptThread(this);
			_acceptThread.Start();

			State = BluetoothAdapterState.Listen;
		}

		public void HandleActivityResult(int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == REQUEST_ENABLE_BT)
			{
				if (resultCode == Result.Ok)
				{
					SAMLog.Debug("Bluetooth enabled");

					State = BluetoothAdapterState.Active;
					StartInternal();
				}
				else
				{
					SAMLog.Warning("ABTA::BNE", "Bluetooth [[not]] enabled");
					State = BluetoothAdapterState.NotEnabledByUser;
				}
			}
		}

		public void StartScan()
		{
			State = BluetoothAdapterState.Scanning;

			if (Adapter.IsDiscovering) Adapter.CancelDiscovery();
			Adapter.StartDiscovery();

			lock (_foundDevices)
			{
				_foundDevices.Clear();

				foreach (var device in Adapter.BondedDevices)
				{
					_foundDevices.Add(new BTDeviceWrapper(device));
				}
			}
		}

		public void EnsureDiscoverable(int sec)
		{
			if (Adapter.ScanMode != ScanMode.ConnectableDiscoverable)
			{
				Intent discoverableIntent = new Intent(BluetoothAdapter.ActionRequestDiscoverable);
				discoverableIntent.PutExtra(BluetoothAdapter.ExtraDiscoverableDuration, sec);
				_activity.StartActivity(discoverableIntent);
			}
		}

		public Tuple<byte[], int> RecieveOrNull()
		{
			Tuple<byte[], int> d;
			if (_messageQueue.TryDequeue(out d)) return d;
			return null;
		}

		public void Write(byte[] data)
		{
			_connectedThread.Write(data);
		}

		public void Connect(IBluetoothDevice d)
		{
			var ad = d as BTDeviceWrapper;
			if (ad != null) Connect(ad.InternalDevice);
		}

		private void CancelAllThreads()
		{
			try
			{
				if (_connectThread != null)
				{
					_connectThread.Cancel();
					_connectThread = null;
				}

				if (_connectedThread != null)
				{
					_connectedThread.Cancel();
					_connectedThread = null;
				}

				if (_acceptThread != null)
				{
					_acceptThread.Cancel();
					_acceptThread = null;
				}
			}
			catch (Exception e)
			{
				SAMLog.Error("ABTA::CAT", e);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Connect(BluetoothDevice device)
		{
			CancelAllThreads();

			_connectThread = new BTConnectThread(device, this);
			_connectThread.Start();

			State = BluetoothAdapterState.Connecting;
		}

		public void ThreadMessage_DataRead(byte[] data, int len)
		{
			_messageQueue.Enqueue(Tuple.Create(data, len));
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void ThreadMessage_Connected(BluetoothSocket socket, BluetoothDevice device)
		{
			CancelAllThreads();

			try
			{
				// Start the thread to manage the connection and perform transmissions
				_connectedThread = new BTConnectedThread(socket, this);
				_connectedThread.Start();
			}
			catch (Exception e)
			{
				SAMLog.Error("ABTA::CantCreateConnectedThread", e);
				State = BluetoothAdapterState.Error;
				CancelAllThreads();
				return;
			}

			RemoteDevice = new BTDeviceWrapper(device);

			State = BluetoothAdapterState.Connected;
		}
		
		public void ThreadMessage_ConnectionLost()
		{
			CancelAllThreads();

			State = BluetoothAdapterState.ConnectionLost;
		}

		public void ThreadMessage_ConnectionFailed()
		{
			CancelAllThreads();

			State = BluetoothAdapterState.ConnectionFailed;
		}

		public void SetConnectThreadCancelled()
		{
			_connectThread = null;
		}

		public void ThreadMessage_DeviceFound(BluetoothDevice device)
		{
			lock (_foundDevices)
			{
				_foundDevices.Add(new BTDeviceWrapper(device));
			}
		}

		public void ThreadMessage_DiscoveryFinished()
		{
			State = BluetoothAdapterState.Active;
		}
	}
}