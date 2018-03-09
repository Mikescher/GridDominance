using System.Collections.Generic;

namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	public enum BluetoothAdapterState
	{
		Created,

		AdapterNotFound,
		PermissionNotGranted,

		RequestingEnable,

		NotEnabledByUser,

		Scanning,

		Active,
		Listen,
		Connecting,
		Connected,

		ConnectionLost,
		ConnectionFailed,

		Error,
	}

	public interface IBluetoothAdapter
	{
		BluetoothAdapterState State { get; }
		IBluetoothDevice RemoteDevice { get; }
		List<IBluetoothDevice> FoundDevices { get; }
		string DebugThreadState { get; }
		string AdapterState { get; }
		string AdapterScanMode { get; }
		string AdapterName { get; }
		bool IsEnabled { get; }
		bool IsDiscovering { get; }
		bool IsDiscoverable { get; }

		void StartAdapter();

		void StartScan();
		void CancelScan();

		void StartWaiting();
		void ContinueWaiting();

		void Connect(IBluetoothDevice d);

		byte[] RecieveOrNull();
		void Write(byte[] data);

		void Reset();

		void Update();
	}
}
