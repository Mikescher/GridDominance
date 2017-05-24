namespace MonoSAMFramework.Portable.DeviceBridge
{
	public class DummyDeviceBridge : IOperatingSystemBridge
	{
		public string FullDeviceInfoString { get; } = "";
		public string DeviceName { get; } = "";
		public string DeviceVersion { get; } = "";
		public string ScreenResolution { get; } = "";
		public FileHelper FileHelper { get; } = new DummyFileHelper();
		public string DoSHA256(string input) => "";
		public void ExitApp() {}
	}

	public class DummyFileHelper : FileHelper
	{
		public override void WriteData(string fileid, string data)
		{
			//
		}

		public override string ReadDataOrNull(string fileid)
		{
			return null;
		}
	}
}
