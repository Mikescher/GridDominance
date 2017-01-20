using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.Screens.Entities
{
	public abstract class EntityManager : ISAMDrawable, ISAMPostDrawable, ISAMUpdateable
	{
		private const int VIEWPORT_TOLERANCE = 32;

		private readonly List<GameEntity> entities = new List<GameEntity>();
		private readonly List<ISAMPostDrawable> postDrawEntities = new List<ISAMPostDrawable>();

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

		public void Update(SAMTime gameTime, InputState state)
		{
			OnBeforeUpdate(gameTime, state);

			foreach (var entity in entities.ToList())
			{
				entity.Update(gameTime, state);
				if (!entity.Alive)
				{
					entities.Remove(entity);
					entity.OnRemove();

					var pde = entity as ISAMPostDrawable;
					if (pde != null) postDrawEntities.Remove(pde);
				}
			}

			OnAfterUpdate(gameTime, state);
		}

		public void Draw(IBatchRenderer sbatch)
		{
			var viewportBox = Owner.CompleteMapViewport.AsInflated(VIEWPORT_TOLERANCE, VIEWPORT_TOLERANCE);

			foreach (var entity in entities)
			{
				if (viewportBox.Contains(entity.Position, entity.DrawingBoundingBox))
				{
					entity.IsInViewport = true;
					entity.Draw(sbatch);
				}
				else
				{
					entity.IsInViewport = false;
				}
			}
		}

		public void PostDraw()
		{
			foreach (var entity in postDrawEntities)
			{
				var ent = entity as GameEntity;
				if (ent != null)
				{
					//if (ent.IsInViewport)
						entity.PostDraw();
				}
				else
				{
					entity.PostDraw();
				}
			}
		}

		public void AddEntity(GameEntity e)
		{
			e.Manager = this;
			entities.Add(e);
			e.OnInitialize(this);
		}

		public void RegisterPostDraw(ISAMPostDrawable e)
		{
			postDrawEntities.Add(e);
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
		protected abstract void OnBeforeUpdate(SAMTime gameTime, InputState state);
		protected abstract void OnAfterUpdate(SAMTime gameTime, InputState state);
	}
}
