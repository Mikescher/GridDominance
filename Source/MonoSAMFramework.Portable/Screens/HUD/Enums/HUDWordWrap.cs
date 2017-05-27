namespace MonoSAMFramework.Portable.Screens.HUD.Enums
{
	public enum HUDWordWrap
	{
		WrapByCharacter,            // Split lines at the overflowing character
		WrapByWordWithOverflow,     // Split lines at word boundaries - if not possible lines will be longer than allowed
		WrapByWordTrusted,          // Split lines at word boundaries - if not possible split at the overflowing character
		NoWrap,                     // Do not wrap
	}
}
