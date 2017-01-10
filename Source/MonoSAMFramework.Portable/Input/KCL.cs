namespace MonoSAMFramework.Portable.Input
{
	public static class KCL // KeyCombinationList
	{
		public static KeyCombinationList C(SKeys key1, KeyModifier mod1)
		{
			return new KeyCombinationList(new KeyCombination(key1, mod1));
		}

		public static KeyCombinationList C(SKeys key1, KeyModifier mod1, SKeys key2, KeyModifier mod2)
		{
			return new KeyCombinationList(new KeyCombination(key1, mod1), new KeyCombination(key2, mod2));
		}

		public static KeyCombinationList C(SKeys key1, KeyModifier mod1, SKeys key2, KeyModifier mod2, SKeys key3, KeyModifier mod3)
		{
			return new KeyCombinationList(new KeyCombination(key1, mod1), new KeyCombination(key2, mod2), new KeyCombination(key3, mod3));
		}

		public static KeyCombinationList C(SKeys key1)
		{
			return new KeyCombinationList(new KeyCombination(key1, KeyModifier.None));
		}

		public static KeyCombinationList C(SKeys key1, SKeys key2)
		{
			return new KeyCombinationList(new KeyCombination(key1, KeyModifier.None), new KeyCombination(key2, KeyModifier.None));
		}

		public static KeyCombinationList C(SKeys key1, SKeys key2, SKeys key3)
		{
			return new KeyCombinationList(new KeyCombination(key1, KeyModifier.None), new KeyCombination(key2, KeyModifier.None), new KeyCombination(key3, KeyModifier.None));
		}
	}
}
