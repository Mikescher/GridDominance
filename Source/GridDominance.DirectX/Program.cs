using GridDominance.Shared;
using GridDominance.Windows;
using MonoSAMFramework.Portable;
using System;

namespace GridDominance.DirectX
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			MonoSAMGame.StaticBridge = new WindowsBridge();

			using (var game = new MainGame()) game.Run();
		}
	}
}

