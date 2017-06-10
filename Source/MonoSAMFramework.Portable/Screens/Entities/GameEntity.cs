using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.Screens.Entities.MouseArea;
using MonoSAMFramework.Portable.Screens.Entities.Operation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.Screens.Entities
{
	public abstract class GameEntity : ISAMDrawable, ISAMUpdateable
	{
		public readonly GameScreen Owner;
		public EntityManager Manager = null; // only set after Add - use only in Update() and Render()

		protected readonly List<IGameEntityOperation> ActiveOperations = new List<IGameEntityOperation>();
		protected readonly List<GameEntityMouseArea> MouseAreas = new List<GameEntityMouseArea>();

		public IEnumerable<IGameEntityOperation> ActiveEntityOperations => ActiveOperations;

		public abstract Vector2 Position { get; } // Center
		public abstract FSize DrawingBoundingBox { get; }
		public FRectangle DrawingBoundingRect => FRectangle.CreateByCenter(Position, DrawingBoundingBox);
		public abstract Color DebugIdentColor { get; }

		public bool IsInViewport = true; // is in viewport and is therefore rendered
		public bool Alive = true;
		public float Lifetime = 0;
		public readonly int Order;

		protected GameEntity(GameScreen scrn, int order)
		{
			Owner = scrn;
			Order = order;
		}

		protected void Remove()
		{
			Alive = false;
		}

		public void Update(SAMTime gameTime, InputState istate)
		{
			Lifetime += gameTime.ElapsedSeconds;

			UpdateOperations(gameTime, istate);
			UpdateMouseAreas(gameTime, istate);

			OnUpdate(gameTime, istate);
		}

		private void UpdateOperations(SAMTime gameTime, InputState istate)
		{
			for (int i = ActiveOperations.Count - 1; i >= 0; i--)
			{
				if (!ActiveOperations[i].Update(this, gameTime, istate))
				{
					ActiveOperations[i].OnEnd(this);
					ActiveOperations.RemoveAt(i);
				}
			}
		}

		private void UpdateMouseAreas(SAMTime gameTime, InputState istate)
		{
			foreach (var area in MouseAreas)
			{
				area.Update(gameTime, istate);
			}
		}

		public IGameEntityOperation AddEntityOperation(IGameEntityOperation op)
		{
			ActiveOperations.Add(op);
			op.OnStart(this);
			return op;
		}

		public IGameEntityOperation AddEntityOperationDelayed(IGameEntityOperation op, float delay)
		{
			ActiveOperations.Add(new DelayGameEntityOperation(op.Name + "#delay", delay, op));
			op.OnStart(this);
			return op;
		}

		public void FinishAllOperations(Func<IGameEntityOperation, bool> condition)
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

		public void AbortAllOperations(Func<IGameEntityOperation, bool> condition)
		{
			for (int i = ActiveOperations.Count - 1; i >= 0; i--)
			{
				if (condition(ActiveOperations[i]))
				{
					ActiveOperations[i].OnAbort(this);
					ActiveOperations.RemoveAt(i);
				}
			}
		}

		public float? FindFirstOperationProgress(Func<IGameEntityOperation, bool> condition)
		{
			for (int i = ActiveOperations.Count - 1; i >= 0; i--)
			{
				if (condition(ActiveOperations[i]))
				{
					return ActiveOperations[i].Progress;
				}
			}
			return null;
		}

		public bool HasActiveOperation(string name)
		{
			return ActiveOperations.Any(o => o.Name == name);
		}

		public GameEntityMouseArea AddMouseArea(IFShape shape, IGameEntityMouseAreaListener listener, bool swallowEvents = true)
		{
			var area = new GameEntityMouseArea(this, shape, swallowEvents);
			area.AddListener(listener);
			MouseAreas.Add(area);
			return area;
		}

		public GameEntityMouseArea AddClickMouseArea(IFShape shape, Action<GameEntityMouseArea, SAMTime, InputState> clickListener, bool swallowEvents = true)
		{
			var area = new GameEntityMouseArea(this, shape, swallowEvents);
			area.AddListener(new GameEntityMouseAreaLambdaAdapter { MouseClick = clickListener });
			MouseAreas.Add(area);
			return area;
		}

		public GameEntityMouseArea AddMouseDownMouseArea(IFShape shape, Action<GameEntityMouseArea, SAMTime, InputState> clickListener, bool swallowEvents = true)
		{
			var area = new GameEntityMouseArea(this, shape, swallowEvents);
			area.AddListener(new GameEntityMouseAreaLambdaAdapter { MouseDown = clickListener });
			MouseAreas.Add(area);
			return area;
		}

		public void Draw(IBatchRenderer sbatch)
		{
			OnDraw(sbatch);

#if DEBUG
			if (DebugSettings.Get("DebugEntityBoundaries"))
			{
				using (sbatch.BeginDebugDraw()) DrawDebugBorders(sbatch);
			}
			if (DebugSettings.Get("DebugEntityMouseAreas"))
			{
				using (sbatch.BeginDebugDraw()) DrawDebugAreas(sbatch);
			}
#endif
		}

		public void DrawOrderedForegroundLayer(IBatchRenderer sbatch)
		{
			// This drawcall gets called after all other entities on the same order level as this one have been OnDraw'ed
			// good to draw shadows in normal OnDraw and objects in ForegroundDraw

			OnDrawOrderedForegroundLayer(sbatch);
		}

		protected virtual void DrawDebugBorders(IBatchRenderer sbatch)
		{
			sbatch.DrawRectangle(Position - DrawingBoundingBox * 0.5f, DrawingBoundingBox, Color.LightGreen, 1);
		}

		protected virtual void DrawDebugAreas(IBatchRenderer sbatch)
		{
			foreach (var area in MouseAreas)
			{
				sbatch.DrawShape(area.AbsoluteShape, area.IsEnabled ? Color.DarkOrange : Color.PeachPuff, 1);
			}
		}

		public abstract void OnInitialize(EntityManager manager);
		public abstract void OnRemove();

		protected abstract void OnUpdate(SAMTime gameTime, InputState istate);
		protected abstract void OnDraw(IBatchRenderer sbatch);
		protected virtual void OnDrawOrderedForegroundLayer(IBatchRenderer sbatch) { /* override me */}
	}
}
