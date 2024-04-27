using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using GridDominance.Shared.Resources;
using Java.Util;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.LogProtocol;
using Exception = Java.Lang.Exception;

namespace GridDominance.Android.Impl
{
	/// <summary>
	/// https://developer.android.com/guide/topics/connectivity/bluetooth.html
	/// </summary>
	class XamarinBluetooth : IBluetoothAdapter
	{
		private const int REQUEST_ENABLE_BT    = 2;
		private const int REQUEST_ENABLE_PERM  = 3;

		public const string NAME = GDConstants.BLUETOOTH_NAME;
		public static readonly UUID UUID = UUID.FromString(GDConstants.BLUETOOTH_UUID);

		public BluetoothAdapterState State { get; set; } = BluetoothAdapterState.Created;
		public IBluetoothDevice RemoteDevice { get; set; }
		public List<IBluetoothDevice> FoundDevices { get { lock(_foundDevices) { return _foundDevices.ToList(); } } }
		public string AdapterState => Adapter.State.ToString();
		public string AdapterScanMode => Adapter.ScanMode.ToString();
		public bool IsEnabled => Adapter.IsEnabled;
		public string AdapterName => Adapter.Name;
		public bool IsDiscovering => Adapter.IsDiscovering;
		public bool IsDiscoverable => Adapter.ScanMode == ScanMode.ConnectableDiscoverable;

		public string DebugThreadState => $"Acc: {_acceptThread?.IsAlive ?? false}; Conn: {_connectThread?.IsAlive ?? false}; Trans: {_transferOutThread?.IsAlive ?? false}+{_transferInThread?.IsAlive ?? false}";


		private readonly MainActivity _activity;
		public readonly BluetoothAdapter Adapter;

		private readonly ConcurrentQueue<byte[]> _messageQueue = new ConcurrentQueue<byte[]>();
		private readonly List<IBluetoothDevice> _foundDevices = new List<IBluetoothDevice>();

		private BTAcceptThread _acceptThread;      // SERVER
		private BTConnectThread _connectThread;    // CLIENT
		private BTTransferSendThread _transferOutThread;
		private BTTransferRecieveThread _transferInThread;

		private readonly BroadcastReceiver _reciever;

		public XamarinBluetooth(MainActivity a)
		{
			Adapter = BluetoothAdapter.DefaultAdapter;
			_activity = a;

			_reciever = new BTScanReciever(this);
			_activity.RegisterReceiver(_reciever, new IntentFilter(BluetoothDevice.ActionFound));
			_activity.RegisterReceiver(_reciever, new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished));
#if DEBUG
			//_activity.RegisterReceiver(_reciever, new IntentFilter(BluetoothAdapter.ActionRequestDiscoverable), ActivityFlags.);
			//_activity.RegisterReceiver(_reciever, new IntentFilter(BluetoothAdapter.ActionRequestEnable));
			//_activity.RegisterReceiver(_reciever, new IntentFilter(BluetoothAdapter.ActionStateChanged));
			//_activity.RegisterReceiver(_reciever, new IntentFilter(BluetoothAdapter.ActionDiscoveryStarted));
#endif

			if (Adapter == null) State = BluetoothAdapterState.AdapterNotFound;
		}

