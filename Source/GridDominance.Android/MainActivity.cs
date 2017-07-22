using System;
using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using GridDominance.Shared;
using Microsoft.Xna.Framework;
using Android.Content;
using GridDominance.Android.Impl;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Android
{
	[Activity(Label = "GridDominance.Android",
		MainLauncher = true,
		Icon = "@drawable/icon",
		Theme = "@style/Theme.Splash",
		LaunchMode = LaunchMode.SingleInstance,
		ScreenOrientation = ScreenOrientation.Landscape,
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.Keyboard | ConfigChanges.ScreenSize)]

	// ReSharper disable once ClassNeverInstantiated.Global
	public class MainActivity : AndroidGameActivity
	{
		private AndroidImpl _impl;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			_impl = new AndroidImpl(this);
			var g = new MainGame(_impl);
			SetContentView(g.Services.GetService<View>());
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
				SAMLog.Error("AMA::OnDestroy", e);
			}
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			try
			{
				SAMLog.Debug("BTScanReciever::OnActivityResult(" + data.Action + ")");

				_impl.HandleActivityResult(requestCode, resultCode, data);
			}
			catch (Exception e)
			{
				SAMLog.Error("AMA::OnActivityResult", e);
			}
		}
	}
}


