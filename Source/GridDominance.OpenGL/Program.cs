using GridDominance.Shared;
using System;
using GridDominance.Windows;

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
