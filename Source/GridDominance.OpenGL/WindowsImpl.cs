using MonoSAMFramework.Portable.DeviceBridge;
using System;

namespace GridDominance.OpenGL
{
	class WindowsImpl : IOperatingSystemBridge
	{
		public FileHelper FileHelper { get; } = new WindowsFileHelper();

		public string FullDeviceInfoString { get; } = "";
		public string DeviceName { get; } = "PC";
		public string DeviceVersion { get; } = Environment.OSVersion.VersionString;
	}
}
