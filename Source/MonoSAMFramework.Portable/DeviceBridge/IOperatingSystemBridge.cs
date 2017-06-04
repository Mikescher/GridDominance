namespace MonoSAMFramework.Portable.DeviceBridge
{
	public interface IOperatingSystemBridge
	{
		string FullDeviceInfoString { get; }
		string DeviceName { get; }
		string DeviceVersion { get; }
		string ScreenResolution { get; }

		FileHelper FileHelper { get; }
		IBillingAdapter IAB { get; }

		string DoSHA256(string input);
		void OpenURL(string url);
		void Sleep(int milsec);

		void ExitApp();
	}
}
