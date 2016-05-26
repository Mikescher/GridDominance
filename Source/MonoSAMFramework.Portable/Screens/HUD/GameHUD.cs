using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Language;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.Screens.HUD
{
	public abstract class GameHUD : ISAMDrawable, ISAMUpdateable
	{
		private class GameHUDElementComparer : Comparer<HUDElement>
		{
			public override int Compare(HUDElement x, HUDElement y) =>(x==null||y==null) ? 0 : x.Depth.CompareTo(y.Depth);
		}

		public readonly GameScreen Owner;

		private readonly AlwaysSortList<HUDElement> elements = new AlwaysSortList<HUDElement>(new GameHUDElementComparer());

		protected GameHUD(GameScreen scrn)
		{
			Owner = scrn;
		}

		public virtual int Top => 0;
		public virtual int Left => 0;

		public virtual int Bottom => Owner.Viewport.VirtualHeight;
		public virtual int Right => Owner.Viewport.VirtualWidth;

		public int Width => Right - Left;
		public int Height => Bottom - Top;

		public int CenterX => Left + Width/2;
		public int CenterY => Top + Height / 2;

		public void Update(GameTime gameTime, InputState istate)
		{
			foreach (var element in elements.ToList())
			{
				element.Update(gameTime, istate);

				if (!element.Alive)
				{
					elements.Remove(element);
					element.OnRemove();
				}
			}
		}

		public void Draw(IBatchRenderer sbatch)
		{
			foreach (var element in elements)
			{
				element.DrawBackground(sbatch);
			}

			foreach (var element in elements)
			{
				element.Draw(sbatch);
			}
		}

		public void AddElement(HUDElement e)
		{
			e.Owner = this;
			elements.Add(e);
			e.Initialize();
		}

		public void AddElements(IEnumerable<HUDElement> es)
		{
			foreach (var e in es)
				AddElement(e);
		}

		public void RecalculateAllElementPositions()
		{
			foreach (var element in elements)
				element.RecalculatePositionLater();
		}

		public int Count()
		{
			return elements.Count;
		}
	}
}
