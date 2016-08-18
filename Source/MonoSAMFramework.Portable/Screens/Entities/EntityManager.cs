using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.MathHelper.FloatClasses;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.Screens.Entities
{
	public abstract class EntityManager : ISAMDrawable, ISAMUpdateable
	{
		private const int VIEWPORT_TOLERANCE = 32;

		private readonly List<GameEntity> entities = new List<GameEntity>();

		public readonly GameScreen Owner;

		public FRectangle BoundingBox;

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
			var viewportBox = Owner.CompleteMapViewport.AsInflated(VIEWPORT_TOLERANCE, VIEWPORT_TOLERANCE);

			foreach (var gdEntity in entities)
			{
				if (viewportBox.Contains(gdEntity.Position, gdEntity.DrawingBoundingBox))
				{
					gdEntity.IsInViewport = true;
					gdEntity.Draw(sbatch);
				}
				else
				{
					gdEntity.IsInViewport = false;
				}
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
		protected abstract FRectangle RecalculateBoundingBox();
		protected abstract void OnBeforeUpdate(GameTime gameTime, InputState state);
		protected abstract void OnAfterUpdate(GameTime gameTime, InputState state);
	}
}
