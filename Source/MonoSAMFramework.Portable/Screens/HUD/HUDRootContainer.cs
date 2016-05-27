using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.HUD
{
	public class HUDRootContainer : HUDContainer
	{
		public override int Depth => int.MinValue;

		public override void OnInitialize()
		{
			// NOP
		}

		public override void OnRemove()
		{
			// NOP
		}

		protected override void DoDraw(IBatchRenderer sbatch, Rectangle bounds)
		{
			// NOP
		}

		protected override void DoUpdate(GameTime gameTime, InputState istate)
		{
			// NOP
		}

		protected override void RecalculatePosition()
		{
			if (HUD == null) return;

			Size = new Size(HUD.Width, HUD.Height);
			Position = new Point(HUD.Left, HUD.Top);
			BoundingRectangle = new Rectangle(Position, Size);
			
			PositionInvalidated = false;
		}
	}
}
