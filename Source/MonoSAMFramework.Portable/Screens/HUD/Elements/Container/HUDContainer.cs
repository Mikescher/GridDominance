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

		private readonly AlwaysSortList<HUDElement> children = new AlwaysSortList<HUDElement>(new GameHUDElementComparer());

		public int ChildrenCount => children.Count;
		public override int DeepInclusiveCount => children.Sum(p => p.DeepInclusiveCount) + 1;

		public override void DrawForeground(IBatchRenderer sbatch)
		{
			base.DrawForeground(sbatch);

			foreach (var child in children)
			{
				child.DrawBackground(sbatch);
			}

			foreach (var child in children)
			{
				child.DrawForeground(sbatch);
			}
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
			base.Update(gameTime, istate);

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
			foreach (var child in children)
			{
				if (child.InternalPointerDown(istate)) return true;
			}

			return base.InternalPointerDown(istate);
		}

		public override bool InternalPointerUp(InputState istate)
		{
			foreach (var child in children)
			{
				if (child.InternalPointerUp(istate)) return true;
			}

			return base.InternalPointerUp(istate);
		}

		public virtual void AddElement(HUDElement e)
		{
#if DEBUG
			if (!Initialized) throw new Exception("Cannot add elements before initialization");
#endif

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
	}
}
