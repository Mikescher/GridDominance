using Android.Content.Res;
using Android.OS;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.Language;
using System.Security.Cryptography;
using System.Text;
using Android.Content;
using Android.Net;

namespace GridDominance.Android
{
	class AndroidImpl : IOperatingSystemBridge
	{
		public FileHelper FileHelper { get; } = new AndroidFileHelper();

		public string FullDeviceInfoString { get; } = GenerateInfoStr();
		public string DeviceName { get; } = string.Format("{0} {1}", Build.Manufacturer, Build.Model);
		public string DeviceVersion { get; } = string.Format("Android {0} sdk-{1}", Build.VERSION.Release, Build.VERSION.Sdk);
		public string ScreenResolution { get; } = ScreenRes();

		private readonly SHA256 sha256 = SHA256.Create();
		private readonly MainActivity _activity;

		public AndroidImpl(MainActivity a)
		{
			_activity = a;
		}

		private static string GenerateInfoStr()
		{
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

			return b.ToString();
		}

		private static string ScreenRes()
		{
			var m = Resources.System.DisplayMetrics;
			var c = Resources.System.Configuration;

			return $"{m.WidthPixels}x{m.HeightPixels} <=> {c.ScreenWidthDp}x{c.ScreenHeightDp} (d = {m.Density})";
		}

		public string DoSHA256(string input)
		{
			return ByteUtils.ByteToHexBitFiddle(sha256.ComputeHash(Encoding.UTF8.GetBytes(input)));
		}

		public void OpenURL(string url)
		{
			_activity.StartActivity(new Intent(Intent.ActionView, Uri.Parse(url)));
		}

		public void ExitApp()
		{
			_activity.FinishAffinity();
//			global::Android.OS.Process.KillProcess(global::Android.OS.Process.MyPid());
//			System.Environment.Exit(0);
		}
	}
}