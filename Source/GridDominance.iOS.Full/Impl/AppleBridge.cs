using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Foundation;
using GridDominance.Generic.Impl;
using GridDominance.iOS.Impl;
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

		public string EnvironmentStackTrace => Environment.StackTrace;

		public IUDPClient CreateUPDClient() => new XamarinUDPClient();

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
				new NSUrl(@"https://apple.com/appstore") //TODO correct app link
			};

			var activityController = new UIActivityViewController(activitiesItems, null);

			var gameController = Program.game.Services.GetService(typeof(UIViewController)) as UIViewController;

			if (gameController != null) gameController.PresentViewController(activityController, true, null);
		}
	}
}