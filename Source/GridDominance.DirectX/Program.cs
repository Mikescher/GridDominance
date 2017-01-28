using GridDominance.Shared;
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

