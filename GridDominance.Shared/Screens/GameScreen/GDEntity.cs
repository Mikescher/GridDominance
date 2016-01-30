using GridDominance.Shared.Framework;
using GridDominance.Shared.Screens.GameScreen.Background;
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

        public abstract void Update(GameTime gameTime, InputState state);
        public abstract void Draw(SpriteBatch sbatch);
    }
}
