namespace MonoSAMFramework.Portable.DeviceBridge
{
	public interface ISAMOperatingSystemBridge
	{
		string EnvironmentStackTrace { get; }

		FileHelper FileHelper { get; }

		string DoSHA256(string input);
		void Sleep(int milsec);
		void ExitApp();
	}
}
