using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Other
{
	public class HUDModalDialog : HUDContainer
	{
		public override int Depth { get; }

		private readonly HUDElement _initElement;
		private readonly bool _removeOnOutOfBoundsClick;
		private readonly float _dimFactor;

		public HUDModalDialog(int d, HUDElement child, float dimFactor, bool removeOnOOB)
		{
			Depth = d;

			_initElement = child;
			_removeOnOutOfBoundsClick = removeOnOOB;
			_dimFactor = dimFactor;
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
			if (_dimFactor > 0) sbatch.FillRectangle(bounds, Color.Black * _dimFactor);
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (!_initElement.Alive) Remove();
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => true;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;

		protected override void OnPointerClick(FPoint relPositionPoint, InputState istate)
		{
			if (_removeOnOutOfBoundsClick)  Remove();
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
