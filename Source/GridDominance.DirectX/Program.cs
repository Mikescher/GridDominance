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
            using (var game = new MainGame())
            {
                game.Construct(new WindowsBridge());
                game.Run();
            }
		}
	}
}

