using MonoSAMFramework.Portable.LogProtocol;
using System.Collections.Generic;

namespace MonoSAMFramework.Portable.Localization
{
	public static class L10N
	{
		public const int LANG_NONE  = -1;
		public const int LANG_EN_US =  0;
		public const int LANG_DE_DE =  1;

		public static int LANGUAGE = -1;

		public static Dictionary<int, string[]> Dictionary = new Dictionary<int, string[]>();

		public static string T(int id)
		{
			string[] d;
			if (!Dictionary.TryGetValue(id, out d))
			{
				SAMLog.Error("L10N", $"Missing {LANGUAGE} translation for '{id}'");
				return $"\"{id}\"";
			}

			return d[LANGUAGE];
		}

		public static string TF(int id, object o1) => string.Format(T(id), o1);
		public static string TF(int id, object o1, object o2) => string.Format(T(id), o1, o2);
		public static string TF(int id, object o1, object o2, object o3) => string.Format(T(id), o1, o2 ,o3);
		public static string TF(int id, params object[] o) => string.Format(T(id), o);
	}
}
