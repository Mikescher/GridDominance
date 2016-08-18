using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.MathHelper.FloatClasses;
using System;
using System.Collections.Generic;

namespace MonoSAMFramework.Portable.Screens.Entities
{
	public abstract class GameEntity : ISAMDrawable, ISAMUpdateable
	{
		protected readonly GameScreen Owner;
		public EntityManager Manager = null; // only set after Add - use only in Update() and Render()

		protected readonly List<IGameEntityOperation> ActiveOperations = new List<IGameEntityOperation>();

		public abstract Vector2 Position { get; } // Center
		public abstract FSize DrawingBoundingBox { get; }
		public abstract Color DebugIdentColor { get; }

		public bool IsInViewport = true; // is in viewport and is therefore rendered
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

		public void Draw(IBatchRenderer sbatch)
		{
			OnDraw(sbatch);

#if DEBUG
			if (DebugSettings.Get("DebugEntityBoundaries"))
			{
				sbatch.DrawRectangle(Position - DrawingBoundingBox*0.5f, DrawingBoundingBox, Color.LightGreen, 1);
			}
#endif
		}

		public abstract void OnInitialize();
		public abstract void OnRemove();

		protected abstract void OnUpdate(GameTime gameTime, InputState istate);
		protected abstract void OnDraw(IBatchRenderer sbatch);
	}
}
