using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.DeviceBridge
{
	public interface ISAMOperatingSystemBridge
	{
		string FullDeviceInfoString { get; }
		string DeviceName { get; }
		string DeviceVersion { get; }
		FSize DeviceResolution { get; }

		string EnvironmentStackTrace { get; }

		FileHelper FileHelper { get; }

		string DoSHA256(string input);
		void Sleep(int milsec);
		void ExitApp();
	}
}
