using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using GridDominance.Shared;
using Microsoft.Xna.Framework;

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
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			
			var g = new MainGame(new AndroidImpl(this));
			SetContentView(g.Services.GetService<View>());
			g.Run();
		}
		
	}
}


