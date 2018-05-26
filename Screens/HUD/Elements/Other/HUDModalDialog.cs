using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

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

			if (_removeOnBackKey && istate.IsKeyExclusiveJustDown(SKeys.AndroidBack)) { istate.SwallowKey(SKeys.AndroidBack, InputConsumer.HUDElement); Remove(); }
			if (_removeOnBackKey && istate.IsKeyExclusiveJustDown(SKeys.Backspace)) { istate.SwallowKey(SKeys.Backspace, InputConsumer.HUDElement); Remove(); }
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

			Alignment = HUDAlignment.ABSOLUTE;
			Size = new FSize(HUD.RealWidth, HUD.RealHeight);
			RelativePosition = new FPoint(HUD.RealLeft, HUD.RealTop);
			
			// a small trick - don't do a recalc, we do it manually here (faster)
			Position = new FPoint(HUD.RealLeft, HUD.RealTop);
			BoundingRectangle = new FRectangle(Position, Size);
			PositionInvalidated = false;
		}
	}
}
