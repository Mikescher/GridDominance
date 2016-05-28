using System;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Button
{
	public class HUDButtonEventArgs : EventArgs
	{
		public readonly HUDButtonEventType EventType;

		public HUDButtonEventArgs(HUDButtonEventType type)
		{
			EventType = type;
		}
	}
}
