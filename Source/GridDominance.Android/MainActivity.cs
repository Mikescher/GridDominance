using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using GridDominance.Shared;
using Microsoft.Xna.Framework;
using Android.Content;

namespace GridDominance.Android
{
	[Activity(Label = "GridDominance.Android", 
		MainLauncher = true,
		Icon = "@drawable/icon",
		Theme = "@style/Theme.Splash",
		LaunchMode = LaunchMode.SingleInstance,
		ScreenOrientation = ScreenOrientation.Landscape,
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.Keyboard | ConfigChanges.ScreenSize)]

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
			_impl.OnDestroy();

			base.OnDestroy();
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			_impl.HandleActivityResult(requestCode, resultCode, data);
		}
	}
}


