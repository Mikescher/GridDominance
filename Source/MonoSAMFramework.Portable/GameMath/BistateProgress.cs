namespace MonoSAMFramework.Portable.GameMath
{
	public enum BistateProgress
	{
		Initial   = 0x00,
		Forward   = 0x01,
		Final     = 0x10,
		Backwards = 0x11,

		Undefined = 0xFF,

		Closed  = Initial,
		Opening = Forward,
		Open    = Final,
		Closing = Backwards,


		Normal    = Initial,
		Expanding = Forward,
		Expanded  = Final,
		Reverting = Backwards,

	}
}
