using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Other
{
	public class HUDModalDialog : HUDContainer
	{
		public override int Depth => int.MinValue;

		private readonly HUDElement _initElement;

		public HUDModalDialog()
		{
			_initElement = null;
		}

		public HUDModalDialog(HUDElement child)
		{
			_initElement = child;
		}

		public override void OnInitialize()
		{
			if (_initElement != null) AddElement(_initElement);
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
			// NOP
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => true;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;

		protected override void OnPointerClick(FPoint relPositionPoint, InputState istate)
		{
			// Do nothing - swallow
		}

		public override void AddElement(HUDElement e)
		{
			if (ChildrenCount == 0)
			{
				base.AddElement(e);
			}
			else
			{
				SAMLog.Error("SAM_INTERNAL", "Cannot add more than one element to HUDModalDialog");
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
