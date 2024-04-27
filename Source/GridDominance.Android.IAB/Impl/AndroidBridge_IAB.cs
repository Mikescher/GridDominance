using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using GridDominance.Generic.Impl;
using GridDominance.Shared.DeviceBridge;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Language;
using MonoSAMFramework.Portable.Network.Multiplayer;

namespace GridDominance.Android.Impl
{
	class AndroidBridge_IAB : IGDOperatingSystemBridge
	{
		public FileHelper FileHelper { get; } = new AndroidFileHelper();
		public IBillingAdapter IAB => _iab;
		public IBluetoothAdapter BluetoothFull => _btfull;
		public IUDPClient CreateUPDClient() => new XamarinUDPClient();
		public string AppType => "Android.IAB";
		public SAMSystemType SystemType => SAMSystemType.MONOGAME_ANDROID;
		public GDFlavor Flavor => GDFlavor.IAB;
		public FMargin DeviceSafeAreaInset { get; } = FMargin.NONE;

		public string FullDeviceInfoString { get; } = GenerateInfoStr();
		public string DeviceName { get; } = string.Format("{0} {1}", Build.Manufacturer, Build.Model);
		public string DeviceVersion { get; } = string.Format("Android {0} sdk-{1}", Build.VERSION.Release, Build.VERSION.Sdk);
		public FSize DeviceResolution { get; } = ScreenRes();

		public string EnvironmentStackTrace => System.Environment.StackTrace;
		
		private readonly MainActivity _activity;
		private readonly AndroidBilling _iab;
		private readonly XamarinBluetooth _btfull;

		public void OnNativeInitialize(MonoSAMGame game)
		{
			// NOP
		}

		public void OnDestroy()
		{
			_iab.Disconnect();
			_btfull.OnDestroy();
		}

		public AndroidBridge_IAB(MainActivity a)
		{
			_activity = a;

			_iab = new AndroidBilling();
			_btfull = new XamarinBluetooth(a);
		}

		private static string GenerateInfoStr()
		{
			var m = Resources.System.DisplayMetrics;
			var c = Resources.System.Configuration;
			
			StringBuilder b = new StringBuilder();

			b.AppendFormat("VERSION.Codename    := '{0}'\n", Build.VERSION.Codename);
			b.AppendFormat("VERSION.Incremental := '{0}'\n", Build.VERSION.Incremental);
			b.AppendFormat("VERSION.Release     := '{0}'\n", Build.VERSION.Release);
			b.AppendFormat("VERSION.Sdk         := '{0}'\n", Build.VERSION.Sdk);
			b.AppendFormat("VERSION.SdkInt      := '{0}'\n", Build.VERSION.SdkInt);
			b.AppendFormat("Board               := '{0}'\n", Build.Board);
			b.AppendFormat("Bootloader          := '{0}'\n", Build.Bootloader);
			b.AppendFormat("Brand               := '{0}'\n", Build.Brand);
			b.AppendFormat("CpuAbi              := '{0}'\n", Build.CpuAbi);
			b.AppendFormat("CpuAbi2             := '{0}'\n", Build.CpuAbi2);
			b.AppendFormat("Device              := '{0}'\n", Build.Device);
			b.AppendFormat("Display             := '{0}'\n", Build.Display);
			b.AppendFormat("Screen              := '{0}'\n", ScreenRes());
			b.AppendFormat("Fingerprint         := '{0}'\n", Build.Fingerprint);
			b.AppendFormat("Hardware            := '{0}'\n", Build.Hardware);
			b.AppendFormat("Host                := '{0}'\n", Build.Host);
			b.AppendFormat("Id                  := '{0}'\n", Build.Id);
			b.AppendFormat("Manufacturer        := '{0}'\n", Build.Manufacturer);
			b.AppendFormat("Model               := '{0}'\n", Build.Model);
			b.AppendFormat("Product             := '{0}'\n", Build.Product);
			b.AppendFormat("Radio               := '{0}'\n", Build.Radio);
			b.AppendFormat("RadioVersion        := '{0}'\n", Build.RadioVersion);
			b.AppendFormat("Serial              := '{0}'\n", Build.Serial);
			b.AppendFormat("Tags                := '{0}'\n", Build.Tags);
			b.AppendFormat("Time                := '{0}'\n", Build.Time);
			b.AppendFormat("Type                := '{0}'\n", Build.Type);
			b.AppendFormat("User                := '{0}'\n", Build.User);
			b.AppendFormat("LayoutDirection     := '{0}'\n", Resources.System.Configuration.LayoutDirection);
			b.AppendFormat("Country             := '{0}'\n", Resources.System.Configuration.Locale.Country);
			b.AppendFormat("Language            := '{0}'\n", Resources.System.Configuration.Locale.Language);
			b.AppendFormat("Orientation         := '{0}'\n", Resources.System.Configuration.Orientation);
			b.AppendFormat("Touchscreen         := '{0}'\n", Resources.System.Configuration.Touchscreen);
			b.AppendFormat("ScreenResolution    := '{0}'\n", $"{m.WidthPixels}x{m.HeightPixels} <=> {c.ScreenWidthDp}x{c.ScreenHeightDp} (d = {m.Density})");
			b.AppendFormat("VersionType         := '{0}'\n", "IAB");

			return b.ToString();
		}

		public void HandleActivityResult(int requestCode, Result resultCode, Intent data)
		{
			_btfull.HandleActivityResult(requestCode, resultCode, data);
		}

		private static FSize ScreenRes()
		{
			var m = Resources.System.DisplayMetrics;
			return new FSize(m.WidthPixels, m.HeightPixels);
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
			_activity.StartActivity(new Intent(Intent.ActionView, global::Android.Net.Uri.Parse(url)));
		}

		public void Sleep(int milsec)
		{
			Thread.Sleep(milsec);
		}

		public void ExitApp()
		{
			_activity.FinishAffinity();
//			global::Android.OS.Process.KillProcess(global::Android.OS.Process.MyPid());
//			System.Environment.Exit(0);
		}

		public void ShareAppLink()
		{
			Intent i = new Intent(Intent.ActionSend);
			i.SetType("text/plain");
			i.PutExtra(Intent.ExtraSubject, "Cannon Conquest");
			var sAux = "\nLet me recommend you this application\n\n";
			sAux = sAux + "https://play.google.com/store/apps/details?id=com.blackforestbytes.griddominance.iab";
			i.PutExtra(Intent.ExtraText, sAux);
			_activity.StartActivity(Intent.CreateChooser(i, "Choose one"));
		}
	}
}