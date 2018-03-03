using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Foundation;
using GridDominance.Generic.Impl;
using GridDominance.iOS.Impl;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Language;
using MonoSAMFramework.Portable.Network.Multiplayer;
using Plugin.DeviceInfo;
using UIKit;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.iOS.Full.Impl
{
	class AppleBridge : IGDOperatingSystemBridge
	{
		public string FullDeviceInfoString => GenerateInfoStr();

		public string DeviceName { get; } = CrossDeviceInfo.Current.Model;
		public string DeviceVersion { get; } = CrossDeviceInfo.Current.Version;
		public FSize DeviceResolution { get; } = new FSize((float)UIScreen.MainScreen.Bounds.Width, (float)UIScreen.MainScreen.Bounds.Height);
		public FileHelper FileHelper { get; } = new AppleFileHelper();
		public IBillingAdapter IAB { get; } = new AppleFullVersionBilling();
		public IBluetoothAdapter BluetoothFull { get; } = null; // Not supported
		public string AppType => "IOS.Full";
		public SAMSystemType SystemType => SAMSystemType.MONOGAME_IOS;
		public GDFlavor Flavor => GDFlavor.FULL_NOMP;

		public string EnvironmentStackTrace => Environment.StackTrace;

		public IUDPClient CreateUPDClient() => new XamarinUDPClient();

		public void OnNativeInitialize(MonoSAMGame game)
		{
			// NOP
		}

		private static string GenerateInfoStr()
		{
			StringBuilder b = new StringBuilder();

			b.AppendFormat("CrossDeviceInfo.Model     := '{0}'\n", CrossDeviceInfo.Current.Model);
			b.AppendFormat("CrossDeviceInfo.Version   := '{0}'\n", CrossDeviceInfo.Current.Version);
			b.AppendFormat("CrossDeviceInfo.Id        := '{0}'\n", CrossDeviceInfo.Current.Id);
			b.AppendFormat("CrossDeviceInfo.Idiom     := '{0}'\n", CrossDeviceInfo.Current.Idiom);
			b.AppendFormat("CrossDeviceInfo.Platform  := '{0}'\n", CrossDeviceInfo.Current.Platform);
			b.AppendFormat("UIScreen.Width            := '{0}'\n", UIScreen.MainScreen.Bounds.Width);
			b.AppendFormat("UIScreen.Height           := '{0}'\n", UIScreen.MainScreen.Bounds.Height);
			b.AppendFormat("UIScreen.Brightness       := '{0}'\n", UIScreen.MainScreen.Brightness);
			b.AppendFormat("UIScreen.CurrentMode      := '{0}'\n", UIScreen.MainScreen.CurrentMode);
			b.AppendFormat("UIScreen.CoordinateSpace  := '{0}'\n", UIScreen.MainScreen.CoordinateSpace);
			b.AppendFormat("UIScreen.NativeScale      := '{0}'\n", UIScreen.MainScreen.NativeScale);
			b.AppendFormat("UIScreen.PreferredMode    := '{0}'\n", UIScreen.MainScreen.PreferredMode);
			b.AppendFormat("UIScreen.Scale            := '{0}'\n", UIScreen.MainScreen.Scale);
			b.AppendFormat("VersionType               := '{0}'\n", "FULL");

			return b.ToString();
		}

		public string DoSHA256(string input)
		{
			using (var sha256 = SHA256.Create()) return ByteUtils.ByteToHexBitFiddle(sha256.ComputeHash(Encoding.UTF8.GetBytes(input)));
		}

		public byte[] DoSHA256(byte[] input)
		{
			using (var sha256 = SHA256.Create()) return sha256.ComputeHash(input);
		}

		public void OpenURL(string url)
		{
			UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
		}

		public void Sleep(int milsec)
		{
			Thread.Sleep(milsec);
		}

		public void ExitApp()
		{
			SAMLog.Error("APPLBRIDG::EXIT", "Trying to exit app in iOS");
		}

		public void ShareAppLink()
		{
			NSObject[] activitiesItems = {
				new NSString("Cannon Conquest"),
				new NSUrl(@"https://itunes.apple.com/de/app/cannon-conquest/id1303565192")
			};

			var activityController = new UIActivityViewController(activitiesItems, null);

			if (Program.game.Services.GetService(typeof(UIViewController)) is UIViewController gameController) gameController.PresentViewController(activityController, true, null);
		}
	}
}