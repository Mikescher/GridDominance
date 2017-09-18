using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.Language;
using System;
using System.Text;
using GridDominance.Generic.Impl;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Network.Multiplayer;
using GridDominance.Android.Impl;
using Windows.System;

// ReSharper disable once CheckNamespace
namespace GridDominance.WinPhone
{
	class WinPhoneBridge : IOperatingSystemBridge
	{
		public FileHelper FileHelper { get; } = new WinPhoneFileHelper();
		public IBillingAdapter IAB { get; } = new WinPhoneFullVersionBilling();
		public IBluetoothAdapter Bluetooth { get; } = null; // Not Supported
		public IUDPClient CreateUPDClient() => new XamarinUDPClient();
		public string AppType => "WinPhone.V8.Full";
		
		public FSize DeviceResolution { get; } = new FSize(0, 0);

		public string FullDeviceInfoString { get; } = GenerateInfoStr();
        public string DeviceName { get; } = DeviceStatus.DeviceName;
		public string DeviceVersion { get; } = Environment.OSVersion.Version.ToString();
        public string EnvironmentStackTrace => Environment.StackTrace;
        private static string GenerateInfoStr()
        {
            StringBuilder b = new StringBuilder();
            
            b.AppendFormat("VersionType         := '{0}'\n", "FULL_NOMP");

            return b.ToString();
        }

        public string DoSHA256(string input)
		{
			using (var sha256 = SHA256.Create()) return ByteUtils.ByteToHexBitFiddle(sha256.ComputeHash(Encoding.UTF8.GetBytes(input)));
		}

        public void OpenURL(string url) { Launcher.LaunchUriAsync(new Uri(url)); }
		public void Sleep(int milsec) => Thread.Sleep(milsec);

		public void ExitApp() { /* works autom by MonoGame */ }

		public void ShareAppLink() { /* Not implemented */ }
	}
}