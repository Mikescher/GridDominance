using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.Language;
using System;
using System.Text;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Network.Multiplayer;
using Windows.System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using GridDominance.UWP.Impl;

// ReSharper disable once CheckNamespace
namespace GridDominance.UWP
{
	class WinPhoneBridge : IOperatingSystemBridge
	{
		public FileHelper FileHelper { get; } = new WinPhoneFileHelper();
		public IBillingAdapter IAB { get; } = new WinPhoneFullVersionBilling();
		public IBluetoothAdapter Bluetooth { get; } = null; // Not Supported
		public IUDPClient CreateUPDClient() => null; // Not Supported

        public FSize DeviceResolution { get; } = GetScreenRes()

		public string FullDeviceInfoString { get; } = GenerateInfoStr();
        public string DeviceName { get; } = UWPDeviceInfo.DeviceModel;
		public string DeviceVersion { get; } = UWPDeviceInfo.SystemVersion;
        public string EnvironmentStackTrace => Environment.StackTrace;
        private static string GenerateInfoStr()
        {
            StringBuilder b = new StringBuilder();

            b.AppendFormat("DeviceModel         := '{0}'\n", UWPDeviceInfo.DeviceModel);
            b.AppendFormat("SystemVersion       := '{0}'\n", UWPDeviceInfo.SystemVersion);
            b.AppendFormat("DeviceManufacturer  := '{0}'\n", UWPDeviceInfo.DeviceManufacturer);
            b.AppendFormat("SystemArchitecture  := '{0}'\n", UWPDeviceInfo.SystemArchitecture);
            b.AppendFormat("TickCount           := '{0}'\n", Environment.TickCount);
            b.AppendFormat("VersionType         := '{0}'\n", "FULL_NOMP");

            return b.ToString();
        }

		public FSize GetScreenRes() 
		{
			var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
			var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
			var size = new Size(bounds.Width*scaleFactor, bounds.Height*scaleFactor);
		}

        public string DoSHA256(string input)
		{
			using (var sha256 = SHA256.Create()) return ByteUtils.ByteToHexBitFiddle(sha256.ComputeHash(Encoding.UTF8.GetBytes(input)));
		}

        public void OpenURL(string url) { Launcher.LaunchUriAsync(new Uri(url)); }
		public void Sleep(int milsec) => Task.Delay(milsec).Wait();

		public void ExitApp() { /* works autom by MonoGame */ }

		public void ShareAppLink() { /* Not implemented */ }
	}
}