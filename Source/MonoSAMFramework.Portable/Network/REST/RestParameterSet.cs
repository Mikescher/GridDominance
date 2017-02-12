using MonoSAMFramework.Portable.DeviceBridge;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoSAMFramework.Portable.Network.REST
{
	public sealed class RestParameterSet
	{
		public enum RestParameterSetType { String, Int, Base64, Hash }

		private readonly Dictionary<string, Tuple<string, RestParameterSetType, bool>> dict = new Dictionary<string, Tuple<string, RestParameterSetType, bool>>();

		public void AddParameterString(string name, string value, bool signed = true)
		{
			dict[name] = Tuple.Create(value, RestParameterSetType.String, signed);
		}

		public void AddParameterInt(string name, int value, bool signed = true)
		{
			dict[name] = Tuple.Create(value.ToString(), RestParameterSetType.Int, signed);
		}

		public void AddParameterHash(string name, string value, bool signed = true)
		{
			dict[name] = Tuple.Create(value, RestParameterSetType.Hash, signed);
		}

		public void AddParameterBase64(string name, string value, bool signed = true)
		{
			dict[name] = Tuple.Create(value, RestParameterSetType.Base64, signed);
		}

		public void AddParameterGuid(string name, Guid value, bool signed = true)
		{
			dict[name] = Tuple.Create(value.ToString("B"), RestParameterSetType.String, signed);
		}

		public string CreateParamString(string secret, IOperatingSystemBridge bridge)
		{
			var sigbuilder = secret;
			
			string result = "";
			foreach (var elem in dict)
			{
				switch (elem.Value.Item2)
				{
					case RestParameterSetType.String:
						sigbuilder += "\n" + elem.Value.Item1;
						result += "&" + elem.Key + "=" + Uri.EscapeDataString(elem.Value.Item1);
						break;
					case RestParameterSetType.Int:
						sigbuilder += "\n" + elem.Value.Item1;
						result += "&" + elem.Key + "=" + elem.Value.Item1;
						break;
					case RestParameterSetType.Base64:
						sigbuilder += "\n" + elem.Value.Item1;
						var data64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(elem.Value.Item1))
							.Replace('+', '-')
							.Replace('\\', '_')
							.Replace('=', '.');

						result += "&" + elem.Key + "=" + data64;
						break;
					case RestParameterSetType.Hash:
						var dataHash = elem.Value.Item1.ToUpper();

						sigbuilder += "\n" + dataHash;

						result += "&" + elem.Key + "=" + dataHash;

						break;
				}
			}

			var sig = MonoSAMGame.CurrentInst.Bridge.DoSHA256(sigbuilder);

			return "?msgk=" + sig + result;
		}
	}
}
