using MonoGame.Extended.ViewportAdapters;

namespace GridDominance.Shared.Framework
{
	class InputStateManager
	{
		private readonly ViewportAdapter adapter;

		private InputState stateCache;

		public InputStateManager(ViewportAdapter vadap)
		{
			adapter = vadap;

			stateCache = InputState.GetInitialState();
		}

		public InputState GetNewState()
		{
			return stateCache = InputState.GetState(adapter, stateCache);
		}

		public InputState GetCurrentState()
		{
			return stateCache;
		}
	}
}
