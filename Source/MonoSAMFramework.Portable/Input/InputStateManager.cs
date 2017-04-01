using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace MonoSAMFramework.Portable.Input
{
	public class InputStateManager
	{
		private readonly SAMViewportAdapter adapterGame;
		private readonly SAMViewportAdapter adapterHUD;

		private InputState stateCache;
		
		public InputStateManager(SAMViewportAdapter gameAdap, SAMViewportAdapter hudAdap, float mapOffsetX, float mapOffsetY)
		{
			adapterGame = gameAdap;
			adapterHUD = hudAdap;

			stateCache = InputState.GetInitialState(mapOffsetX, mapOffsetY);
		}

		public InputState GetNewState(float mapOffsetX, float mapOffsetY)
		{
			return stateCache = InputState.GetState(adapterGame, adapterHUD, stateCache, mapOffsetX, mapOffsetY);
		}

		public InputState GetCurrentState()
		{
			return stateCache;
		}
	}
}
