using MonoSAMFramework.Portable.Network.Multiplayer;
using Plugin.BLE.Abstractions.Contracts;

namespace GridDominance.Android.Impl
{
	class BLEDeviceWrapper : IBluetoothDevice
	{
		private readonly IDevice _device;
		public IDevice InternalDevice => _device;

		public BLEDeviceWrapper(IDevice d)
		{
			_device = d;
		}

		public string Name => _device.Name;
		public string Address => _device.Id.ToString();
		public string DeviceClass => "generic";
		public string Type => "generic";
		public bool IsBonded => false;
		public bool IsBonding => false;

	}
}