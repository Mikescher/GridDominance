using System;
using System.Collections.Generic;

namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	public enum BluetoothAdapterState
	{
		AdapterNotFound,
		Created,

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

		void StartAdapter();

		void StartScan();
		void StartWaiting();
		void Connect(IBluetoothDevice d);

		Tuple<byte[], int> RecieveOrNull();
		void Write(byte[] data);

		void Reset();
	}
}
