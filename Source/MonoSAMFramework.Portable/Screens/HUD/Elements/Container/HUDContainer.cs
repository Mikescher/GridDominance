using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Language;
using System.Collections.Generic;
using System.Linq;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;

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
		public int NonToastChildrenMaxDepth => children.Any() ? children.Where(c => !(c is HUDToast)).Max(c => c.Depth) : +1;
		public IEnumerable<HUDElement> Children => children;

		public override void DrawForeground(IBatchRenderer sbatch)
		{
			base.DrawForeground(sbatch);

			var copy = children.ToList();

			foreach (var child in copy)
			{
				if (child.IsVisible) child.DrawBackground(sbatch);
			}

			foreach (var child in copy)
			{
				if (child.IsVisible) child.DrawForeground(sbatch);
			}
		}

		public override void Remove()
		{
			base.Remove();
			foreach (var child in children)
			{
				child.Remove();
			}
		}

		public void ClearChildren()
		{
			foreach (var child in children)
			{
				child.Remove();
			}
			children.Clear();
		}

		public void RemoveChild(HUDElement c)
		{
			children.Remove(c);
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
			base.Update(gameTime, istate);

			if (Initialized)
			{
				while (queuedChildren.Count > 0) AddElement(queuedChildren.Dequeue());
			}

			bool childrenChanged = false;
			foreach (var element in Enumerable.Reverse(children).ToList()) // in reverse order so topmost elements get keypresses first
			{
				element.Update(gameTime, istate);

				if (!element.Alive)
				{
					children.Remove(element);
					element.OnRemove();
					childrenChanged = true;
				}
			}
			if (childrenChanged) OnChildrenChanged();
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

			OnChildrenChanged();
		}

		public virtual void AddElements(IEnumerable<HUDElement> es)
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

		protected override void OnAfterRecalculatePosition()
		{
			base.OnAfterRecalculatePosition();

			foreach (var child in children)
			{
				child.Revalidate();
			}
		}

		protected virtual void OnChildrenChanged()
		{
			// OVERRIDE ME
		}
	}
}
