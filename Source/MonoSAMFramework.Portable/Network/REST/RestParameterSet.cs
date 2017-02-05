using MonoSAMFramework.Portable.DeviceBridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoSAMFramework.Portable.Network.REST
{
	public sealed class RestParameterSet
	{
		public enum RestParameterSetType { String, Int, Base64, Encrypted }

		private readonly Dictionary<string, Tuple<string, RestParameterSetType, bool>> dict = new Dictionary<string, Tuple<string, RestParameterSetType, bool>>();

		public void AddParameterString(string name, string value, bool signed = true)
		{
			dict[name] = Tuple.Create(value, RestParameterSetType.String, signed);
		}

		public void AddParameterInt(string name, int value, bool signed = true)
		{
			dict[name] = Tuple.Create(value.ToString(), RestParameterSetType.Int, signed);
		}

		public void AddParameterEncrypted(string name, string value, bool signed = true)
		{
			dict[name] = Tuple.Create(value, RestParameterSetType.Encrypted, signed);
		}

		public void AddParameterBase64(string name, string value, bool signed = true)
		{
			dict[name] = Tuple.Create(value, RestParameterSetType.Base64, signed);
		}

		public string CreateParamString(string secret, IRSAProvider rsa)
		{
			var sigbuilder = string.Join("\n", new[]{secret}.Concat(dict.Where(p => p.Value.Item3).Select(p => p.Value.Item1)));
			var sig = MonoSAMGame.CurrentInst.Bridge.DoSHA256(sigbuilder);

			string result = "?msgk=" + sig;
			foreach (var elem in dict)
			{
				switch (elem.Value.Item2)
				{
					case RestParameterSetType.String:
						result += "&" + elem.Key + "=" + Uri.EscapeDataString(elem.Value.Item1);
						break;
					case RestParameterSetType.Int:
						result += "&" + elem.Key + "=" + elem.Value.Item1;
						break;
					case RestParameterSetType.Base64:
						var data64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(elem.Value.Item1))
							.Replace('+', '-')
							.Replace('\\', '_')
							.Replace('=', '.');

						result += "&" + elem.Key + "=" + data64;
						break;
					case RestParameterSetType.Encrypted:
						var dataPPK = rsa.Encrypt(elem.Value.Item1)
							.Replace('+', '-')
							.Replace('\\', '_')
							.Replace('=', '.');

						result += "&" + elem.Key + "=" + dataPPK;

						break;
				}
			}
			return result;
		}
	}
}
