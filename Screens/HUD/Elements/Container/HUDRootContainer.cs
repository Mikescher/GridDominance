using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Container
{
	public class HUDRootContainer : HUDContainer
	{
		public override int Depth => int.MinValue;
		
		private FMargin _margins;

		public HUDRootContainer()
		{
			// SafeAreaMargins (eg iPhone X Notch)
			_margins = MonoSAMGame.CurrentInst.Bridge.DeviceSafeAreaInset;
		}

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
					istate.Swallow(InputConsumer.HUDElement);
				}
				else
				{
					HUD.FocusedElement = null;
				}
			}

			if (istate.IsExclusiveJustUp)
			{
				var swallow = InternalPointerUp(istate);

				if (swallow) istate.Swallow(InputConsumer.HUDElement);
			}
		}

		protected override void RecalculatePosition()
		{
			if (HUD == null) return;

			Size = new FSize(HUD.Width - _margins.SumX, HUD.Height - _margins.SumY);
			Position = new FPoint(HUD.Left + _margins.MarginLeft, HUD.Top + _margins.MarginTop);
			BoundingRectangle = new FRectangle(Position, Size);
			
			PositionInvalidated = false;
		}
	}
}
