using System;
using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using GridDominance.Shared;
using Microsoft.Xna.Framework;
using Android.Content;
using GridDominance.Android.Impl;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable;

namespace GridDominance.Android
{
	[Activity(Label = "Cannon Conquest",
		MainLauncher = true,
		Icon = "@drawable/icon",
		Theme = "@style/Theme.Splash",
		LaunchMode = LaunchMode.SingleInstance,
		ScreenOrientation = ScreenOrientation.SensorLandscape,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.Keyboard | ConfigChanges.ScreenSize)]

	// ReSharper disable once ClassNeverInstantiated.Global
	public class MainActivity : AndroidGameActivity
	{
		private AndroidBridge_IAB _impl;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			_impl = new AndroidBridge_IAB(this);
			MonoSAMGame.StaticBridge = _impl;

			var g = new MainGame();

			var _view = g.Services.GetService<View>();

            _view.SystemUiFlags = SystemUiFlags.LayoutStable |
                SystemUiFlags.LayoutHideNavigation |
                SystemUiFlags.LayoutFullscreen |
                SystemUiFlags.Fullscreen;

			if (Build.VERSION.SdkInt >= BuildVersionCodes.P) Window.Attributes.LayoutInDisplayCutoutMode |= LayoutInDisplayCutoutMode.ShortEdges;

            SetContentView(_view);

            g.Run();
		}

		protected override void OnDestroy()
		{
			try
			{
				_impl.OnDestroy();

				base.OnDestroy();
			}
			catch (Exception e)
			{
				SAMLog.Error("AMA_IAB::OnDestroy", e);
			}
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			try
			{
				SAMLog.Debug("BTScanReciever::OnActivityResult(" + data?.Action + ")");

				_impl.HandleActivityResult(requestCode, resultCode, data);
			}
			catch (Exception e)
			{
				SAMLog.Error("AMA_IAB::OnActivityResult", e);
			}
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			for (int i = 0; i < Math.Min(permissions.Length, grantResults.Length); i++)
			{
				SAMLog.Debug($"PermissionRequestResult {permissions[i]} = {grantResults[i]}");
			}
		}
	}
}


