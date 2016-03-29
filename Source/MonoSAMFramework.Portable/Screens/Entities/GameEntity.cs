using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.Entities
{
	public abstract class GameEntity
	{
		protected readonly GameScreen Owner;
		public EntityManager Manager = null; // only set after Add - use only in Update() and Render()

		protected readonly List<IGameEntityOperation> ActiveOperations = new List<IGameEntityOperation>();

		public abstract Vector2 Position { get; }

		public bool Alive = true;
		public float Lifetime = 0;

		protected GameEntity(GameScreen scrn)
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

		public void AddEntityOperation(IGameEntityOperation op)
		{
			ActiveOperations.Add(op);
			op.OnStart(this);
		}

		public void RemoveAllOperations(Func<IGameEntityOperation, bool> condition)
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
