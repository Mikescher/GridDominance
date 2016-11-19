using MonoSAMFramework.Portable.Screens.ViewportAdapters;
using System;

namespace MonoSAMFramework.Portable.Input
{
	public class InputStateManager
	{
		private readonly SAMViewportAdapter adapter;

		private InputState stateCache;

		public EventHandler<PointerEventArgs> PointerDown = (o, e) => { };
		public EventHandler<PointerEventArgs> PointerUp = (o, e) => { };

		public InputStateManager(SAMViewportAdapter vadap, float mapOffsetX, float mapOffsetY)
		{
			adapter = vadap;

			stateCache = InputState.GetInitialState(vadap, mapOffsetX, mapOffsetY);
		}

		public InputState GetNewState(float mapOffsetX, float mapOffsetY)
		{
			return stateCache = InputState.GetState(adapter, stateCache, mapOffsetX, mapOffsetY);
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
