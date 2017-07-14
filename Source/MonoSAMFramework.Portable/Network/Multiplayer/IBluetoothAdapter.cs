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

		void StartAdapter();

		void StartScan();
		void EnsureDiscoverable(int seconds);
		Tuple<byte[], int> RecieveOrNull();
		void Write(byte[] data);
	}
}
