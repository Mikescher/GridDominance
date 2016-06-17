#region Using Statements
using GridDominance.Shared;
using System;
using MonoSAMFramework.Portable.Persistance;

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
			FileHelper.RegisterSytsemSecificHandler(new WindowsFileHelper());

			using (var game = new MainGame())
				game.Run();
		}
	}
}

