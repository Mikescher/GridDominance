#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Shared;

#if MONOMAC
using MonoMac.AppKit;
using MonoMac.Foundation;

#elif __IOS__
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
#endregion

namespace GridDominance.Shared
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

