using System.Collections.Generic;
using GridDominance.Shared.Framework;
using GridDominance.Shared.Screens.GameScreen.Background;
using GridDominance.Shared.Screens.GameScreen.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GridDominance.Shared.Screens.GameScreen
{
	abstract class GDEntity
	{
		protected readonly GameScreen Owner;
		public GDEntityManager Manager = null; // only set after Add - use only in Update() and Render()

		private List<IGDEntityOperation> operations = new List<IGDEntityOperation>(); 

		public bool Alive = true;
		public float Lifetime = 0;

		protected GDEntity(GameScreen scrn)
		{
			Owner = scrn;
		}

		protected void Remove()
		{
			Alive = false;
		}

		public void Update(GameTime gameTime, InputState istate)
		{
			Lifetime += gameTime.GetElapsedSeconds();

			for (int i = operations.Count - 1; i >= 0; i--)
			{
				if (!operations[i].Update(this, gameTime, istate))
				{
					operations.RemoveAt(i);
				}
			}

			OnUpdate(gameTime, istate);
		}

		public void AddEntityOperation(IGDEntityOperation op)
		{
			operations.Add(op);
		}

		public abstract void OnUpdate(GameTime gameTime, InputState istate);
		public abstract void Draw(SpriteBatch sbatch);
		public abstract void OnInitialize();
		public abstract void OnRemove();
	}
}
