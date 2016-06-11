using GridDominance.Shared;
using MonoSAMFramework.Portable.FileHelper;
using System;

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
