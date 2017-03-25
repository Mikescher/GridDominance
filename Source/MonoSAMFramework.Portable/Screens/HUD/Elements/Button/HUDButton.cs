using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using System;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Button
{
	public abstract class HUDButton : HUDContainer
	{
		[Flags]
		public enum HUDButtonClickMode
		{
			Disabled    = 0x0,
			Single      = 0x1,
			Double      = 0x2,
			Triple      = 0x4,
			Hold        = 0x8,			 // Event on PointerUp
			InstantHold = 0x10,			 // Event on HoldDownTime
		}

		private const float LONG_PRESS_TIME = 0.75f;
		private const float MULTI_CLICK_TIME = 0.2f;

		public bool IsPressed => IsPointerDownOnElement;

		public HUDButtonClickMode ClickMode = HUDButtonClickMode.Single;

		private bool IsSingleClickMode      => (ClickMode & HUDButtonClickMode.Single)      == HUDButtonClickMode.Single;
		private bool IsDoubleClickMode      => (ClickMode & HUDButtonClickMode.Double)      == HUDButtonClickMode.Double;
		private bool IsTripleClickMode      => (ClickMode & HUDButtonClickMode.Triple)      == HUDButtonClickMode.Triple;
		private bool IsInstantHoldClickMode => (ClickMode & HUDButtonClickMode.InstantHold) == HUDButtonClickMode.InstantHold;

		private bool IsHoldClickMode        => ((ClickMode & HUDButtonClickMode.Hold) == HUDButtonClickMode.Hold) || ((ClickMode & HUDButtonClickMode.InstantHold) == HUDButtonClickMode.InstantHold);

		private bool suppressClick;
		private float pointerDownTime;
		private float lastClickTime;
		private int multiClickCounter;
		private bool isHoldingDown;

		protected override void OnPointerClick(FPoint relPositionPoint, InputState istate)
		{
			if (!IsEnabled) return;
			if (suppressClick) return;
			if (!IsCursorOnButton(istate)) return;

			lastClickTime = MonoSAMGame.CurrentTime.TotalElapsedSeconds;

			if (IsTripleClickMode)
			{
				multiClickCounter++;

				if (multiClickCounter >= 3)
				{
					multiClickCounter = 0;
					OnTriplePress(istate);
				}
			}
			else if (IsDoubleClickMode)
			{
				multiClickCounter++;

				if (multiClickCounter >= 2)
				{
					multiClickCounter = 0;
					OnDoublePress(istate);
				}
			}
			else if (IsSingleClickMode)
			{
				OnPress(istate);
			}

			HUD.Screen.Game.Sound.TryPlayButtonClickEffect();
		}

		private void UpdateMultiClick(InputState istate, float delta)
		{
			if (multiClickCounter > 0 && delta > MULTI_CLICK_TIME)
			{
				if (multiClickCounter == 1 && IsSingleClickMode)
				{
					OnPress(istate);
				}

				if (multiClickCounter == 2 && IsDoubleClickMode)
				{
					OnDoublePress(istate);
				}

				multiClickCounter = 0;
			}
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
			base.Update(gameTime, istate);

			if (suppressClick && !IsPointerDownOnElement) suppressClick = false;

			if (multiClickCounter > 0) UpdateMultiClick(istate, gameTime.TotalElapsedSeconds - lastClickTime);

			if (IsPointerDownOnElement && IsInstantHoldClickMode) UpdateInstantHold(istate);
		}

		private void UpdateInstantHold(InputState istate)
		{
			if (!IsInstantHoldClickMode) return;
			if (!isHoldingDown) return;
			if (!IsCursorOnButton(istate)) return;

			var delta = MonoSAMGame.CurrentTime.TotalElapsedSeconds - pointerDownTime;

			if (delta > LONG_PRESS_TIME && IsHoldClickMode)
			{
				suppressClick = true;
				pointerDownTime = -999;

				OnHold(istate, delta);
				isHoldingDown = false;
			}
		}

		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate)
		{
			if (!IsCursorOnButton(istate)) return false;

			pointerDownTime = MonoSAMGame.CurrentTime.TotalElapsedSeconds;
			isHoldingDown = true;

			return true;
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate)
		{
			if (!IsPointerDownOnElement) return true;
			if (!isHoldingDown) return true;
			if (!IsCursorOnButton(istate)) return false;

			var delta = MonoSAMGame.CurrentTime.TotalElapsedSeconds - pointerDownTime;

			if (delta > LONG_PRESS_TIME && IsHoldClickMode)
			{
				suppressClick = true;
				pointerDownTime = -999;

				OnHold(istate, delta);
			}

			isHoldingDown = false;

			return true;
		}
		
		protected virtual bool IsCursorOnButton(InputState istate) => true;

		protected abstract void OnPress(InputState istate);
		protected abstract void OnDoublePress(InputState istate);
		protected abstract void OnTriplePress(InputState istate);
		protected abstract void OnHold(InputState istate, float holdTime);
	}
}
