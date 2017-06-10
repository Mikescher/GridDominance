using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Language;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Container
{
	public abstract class HUDContainer : HUDElement
	{
		private class GameHUDElementComparer : Comparer<HUDElement>
		{
			public override int Compare(HUDElement x, HUDElement y) => (x == null || y == null) ? 0 : x.Depth.CompareTo(y.Depth);
		}

		// elements with higher depth are in foreground
		// foreground elements get rendered last (on top)
		// foreground elements get pointer events first
		// Updates happen back to front, foreground elements last
		private readonly AlwaysSortList<HUDElement> children = new AlwaysSortList<HUDElement>(new GameHUDElementComparer());
		private readonly Queue<HUDElement> queuedChildren = new Queue<HUDElement>();
		
		public int ChildrenCount => children.Count;
		public override int DeepInclusiveCount => children.Sum(p => p.DeepInclusiveCount) + 1;
		public int ChildrenMinDepth => children.Any() ? children.Min(c => c.Depth) : -1;
		public int ChildrenMaxDepth => children.Any() ? children.Max(c => c.Depth) : +1;

		public override void DrawForeground(IBatchRenderer sbatch)
		{
			base.DrawForeground(sbatch);

			foreach (var child in children)
			{
				if (child.IsVisible) child.DrawBackground(sbatch);
			}

			foreach (var child in children)
			{
				if (child.IsVisible) child.DrawForeground(sbatch);
			}
		}

		public override void Remove()
		{
			base.Remove();
			foreach (var child in children)
			{
				if (child.IsVisible) child.Remove();
			}
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
			base.Update(gameTime, istate);

			if (Initialized)
			{
				while (queuedChildren.Count > 0) AddElement(queuedChildren.Dequeue());
			}
			
			foreach (var element in children.ToList())
			{
				element.Update(gameTime, istate);

				if (!element.Alive)
				{
					children.Remove(element);
					element.OnRemove();
				}
			}
		}

		public override bool InternalPointerDown(InputState istate)
		{
			foreach (var child in Enumerable.Reverse(children))
			{
				if (child.InternalPointerDown(istate)) return true;
			}

			return base.InternalPointerDown(istate);
		}

		public override bool InternalPointerUp(InputState istate)
		{
			foreach (var child in Enumerable.Reverse(children))
			{
				if (child.InternalPointerUp(istate)) return true;
			}

			return base.InternalPointerUp(istate);
		}

		public virtual void AddElement(HUDElement e)
		{
			if (!Initialized)
			{
				queuedChildren.Enqueue(e);
				return;
			}

			e.Owner = this;
			e.HUD = HUD;
			children.Add(e);
			e.Initialize();
		}

		public void AddElements(IEnumerable<HUDElement> es)
		{
			foreach (var e in es)
				AddElement(e);
		}

		public override void InvalidatePosition()
		{
			base.InvalidatePosition();

			foreach (var child in children)
				child.InvalidatePosition();
		}

		public override void Revalidate()
		{
			base.Revalidate();

			foreach (var child in children)
				child.Revalidate();
		}

		public override IEnumerable<HUDElement> EnumerateElements()
		{
			foreach (var child in children)
			{
				yield return child;
				foreach (var subchild in child.EnumerateElements()) yield return subchild;
			}
		}

		public override void ValidateRecursive()
		{
			base.ValidateRecursive();

			foreach (var child in children) child.ValidateRecursive();
		}
	}
}
