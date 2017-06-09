using System;

namespace MonoSAMFramework.Portable.Input
{
	[Flags]
	public enum KeyModifier
	{
		None    = 0,
		Control = 1,
		Shift   = 2,
		Alt     = 4,


		ShiftCtrl    = Shift | Control,
		AltShift     = Shift |           Alt,
		AltCtrl      =         Control | Alt,
		ShiftCtrlAlt = Shift | Control | Alt,
	}
}
