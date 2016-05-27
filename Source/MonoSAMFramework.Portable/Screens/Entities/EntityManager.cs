using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using System.Collections.Generic;
using System.Linq;
using MonoSAMFramework.Portable.Interfaces;

namespace MonoSAMFramework.Portable.Screens.Entities
{
	public abstract class EntityManager : ISAMDrawable, ISAMUpdateable
	{
		private readonly List<GameEntity> entities = new List<GameEntity>();

		public readonly GameScreen Owner;

		public RectangleF BoundingBox;

		protected EntityManager(GameScreen screen)
		{
			Owner = screen;

			Owner.Game.Window.ClientSizeChanged += (s, e) => DoRecalcBoundingBox();
			DoRecalcBoundingBox();
		}

		private void DoRecalcBoundingBox()
		{
			BoundingBox = RecalculateBoundingBox();
		}

		public void Update(GameTime gameTime, InputState state)
		{
			OnBeforeUpdate(gameTime, state);

			foreach (var entity in entities.ToList())
			{
				entity.Update(gameTime, state);
				if (!entity.Alive)
				{
					entities.Remove(entity);
					entity.OnRemove();
				}
			}

			OnAfterUpdate(gameTime, state);
		}

		public void Draw(IBatchRenderer sbatch)
		{
			foreach (var gdEntity in entities)
			{
				gdEntity.Draw(sbatch);
			}
		}

		public void AddEntity(GameEntity e)
		{
			e.Manager = this;
			entities.Add(e);
			e.OnInitialize();
		}

		public int Count()
		{
			return entities.Count;
		}
		
		public IEnumerable<GameEntity> Enumerate()
		{
			return entities;
		}

		public abstract void DrawOuterDebug();
		protected abstract RectangleF RecalculateBoundingBox();
		protected abstract void OnBeforeUpdate(GameTime gameTime, InputState state);
		protected abstract void OnAfterUpdate(GameTime gameTime, InputState state);
	}
}
