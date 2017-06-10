using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath;
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
		private readonly IHUDModalChild _initElementInterface; // can be null
		private readonly bool _removeOnOutOfBoundsClick;
		private readonly bool _removeOnBackKey;
		private readonly float _dimFactor;
		private readonly float _dimTime;

		public float Lifetime = 0;

		public HUDModalDialog(int d, HUDElement child, float dimFactor, float dimTime, bool removeOnOOBorBack)
		{
			Depth = d;

			_initElement = child;
			_initElementInterface = child as IHUDModalChild;

			_removeOnOutOfBoundsClick = removeOnOOBorBack;
			_removeOnBackKey = removeOnOOBorBack;
			_dimFactor = dimFactor;
			_dimTime = dimTime;
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
			if (_dimFactor > 0)
			{
				if (Lifetime >= _dimTime)
					sbatch.FillRectangle(bounds, Color.Black * _dimFactor);
				else
					sbatch.FillRectangle(bounds, Color.Black * FloatMath.Lerp(0, _dimFactor, Lifetime / _dimTime));
			}
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (!_initElement.Alive) Remove();

			Lifetime += gameTime.ElapsedSeconds;

			if (_removeOnBackKey && istate.IsKeyJustDown(SKeys.AndroidBack)) Remove();
			if (_removeOnBackKey && istate.IsKeyJustDown(SKeys.Escape)) Remove();
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => true;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;

		protected override void OnPointerClick(FPoint relPositionPoint, InputState istate)
		{
			if (_removeOnOutOfBoundsClick)
			{
				if (_initElementInterface != null)
					_initElementInterface.OnOutOfBoundsClick();
				else
					Remove();
			}
			else
			{
				_initElementInterface?.OnOutOfBoundsClick();
			}
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
