namespace MonoSAMFramework.Portable.Input
{
	public class KeyCombination
	{
		public readonly SKeys Key;
		public readonly KeyModifier Mod;

		public KeyCombination(SKeys k, KeyModifier m)
		{
			Key = k;
			Mod = m;
		}

		public string GetMmemonic()
		{
			string str = "";


			if (Mod.HasFlag(KeyModifier.Control))
			{
				str += "Ctrl+";
			}

			if (Mod.HasFlag(KeyModifier.Alt))
			{
				str += "Alt+";
			}

			if (Mod.HasFlag(KeyModifier.Shift))
			{
				str += "Shift+";
			}

			return str + FormatKey(Key);
		}

		private string FormatKey(SKeys k)
		{
			// ReSharper disable once SwitchStatementMissingSomeCases
			switch (k)
			{
				case SKeys.D0:
				case SKeys.D1:
				case SKeys.D2:
				case SKeys.D3:
				case SKeys.D4:
				case SKeys.D5:
				case SKeys.D6:
				case SKeys.D7:
				case SKeys.D8:
				case SKeys.D9:
					return k.ToString().Substring(1);
				case SKeys.AndroidBack:
					return "Back";
				case SKeys.AndroidMenu:
					return "Menu";
				default:
					return k.ToString();
			}
		}
	}
}
