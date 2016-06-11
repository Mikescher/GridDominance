
using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using GridDominance.Shared;

#if OUYA
using Ouya.Console.Api;
#endif

using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.FileHelper;

namespace GridDominance.Android
{
	[Activity(Label = "GridDominance.Android", 
		MainLauncher = true,
		Icon = "@drawable/icon",
		Theme = "@style/Theme.Splash",
		AlwaysRetainTaskState = true,
		LaunchMode = LaunchMode.SingleInstance,
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.Keyboard | ConfigChanges.ScreenSize)]
	#if OUYA
	[IntentFilter(new[] { Intent.ActionMain }, Categories = new[] { Intent.CategoryLauncher, OuyaIntent.CategoryGame })]
	#endif
	public class MainActivity : AndroidGameActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			FileHelper.RegisterSytsemSecificHandler(new AndroidFileHelper());

			var g = new MainGame();
			SetContentView(g.Services.GetService<View>());
			g.Run();
		}
		
	}
}


