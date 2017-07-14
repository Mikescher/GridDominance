using Android.Bluetooth;
using MonoSAMFramework.Portable.Network.Multiplayer;

namespace GridDominance.Android.Impl
{
	class BTDeviceWrapper : IBluetoothDevice
	{
		private readonly BluetoothDevice _device;
		public BluetoothDevice InternalDevice => _device;

		public BTDeviceWrapper(BluetoothDevice d)
		{
			_device = d;
		}

		public string Name => _device.Name;
		public string Address => _device.Address;
		public string DeviceClass => _device.BluetoothClass.DeviceClass.ToString();
		public string Type => _device.Type.ToString();
		public bool IsBonded => _device.BondState == Bond.Bonded;
		public bool IsBonding => _device.BondState == Bond.Bonding;

	}
}