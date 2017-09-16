using Foundation;
using GridDominance.iOS.Full.Impl;
using GridDominance.Shared;
using MonoSAMFramework.Portable;
using UIKit;

namespace GridDominance.iOS.Full
{
	[Register("AppDelegate")]
	class Program : UIApplicationDelegate
	{
		public static MainGame game;
		public static AppleBridge _impl;

		internal static void RunGame()
		{
			MonoSAMGame.StaticBridge = new AppleBridge();
			game = new MainGame();
			game.Run();
		}

		static void Main(string[] args)
		{
			UIApplication.Main(args, null, "AppDelegate");
		}

		public override void FinishedLaunching(UIApplication app)
		{
			RunGame();
		}
	}
}
