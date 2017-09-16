using Foundation;
using GridDominance.iOS.Full.Impl;
using GridDominance.Shared;
using UIKit;

namespace GridDominance.iOS.Full
{
	[Register("AppDelegate")]
	class Program : UIApplicationDelegate
	{
		private static MainGame game;
		private static AppleBridge _impl;

		internal static void RunGame()
		{
			_impl = new AppleBridge();
			game = new MainGame();
            game.Construct(_impl);
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
