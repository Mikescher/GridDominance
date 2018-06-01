using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Foundation;
using GridDominance.Generic.Impl;
using GridDominance.Shared.DeviceBridge;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Language;
using MonoSAMFramework.Portable.LogProtocol;
using Plugin.DeviceInfo;
using UIKit;

namespace GridDominance.iOS.Impl
{
	class AppleBridge : IGDOperatingSystemBridge
	{
		public string FullDeviceInfoString => GenerateInfoStr();

		public string DeviceName { get; } = CrossDeviceInfo.Current.Model;
		public string DeviceVersion { get; } = "Apple iOS " + CrossDeviceInfo.Current.Version;
		public FSize DeviceResolution { get; } = new FSize((float)UIScreen.MainScreen.NativeBounds.Width, (float)UIScreen.MainScreen.NativeBounds.Height);
		public FileHelper FileHelper { get; } = new AppleFileHelper();
		public IBillingAdapter IAB { get; } = new AppleIABVersionBilling();
		public IBluetoothAdapter BluetoothFull { get; } = null; // Not supported
		public string AppType => "IOS.IAB";
		public SAMSystemType SystemType => SAMSystemType.MONOGAME_IOS;
		public GDFlavor Flavor => GDFlavor.IAB_NOMP;

		public FMargin DeviceSafeAreaInset { get; } = GetSafeArea();

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
            b.AppendFormat("UIScreen.NatBounds.Width  := '{0}'\n", UIScreen.MainScreen.NativeBounds.Width);
            b.AppendFormat("UIScreen.NatBounds.Height := '{0}'\n", UIScreen.MainScreen.NativeBounds.Height);
			b.AppendFormat("UIScreen.Brightness       := '{0}'\n", UIScreen.MainScreen.Brightness);
			b.AppendFormat("UIScreen.CurrentMode      := '{0}'\n", UIScreen.MainScreen.CurrentMode);
			b.AppendFormat("UIScreen.CoordinateSpace  := '{0}'\n", UIScreen.MainScreen.CoordinateSpace);
			b.AppendFormat("UIScreen.NativeScale      := '{0}'\n", UIScreen.MainScreen.NativeScale);
			b.AppendFormat("UIScreen.PreferredMode    := '{0}'\n", UIScreen.MainScreen.PreferredMode);
			b.AppendFormat("UIScreen.Scale            := '{0}'\n", UIScreen.MainScreen.Scale);
			b.AppendFormat("VersionType               := '{0}'\n", "IAB");

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
			SAMLog.Error("APPLBRIDG_IAB::EXIT", "Trying to exit app in iOS");
		}

		public void ShareAppLink()
		{
			NSObject[] activitiesItems = {
				new NSString("Cannon Conquest"),
				new NSUrl(@"https://itunes.apple.com/us/app/cannon-conquest/id1363746596")
			};

			var activityController = new UIActivityViewController(activitiesItems, null);

			if (Program.game.Services.GetService(typeof(UIViewController)) is UIViewController gameController) gameController.PresentViewController(activityController, true, null);
		}

		private static FMargin GetSafeArea()
		{
			var sz = new FSize((float)UIScreen.MainScreen.NativeBounds.Width, (float)UIScreen.MainScreen.NativeBounds.Height);

			var w = FloatMath.Min(FloatMath.Round(sz.Width), FloatMath.Round(sz.Height));
            var h = FloatMath.Max(FloatMath.Round(sz.Width), FloatMath.Round(sz.Height));

			if (w == 1125 && h == 2436) return new FMargin(16, 36, 16, 36);

			return FMargin.NONE;
		}
	}
}