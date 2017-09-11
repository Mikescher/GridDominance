using MonoSAMFramework.Portable.LogProtocol;

namespace MonoSAMFramework.Portable.Localization
{
	public static class L10N
	{
		public const int LANG_EN_US =  0;
		public const int LANG_DE_DE =  1;
		public const int LANG_FR_FR =  2;
		public const int LANG_IT_IT =  3;

		public const  int LANG_COUNT = 4;
		public static int TEXT_COUNT = 0;

		public static int LANGUAGE = LANG_EN_US;

		public static string[,] Dictionary = new string[TEXT_COUNT, LANG_COUNT];

		public static void Init(int lang, int count)
		{
			LANGUAGE   = lang;
			TEXT_COUNT = count;

			Dictionary = new string[TEXT_COUNT, LANG_COUNT];
		}

		public static void Add(int id, params string[] data)
		{
			for (int i = 0; i < data.Length; i++)
			{
				Dictionary[id, i] = data[i];
			}
		}

		public static void Verify()
		{
			for (int t = 0; t < TEXT_COUNT; t++)
			{
				for (int l = 0; l < LANG_COUNT; l++)
				{
					if (string.IsNullOrWhiteSpace(Dictionary[t, l])) SAMLog.Error("L10N::Verify", $"Missing translation {t} for lang={l}");
				}
			}
		}

		public static string T(int id)
		{
			if (id < 0 || id >= TEXT_COUNT)
			{
				SAMLog.Error("L10N::T", $"Missing translation {id} for lang={LANGUAGE}");
				return $"\"{id}\"";
			}
			
			return Dictionary[id, LANGUAGE];
		}

		public static string TF(int id, object o1) => string.Format(T(id), o1);
		public static string TF(int id, object o1, object o2) => string.Format(T(id), o1, o2);
		public static string TF(int id, object o1, object o2, object o3) => string.Format(T(id), o1, o2 ,o3);
		public static string TF(int id, params object[] o) => string.Format(T(id), o);

		public static void ChangeLanguage(int lang)
		{
			LANGUAGE = lang;
		}
	}
}