		public void StartAdapter()
		{
			CancelAllThreads();
			lock (_foundDevices) { _foundDevices.Clear(); }
			if (Adapter.IsDiscovering) Adapter.CancelDiscovery();
			State = BluetoothAdapterState.Created;

			var missingPermissions = new List<string>();
			if (_activity.CheckCallingOrSelfPermission(Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
				missingPermissions.Add(Manifest.Permission.AccessCoarseLocation);
			if (_activity.CheckCallingOrSelfPermission(Manifest.Permission.AccessFineLocation) != Permission.Granted)
				missingPermissions.Add(Manifest.Permission.AccessFineLocation);
			if (_activity.CheckCallingOrSelfPermission(Manifest.Permission.Bluetooth) != Permission.Granted)
				missingPermissions.Add(Manifest.Permission.Bluetooth);
			if (_activity.CheckCallingOrSelfPermission(Manifest.Permission.BluetoothAdmin) != Permission.Granted)
				missingPermissions.Add(Manifest.Permission.BluetoothAdmin);

			if (missingPermissions.Any())
			{
				SAMLog.Warning("ABTA::MissingPerms", string.Join("|", missingPermissions.Select(p => p.Split('.').Last())));

				if ((int)Build.VERSION.SdkInt >= 23)
				{
					// With API>23 I can request them here
					// https://blog.xamarin.com/requesting-runtime-permissions-in-android-marshmallow/
					_activity.RequestPermissions(missingPermissions.ToArray(), REQUEST_ENABLE_PERM);
				}

				State = BluetoothAdapterState.PermissionNotGranted;
				return;
			}

			if (Adapter.IsEnabled)
			{
				State = BluetoothAdapterState.Active;
			}
			else
			{
				State = BluetoothAdapterState.RequestingEnable;

				Intent enableIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
				_activity.StartActivityForResult(enableIntent, REQUEST_ENABLE_BT);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void StartWaiting()
		{
			CancelAllThreads();

			EnsureDiscoverable(120);

			try
			{
				_acceptThread = new BTAcceptThread(this);
				_acceptThread.Start();
			}
			catch (Exception e)
			{
				SAMLog.Error("ABTA::CantCreateAcceptThread", e);
				State = BluetoothAdapterState.Error;
				CancelAllThreads();
				return;
			}

			State = BluetoothAdapterState.Listen;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void ContinueWaiting()
		{
			EnsureDiscoverable(600);

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

			SAMLog.Debug($"StartDiscovery (1)");

			if (Adapter.IsDiscovering) Adapter.CancelDiscovery();
			bool s = Adapter.StartDiscovery();

			SAMLog.Debug($"StartDiscovery (2 => {s})");


			lock (_foundDevices)
			{
				_foundDevices.Clear();
			}
		}

		public void CancelScan()
		{
			if (Adapter.IsDiscovering) Adapter.CancelDiscovery();

			State = BluetoothAdapterState.Active;
			SAMLog.Debug($"CancelScan");
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

		public byte[] RecieveOrNull()
		{
			byte[] d;
			if (_messageQueue.TryDequeue(out d)) return d;
			return null;
		}

		public void Write(byte[] data)
		{
			_transferOutThread.Write(data);
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

				if (_transferOutThread != null)
				{
					_transferOutThread.Cancel();
					_transferOutThread = null;
				}

				if (_transferInThread != null)
				{
					_transferInThread.Cancel();
					_transferInThread = null;
				}

				if (_acceptThread != null)
				{
					_acceptThread.Cancel();
					_acceptThread = null;
				}

				RemoteDevice = null;
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

			try
			{
				_connectThread = new BTConnectThread(device, this);
				_connectThread.Start();
			}
			catch (Exception e)
			{
				SAMLog.Error("ABTA::CantCreateConnectThread", e);
				State = BluetoothAdapterState.Error;
				CancelAllThreads();
				return;
			}

			State = BluetoothAdapterState.Connecting;
		}

		public void ThreadMessage_DataRead(byte[] data, int offset, int len)
		{
			_messageQueue.Enqueue(data.Skip(offset).Take(len).ToArray());
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void ThreadMessage_Connected(BluetoothSocket socket, BluetoothDevice device)
		{
			CancelAllThreads();

			try
			{
				_transferOutThread = new BTTransferSendThread(socket, this);
				_transferInThread = new BTTransferRecieveThread(socket, this);

				_transferOutThread.Start();
				_transferInThread.Start();
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

		public void ThreadMessage_ConnectionError()
		{
			CancelAllThreads();

			State = BluetoothAdapterState.Error;
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
				if (_foundDevices.All(fd => fd.Address != device.Address)) _foundDevices.Add(new BTDeviceWrapper(device));
			}
		}

		public void ThreadMessage_DiscoveryFinished()
		{
			lock (_foundDevices)
			{
				State = BluetoothAdapterState.Active;

				_foundDevices.Sort(new BTDeviceScoreComparer());

				SAMLog.Debug($"DiscoveryResult: [{string.Join(", ", _foundDevices.Select(d => d.Name))}]");
			}
		}

		public void OnDestroy()
		{
			CancelAllThreads();

			_activity.UnregisterReceiver(_reciever);
		}

		public void Reset()
		{
			CancelAllThreads();
			if (Adapter != null && Adapter.IsDiscovering) Adapter.CancelDiscovery();
			State = BluetoothAdapterState.Created;
		}

		public void Update()
		{
			//
		}
	}
}