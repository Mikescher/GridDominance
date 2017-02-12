using MonoSAMFramework.Portable.DeviceBridge;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace MonoSAMFramework.Portable.Network.REST
{
	public sealed class RestParameterSet
	{
		public enum RestParameterSetType { String, Int, Base64, Hash, Compressed, Guid }

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

		public void AddParameterCompressed(string name, string value, bool signed = true)
		{
			dict[name] = Tuple.Create(value, RestParameterSetType.Compressed, signed);
		}

		public void AddParameterGuid(string name, Guid value, bool signed = true)
		{
			dict[name] = Tuple.Create(value.ToString("B"), RestParameterSetType.Guid, signed);
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
					case RestParameterSetType.Guid:
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

					case RestParameterSetType.Compressed:
						sigbuilder += "\n" + elem.Value.Item1;
						var dataComp = CompressString(elem.Value.Item1)
							.Replace('+', '-')
							.Replace('\\', '_')
							.Replace('=', '.');
						result += "&" + elem.Key + "=" + dataComp;
						break;
				}
			}

			var sig = MonoSAMGame.CurrentInst.Bridge.DoSHA256(sigbuilder);

			return "?msgk=" + sig + result;
		}


		public static string CompressString(string str)
		{
			using (var msi = new MemoryStream(Encoding.UTF8.GetBytes(str)))
			{
				using (var mso = new MemoryStream())
				{
					using (var gs = new DeflateStream(mso, CompressionMode.Compress))
					{
						byte[] buffer = new byte[4096];
						int cnt;
						while ((cnt = msi.Read(buffer, 0, buffer.Length)) != 0) gs.Write(buffer, 0, cnt);
					}
					return Convert.ToBase64String(mso.ToArray());
				}
			}
		}
	}
}
