using System;
using GridDominance.Shared;

namespace GridDominance.OpenGL
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new MainGame())
                game.Run();
        }
    }
}
