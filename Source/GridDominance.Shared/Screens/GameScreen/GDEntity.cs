using System;
using System.Collections.Generic;
using GridDominance.Shared.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GridDominance.Shared.Screens.GameScreen
{
	abstract class GDEntity
	{
		protected readonly GameScreen Owner;
		public GDEntityManager Manager = null; // only set after Add - use only in Update() and Render()
 
		protected readonly List<IGDEntityOperation> ActiveOperations = new List<IGDEntityOperation>(); 

		public abstract Vector2 Position { get; }

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

			for (int i = ActiveOperations.Count - 1; i >= 0; i--)
			{
				if (!ActiveOperations[i].Update(this, gameTime, istate))
				{
					ActiveOperations[i].OnEnd(this);
					ActiveOperations.RemoveAt(i);
				}
			}

			OnUpdate(gameTime, istate);
		}

		public void AddEntityOperation(IGDEntityOperation op)
		{
			ActiveOperations.Add(op);
			op.OnStart(this);
		}

		public void RemoveAllOperations(Func<IGDEntityOperation, bool> condition )
		{
			for (int i = ActiveOperations.Count - 1; i >= 0; i--)
			{
				if (condition(ActiveOperations[i]))
				{
					ActiveOperations[i].OnEnd(this);
					ActiveOperations.RemoveAt(i);
				}
			}
		}

		public abstract void OnInitialize();
		public abstract void OnRemove();
		protected abstract void OnUpdate(GameTime gameTime, InputState istate);
		public abstract void Draw(SpriteBatch sbatch);
	}
}
