using MonoGame.Extended.ViewportAdapters;
using System;

namespace MonoSAMFramework.Portable.Input
{
	public class InputStateManager
	{
		private readonly ViewportAdapter adapter;

		private InputState stateCache;

		public EventHandler<PointerEventArgs> PointerDown = (o, e) => { };
		public EventHandler<PointerEventArgs> PointerUp = (o, e) => { };

		public InputStateManager(ViewportAdapter vadap)
		{
			adapter = vadap;

			stateCache = InputState.GetInitialState(vadap);
		}

		public InputState GetNewState()
		{
			return stateCache = InputState.GetState(adapter, stateCache);
		}

		public InputState GetCurrentState()
		{
			return stateCache;
		}

		public void TriggerListener()
		{
			var state = GetCurrentState();

			if (state.IsJustDown) PointerDown(this, new PointerEventArgs(state.PointerPosition));
			if (state.IsJustUp) PointerUp(this, new PointerEventArgs(state.PointerPosition));
		}
	}
}
