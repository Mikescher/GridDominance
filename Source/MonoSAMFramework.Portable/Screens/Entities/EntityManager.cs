using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.Entities
{
	public abstract class EntityManager
	{
		private List<GameEntity> entities = new List<GameEntity>();

		public readonly GameScreen Owner;

		public RectangleF BoundingBox;

		protected EntityManager(GameScreen screen)
		{
			Owner = screen;

			Owner.Owner.Window.ClientSizeChanged += (s, e) => DoRecalcBoundingBox();
			DoRecalcBoundingBox();
		}

		private void DoRecalcBoundingBox()
		{
			BoundingBox = RecalculateBoundingBox();
		}

		public void Update(GameTime gameTime, InputState state)
		{
			OnBeforeUpdate(gameTime, state);

			foreach (var gdEntity in entities.ToList())
			{
				gdEntity.Update(gameTime, state);
				if (!gdEntity.Alive)
				{
					entities.Remove(gdEntity);
					gdEntity.OnRemove();
				}
			}

			OnAfterUpdate(gameTime, state);
		}

		public void Draw(SpriteBatch sbatch)
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
