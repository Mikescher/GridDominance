using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;

namespace GridDominance.Shared.Screens
{
    abstract class GDScreen : Screen
    {
        protected readonly GraphicsDeviceManager graphics;
        protected readonly MainGame owner;

        public GDScreen(MainGame game, GraphicsDeviceManager gdm)
        {
            graphics = gdm;
            owner = game;
        }
    }
}
