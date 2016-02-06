using GridDominance.Shared.Framework;
using GridDominance.Shared.Screens.GameScreen.Background;
using GridDominance.Shared.Screens.GameScreen.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GridDominance.Shared.Screens.GameScreen
{
	abstract class GDEntity
	{
		protected readonly GameScreen Owner;
		public GDEntityManager Manager = null; // only set after Add - use only in Update() and Render()

		 protected GDEntity(GameScreen scrn)
		{
			Owner = scrn;
		}

		public abstract void Update(GameTime gameTime, InputState istate);
		public abstract void Draw(SpriteBatch sbatch);
		public abstract void OnInitialize();
	}
}
