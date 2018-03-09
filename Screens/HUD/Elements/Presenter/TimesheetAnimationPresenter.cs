using System;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper.TimesheetAnimation;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Presenter
{
	public class TimesheetAnimationPresenter : HUDElement
	{
		public override int Depth { get; }

		public TimesheetAnimation Animation = null;

		private float _animPos = 0;

		public delegate void ButtonEventHandler(TimesheetAnimationPresenter sender, EventArgs e);

		public event ButtonEventHandler ButtonClick;

		public ButtonEventHandler Click
		{
			set => ButtonClick += value;
		}

		public TimesheetAnimationPresenter(int d = 0)
		{
			Depth = d;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			Animation?.Draw(sbatch, bounds, _animPos);
		}

		public override void OnInitialize()
		{
			//
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			_animPos += gameTime.ElapsedSeconds;
		}

		protected override void OnPointerClick(FPoint relPositionPoint, InputState istate)
		{
			ButtonClick?.Invoke(this, EventArgs.Empty);
		}

		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;
		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => true;
	}
}
