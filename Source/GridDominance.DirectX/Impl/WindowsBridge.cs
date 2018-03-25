using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.Language;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using GridDominance.Generic.Impl;
using GridDominance.Shared.DeviceBridge;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Network.Multiplayer;

// ReSharper disable once CheckNamespace
namespace GridDominance.Windows
{
	class WindowsBridge : IGDOperatingSystemBridge
	{
		public FileHelper FileHelper { get; } = new WindowsFileHelper();
		public IBillingAdapter IAB { get; } = new WindowsEmulatingBillingAdapter();
		public IBluetoothAdapter BluetoothFull { get; } = null; // Not Supported
		public IBluetoothAdapter BluetoothLE { get; }   = null; // Not Supported
		public IUDPClient CreateUPDClient() => new XamarinUDPClient();
		public string AppType => "Windows.DirectX";
		public SAMSystemType SystemType => SAMSystemType.MONOGAME_DESKTOP;
		public GDFlavor Flavor => GDFlavor.IAB;

		public FSize DeviceResolution { get; } = new FSize(0, 0);

		public string FullDeviceInfoString { get; } = "?? GridDominance.Windows.WindowsImpl ??" + "\n" + Environment.MachineName + "/" + Environment.UserName;
		public string DeviceName { get; } = "PC";
		public string DeviceVersion { get; } = Environment.OSVersion.VersionString;
		public string EnvironmentStackTrace => Environment.StackTrace;

		public void OnNativeInitialize(MonoSAMGame game)
		{
			// NOP
		}

		public string DoSHA256(string input)
		{
			using (var sha256 = SHA256.Create()) return ByteUtils.ByteToHexBitFiddle(sha256.ComputeHash(Encoding.UTF8.GetBytes(input)));
		}

		public byte[] DoSHA256(byte[] input)
		{
			using (var sha256 = SHA256.Create()) return sha256.ComputeHash(input);
		}

		public void OpenURL(string url) => Process.Start(url);
		public void Sleep(int milsec) => Thread.Sleep(milsec);

		public void ExitApp() { Environment.Exit(0); }

		public void ShareAppLink() { /* Not implemented */ }
	}
}