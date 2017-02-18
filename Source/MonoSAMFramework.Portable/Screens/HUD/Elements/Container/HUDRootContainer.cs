using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Container
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

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			// NOP
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (istate.IsExclusiveJustDown)
			{
				var swallow = InternalPointerDown(istate);

				if (swallow)
				{
					istate.Swallow();
				}
				else
				{
					HUD.FocusedElement = null;
				}
			}

			if (istate.IsExclusiveJustUp)
			{
				var swallow = InternalPointerUp(istate);

				if (swallow) istate.Swallow();
			}
		}

		protected override void RecalculatePosition()
		{
			if (HUD == null) return;

			Size = new FSize(HUD.Width, HUD.Height);
			Position = new FPoint(HUD.Left, HUD.Top);
			BoundingRectangle = new FRectangle(Position, Size);
			
			PositionInvalidated = false;
		}
	}
}
