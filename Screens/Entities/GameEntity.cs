using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.Screens.Entities.MouseArea;
using System;
using System.Collections.Generic;
using System.Linq;
using MonoSAMFramework.Portable.UpdateAgents;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace MonoSAMFramework.Portable.Screens.Entities
{
	public abstract class GameEntity : ISAMDrawable, ISAMUpdateable, ILifetimeObject, IUpdateOperationOwner
	{
		public readonly GameScreen Owner;
		public EntityManager Manager = null; // only set after Add - use only in Update() and Render()

		protected readonly List<IUpdateOperation> ActiveOperations = new List<IUpdateOperation>();
		protected readonly List<GameEntityMouseArea> MouseAreas = new List<GameEntityMouseArea>();

		public IEnumerable<IUpdateOperation> ActiveEntityOperations => ActiveOperations;

		public abstract FPoint Position { get; } // Center
		public abstract FSize DrawingBoundingBox { get; }
		public FRectangle DrawingBoundingRect => FRectangle.CreateByCenter(Position, DrawingBoundingBox);
		public abstract Color DebugIdentColor { get; }

		public bool IsInViewport = true; // is in viewport and is therefore rendered
		public bool Alive = true;
		public float Lifetime = 0;
		
		public int Order { get; private set; }
		public bool OrderDirty = false;

		bool ILifetimeObject.Alive => Alive && (Owner != null && Owner.Alive && !Owner.IsRemoved) && (Owner.Game != null && Owner.Game.Alive);

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
				if (!ActiveOperations[i].UpdateUnchecked(this, gameTime, istate))
				{
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

		public void ChangeOrder(int neworder)
		{
			if (Order == neworder) return;
			Order = neworder;
			OrderDirty = true;
		}

		void IUpdateOperationOwner.AddOperation(IUpdateOperation op) { AddOperation(op); }

		public IUpdateOperation AddOperation(IUpdateOperation op)
		{
			ActiveOperations.Add(op);
			op.InitUnchecked(this);
			return op;
		}

		public IUpdateOperation AddOperationDelayed<TElement>(SAMUpdateOp<TElement> op, float delay) where TElement : GameEntity
		{
			return AddOperation(new DelayedOperation<TElement>(op, delay));
		}

		public void AbortAllOperations(Func<IUpdateOperation, bool> condition)
		{
			for (int i = ActiveOperations.Count - 1; i >= 0; i--)
			{
				if (condition(ActiveOperations[i]))
				{
					ActiveOperations[i].Abort();
				}
			}
		}

		public float? FindFirstOperationProgress(Func<IUpdateOperation, bool> condition)
		{
			for (int i = ActiveOperations.Count - 1; i >= 0; i--)
			{
				if (condition(ActiveOperations[i]) && ActiveOperations[i] is IProgressableOperation ipo) return ipo.Progress;
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

		public GameEntityMouseArea AddClickMouseArea(IFShape shape, Action<GameEntityMouseArea, SAMTime, InputState> clickListener, Action<GameEntityMouseArea, SAMTime, InputState> downListener, Action<GameEntityMouseArea, SAMTime, InputState> upListener, bool swallowEvents = true)
		{
			var area = new GameEntityMouseArea(this, shape, swallowEvents);
			area.AddListener(new GameEntityMouseAreaLambdaAdapter
			{
				MouseClick = clickListener,
				MouseDown = downListener,
				MouseUp = upListener,
			});
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
			sbatch.DrawRectangle(Position.AsTranslated(-DrawingBoundingBox.Width * 0.5f, -DrawingBoundingBox.Height * 0.5f), DrawingBoundingBox, Color.LightGreen, 1);
		}

		protected virtual void DrawDebugAreas(IBatchRenderer sbatch)
		{
			foreach (var area in MouseAreas)
			{
				sbatch.DrawShape(area.AbsoluteShape, area.IsEnabled ? Color.DarkOrange : Color.PeachPuff, 3);
			}
		}

		public abstract void OnInitialize(EntityManager manager);
		public abstract void OnRemove();

		protected abstract void OnUpdate(SAMTime gameTime, InputState istate);
		protected abstract void OnDraw(IBatchRenderer sbatch);
		protected virtual void OnDrawOrderedForegroundLayer(IBatchRenderer sbatch) { /* override me */}
	}
}
