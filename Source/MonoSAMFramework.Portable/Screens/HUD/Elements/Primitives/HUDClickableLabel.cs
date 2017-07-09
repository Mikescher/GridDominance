using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using System;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives
{
	public class HUDClickableLabel : HUDLabel
	{
		public delegate void LabelEventHandler(HUDClickableLabel sender, EventArgs e);

		public event LabelEventHandler LabelClick;

		public LabelEventHandler Click { set => LabelClick += value; }
		public bool ClickSound = false;

		private bool recalcText = false;

		public HUDClickableLabel(int depth = 0) : base(depth)
		{ 

		}

		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => IsVisible;
		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => IsVisible;

		protected override void OnPointerClick(FPoint relPositionPoint, InputState istate)
		{
			if (!IsEnabled) return;

			if (ClickSound) HUD.Screen.Game.Sound.TryPlayButtonClickEffect();

			LabelClick?.Invoke(this, new EventArgs());
		}
	}
}
