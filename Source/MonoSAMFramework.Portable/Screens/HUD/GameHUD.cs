using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.Input;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.Screens.HUD
{
	public abstract class GameHUD : ISAMDrawable, ISAMUpdateable
	{
		public readonly GameScreen Owner;

		private readonly List<GameHUDElement> elements = new List<GameHUDElement>();

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

		public void RecalculateAllElementPositions()
		{
			foreach (var element in elements)
			{
				element.RecalculatePosition();
			}
		}
	}
}
