using GridDominance.Shared;
using System;
using GridDominance.Windows;
using MonoSAMFramework.Portable;

namespace GridDominance.OpenGL
{
	public static class Program
	{
		[STAThread]
		static void Main()
		{
			MonoSAMGame.StaticBridge = new WindowsBridge();

			using (var game = new MainGame()) game.Run();
		}
	}
}
