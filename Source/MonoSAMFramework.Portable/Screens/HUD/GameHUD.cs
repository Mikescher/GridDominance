using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using System.Collections.Generic;

namespace MonoSAMFramework.Portable.Screens.HUD
{
	public abstract class GameHUD : ISAMDrawable, ISAMUpdateable
	{
		public readonly GameScreen Screen;
		private readonly HUDRootContainer root;
		public readonly SpriteFont DefaultFont;

		protected GameHUD(GameScreen scrn, SpriteFont font)
		{
			Screen = scrn;
			DefaultFont = font;

			root = new HUDRootContainer { HUD = this };
			root.Initialize();
		}

		public virtual int Top => 0;
		public virtual int Left => 0;

		public virtual int Bottom => Screen.Viewport.VirtualHeight;
		public virtual int Right => Screen.Viewport.VirtualWidth;

		public int Width => Right - Left;
		public int Height => Bottom - Top;

		public int CenterX => Left + Width/2;
		public int CenterY => Top + Height / 2;
		
		public void Update(GameTime gameTime, InputState istate)
		{
			root.Update(gameTime, istate);
		}

		public void Draw(IBatchRenderer sbatch)
		{
			root.DrawBackground(sbatch);
			root.DrawForeground(sbatch);
		}

		public void AddElement(HUDElement e)
		{
			root.AddElement(e);
		}

		public void AddElements(IEnumerable<HUDElement> es)
		{
			root.AddElements(es);
		}

		public void RecalculateAllElementPositions()
		{
			root.InvalidatePosition();
		}

		public int FlatCount()
		{
			return root.ChildrenCount;
		}

		public int DeepCount()
		{
			return root.DeepInclusiveCount;
		}
	}
}
