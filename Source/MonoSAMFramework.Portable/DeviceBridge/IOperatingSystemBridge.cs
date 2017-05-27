namespace MonoSAMFramework.Portable.DeviceBridge
{
	public interface IOperatingSystemBridge
	{
		string FullDeviceInfoString { get; }
		string DeviceName { get; }
		string DeviceVersion { get; }
		string ScreenResolution { get; }

		FileHelper FileHelper { get; }

		string DoSHA256(string input);
		void OpenURL(string url);

		void ExitApp();
	}
}
