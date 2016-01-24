using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace GridDominance.Shared.Screens.GameScreen
{
    abstract class GDEntity : IDraw, IUpdate
    {
        protected readonly GameScreen Owner;

        protected GDEntity(GameScreen screen)
        {
            Owner = screen;
        }

        public abstract void Draw(GameTime gameTime);
        public abstract void Update(GameTime gameTime);
    }
}
