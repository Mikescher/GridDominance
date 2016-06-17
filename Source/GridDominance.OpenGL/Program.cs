using GridDominance.Shared;
using System;
using MonoSAMFramework.Portable.Persistance;

namespace GridDominance.OpenGL
{
	public static class Program
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
