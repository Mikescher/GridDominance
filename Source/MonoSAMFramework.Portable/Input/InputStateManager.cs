using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace MonoSAMFramework.Portable.Input
{
	public class InputStateManager
	{
		private readonly SAMViewportAdapter adapter;

		private InputState stateCache;
		
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
	}
}
