using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;

namespace MonoSAMFramework.Portable.Persistance
{
	public class XConfigFile
	{
		private Dictionary<string, string> _configdata = new Dictionary<string, string>();

		private XConfigFile()
		{
			
		}

		public static XConfigFile LoadFromString(string data)
		{
			var xcf = new XConfigFile();

			foreach (var rawline in Regex.Split(data, @"\r?\n"))
			{
				var line = rawline.Trim();

				if (line.StartsWith("#") || line == "") continue;

				var stmt_ident = rawline.Substring(0, rawline.IndexOf('=')).Trim().ToLower();
				var stmt_data = rawline.Substring(rawline.IndexOf('=')+1).Trim();

				if (stmt_data.EndsWith(",")) stmt_data = stmt_data.Substring(0, stmt_data.Length - 1);

				xcf._configdata[stmt_ident] = stmt_data;
			}

			return xcf;
		}

		public string GetString(string ident)
		{
			return _configdata[ident.ToLower()];
		}

		public int GetInt(string ident)
		{
			return int.Parse(GetString(ident));
		}

		public float GetFloat(string ident)
		{
			var str = GetString(ident);
			if (str.EndsWith("f") || str.EndsWith("d")) str = str.Substring(0, str.Length - 1);

			return float.Parse(str, CultureInfo.InvariantCulture);
		}

		public bool Contains(string ident)
		{
			return _configdata.ContainsKey(ident.ToLower());
		}

		public Color GetKnownColor(string ident)
		{
			return ColorMath.COLOR_MAP[GetString(ident).ToLower()];
		}
	}
}
