using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Container
{
	public class HUDRootContainer : HUDContainer
	{
		public override int Depth => int.MinValue;
		
		public HUDRootContainer()
		{
			//
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

			Alignment = HUDAlignment.ABSOLUTE;
			Size = new FSize(HUD.SafeWidth, HUD.SafeHeight);
			RelativePosition = new FPoint(HUD.SafeLeft, HUD.SafeTop);

			// trick for speed - no real recalc, just set Position+Bounds directly
			Position = new FPoint(HUD.SafeLeft, HUD.SafeTop);
			BoundingRectangle = new FRectangle(Position, Size);
			PositionInvalidated = false;
		}
	}
}
