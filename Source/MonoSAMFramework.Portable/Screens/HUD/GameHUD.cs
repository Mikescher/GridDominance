using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Language;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.Screens.HUD
{
	public abstract class GameHUD : ISAMDrawable, ISAMUpdateable
	{
		private class GameHUDElementComparer : Comparer<GameHUDElement>
		{
			public override int Compare(GameHUDElement x, GameHUDElement y) =>(x==null||y==null) ? 0 : x.Depth.CompareTo(y.Depth);
		}

		public readonly GameScreen Owner;

		private readonly AlwaysSortList<GameHUDElement> elements = new AlwaysSortList<GameHUDElement>(new GameHUDElementComparer());

		protected GameHUD(GameScreen scrn)
		{
			Owner = scrn;
		}

		public virtual int Top => 0;
		public virtual int Left => 0;

		public virtual int Bottom => Owner.Viewport.VirtualHeight;
		public virtual int Right => Owner.Viewport.VirtualWidth;

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

		public void Draw(SpriteBatch sbatch)
		{
			foreach (var element in elements)
			{
				element.Draw(sbatch);
			}
		}

		public void AddElement(GameHUDElement e)
		{
			e.Owner = this;
			elements.Add(e);
			e.Initialize();
		}

		public void AddElements(IEnumerable<GameHUDElement> es)
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
