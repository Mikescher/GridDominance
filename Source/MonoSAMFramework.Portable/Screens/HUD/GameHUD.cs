using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
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

		public float Left => -Screen.VAdapter.VirtualGuaranteedBoundingsOffsetX;
		public float Top => -Screen.VAdapter.VirtualGuaranteedBoundingsOffsetY;

		public float Right => Screen.VAdapter.VirtualTotalWidth - Screen.VAdapter.VirtualGuaranteedBoundingsOffsetX;
		public float Bottom => Screen.VAdapter.VirtualTotalHeight - Screen.VAdapter.VirtualGuaranteedBoundingsOffsetY;

		public float Width => Right - Left;
		public float Height => Bottom - Top;

		public float CenterX => Left + Width/2;
		public float CenterY => Top + Height / 2;

		public float PixelWidth => Width * 1f / Screen.VAdapter.RealTotalWidth;

		public void Update(SAMTime gameTime, InputState istate)
		{
			root.Update(gameTime, istate);

			OnUpdate(gameTime, istate);
		}

		protected virtual void OnUpdate(SAMTime gameTime, InputState istate)
		{
			// override me
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
