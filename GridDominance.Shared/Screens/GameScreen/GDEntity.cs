using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GridDominance.Shared.Screens.GameScreen
{
    abstract class GDEntity
    {
        protected GDEntity()
        {
			//
        }

        public abstract void Draw(SpriteBatch sbatch);
        public abstract void Update(GameTime gameTime);
    }
}
