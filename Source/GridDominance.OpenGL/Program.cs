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
			using (var game = new MainGame(new WindowsImpl())) game.Run();
		}
	}
}
