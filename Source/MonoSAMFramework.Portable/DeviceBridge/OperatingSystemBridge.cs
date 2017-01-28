namespace MonoSAMFramework.Portable.DeviceBridge
{
	public interface IOperatingSystemBridge
	{
		string FullDeviceInfoString { get; }
		string DeviceName { get; }
		string DeviceVersion { get; }

		FileHelper FileHelper { get; }
	}
}
