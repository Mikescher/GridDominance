using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Content.PM;
using Android.Views;
using Android.Widget;
using Android.OS;
using GridDominance.Shared;

#if OUYA
using Ouya.Console.Api;
#endif

using Microsoft.Xna.Framework;

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

			var g = new MainGame();
			SetContentView(g.Services.GetService<View>());
			g.Run();
		}
		
	}
}


