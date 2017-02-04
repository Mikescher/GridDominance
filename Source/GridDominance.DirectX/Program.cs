using GridDominance.Shared;
using GridDominance.Windows;
using System;

namespace GridDominance.DirectX
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			using (var game = new MainGame(new WindowsImpl())) game.Run();
		}
	}
}

