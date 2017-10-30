using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Network.Multiplayer;

namespace MonoSAMFramework.Portable.DeviceBridge
{
	public interface IGDOperatingSystemBridge : ISAMOperatingSystemBridge
	{
		string FullDeviceInfoString { get; }
		string DeviceName { get; }
		string DeviceVersion { get; }
		string AppType { get; }
		FSize DeviceResolution { get; }

		IBillingAdapter IAB { get; }

		IBluetoothAdapter BluetoothFull { get; }
		IUDPClient CreateUPDClient();

		void OpenURL(string url);
		void ShareAppLink();
	}
}
