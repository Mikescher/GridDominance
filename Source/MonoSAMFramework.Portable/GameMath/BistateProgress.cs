namespace MonoSAMFramework.Portable.GameMath
{
	public enum BistateProgress
	{
		Initial,
		Forward,
		Final,
		Backwards,


		Closed  = Initial,
		Opening = Forward,
		Open    = Final,
		Closing = Backwards,
	}
}
