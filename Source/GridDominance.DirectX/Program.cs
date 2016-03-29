#region Using Statements
using GridDominance.Shared;
using System;

#if MONOMAC
using MonoMac.AppKit;
using MonoMac.Foundation;

#elif __IOS__
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
#endregion

namespace GridDominance.DirectX
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			using (var game = new MainGame())
				game.Run();
		}
	}
}

