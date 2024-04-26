using System;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Button
{
	public class HUDToggleButtonEventArgs : EventArgs
	{
		public readonly bool NewState;

		public HUDToggleButtonEventArgs(bool newState)
		{
			NewState = newState;
		}
	}
}
