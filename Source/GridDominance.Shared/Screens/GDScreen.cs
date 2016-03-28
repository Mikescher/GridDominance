using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;

namespace GridDominance.Shared.Screens
{
    abstract class GDScreen : Screen
    {
        public readonly GraphicsDeviceManager Graphics;
        public readonly MainGame Owner;

        public GDScreen(MainGame game, GraphicsDeviceManager gdm)
        {
            Graphics = gdm;
            Owner = game;
        }
    }
}