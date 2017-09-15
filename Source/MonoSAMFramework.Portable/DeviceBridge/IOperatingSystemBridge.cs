using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Network.Multiplayer;

namespace MonoSAMFramework.Portable.DeviceBridge
{
	public interface IOperatingSystemBridge
	{
		string FullDeviceInfoString { get; }
		string DeviceName { get; }
		string DeviceVersion { get; }
		FSize DeviceResolution { get; }

		FileHelper FileHelper { get; }
		IBillingAdapter IAB { get; }
		IBluetoothAdapter Bluetooth { get; }
		string EnvironmentStackTrace { get; }

		IUDPClient CreateUPDClient();

		string DoSHA256(string input);
		void OpenURL(string url);
		void Sleep(int milsec);

		void ExitApp();

		void ShareAppLink();
	}
}
