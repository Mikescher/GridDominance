using MonoSAMFramework.Portable.DeviceBridge;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using MonoSAMFramework.Portable.LogProtocol;
using Newtonsoft.Json;

namespace MonoSAMFramework.Portable.Network.REST
{
	public sealed class RestParameterSet
	{
		private const int MAX_GET_LENGTH = 128;
		
		public enum RestParameterSetType { String, Int, Base64, Hash, Compressed, Guid, Json }

		private readonly List<Tuple<string, string, RestParameterSetType, bool>> dict = new List<Tuple<string, string, RestParameterSetType, bool>>();

		public void AddParameterString(string name, string value, bool signed = true)
		{
			dict.Add(Tuple.Create(name, value, RestParameterSetType.String, signed));
		}
		
		public void AddParameterJson(string name, object value, bool signed = true)
		{
			dict.Add(Tuple.Create(name, JsonConvert.SerializeObject(value, Formatting.None), RestParameterSetType.Json, signed));
		}

		public void AddParameterInt(string name, int value, bool signed = true)
		{
			dict.Add(Tuple.Create(name, value.ToString(), RestParameterSetType.Int, signed));
		}

		public void AddParameterHash(string name, string value, bool signed = true)
		{
			dict.Add(Tuple.Create(name, value, RestParameterSetType.Hash, signed));
		}

		public void AddParameterBase64(string name, string value, bool signed = true)
		{
			dict.Add(Tuple.Create(name, value, RestParameterSetType.Base64, signed));
		}

		public void AddParameterCompressed(string name, string value, bool signed = true)
		{
			dict.Add(Tuple.Create(name, value, RestParameterSetType.Compressed, signed));
		}

		public void AddParameterGuid(string name, Guid value, bool signed = true)
		{
			dict.Add(Tuple.Create(name, value.ToString("B"), RestParameterSetType.Guid, signed));
		}

		public Tuple<string, MultipartFormDataContent> CreateParamString(string secret)
		{
			var post = new MultipartFormDataContent();
			
			var sigbuilder = secret;

			string result = "";
			foreach (var elem in dict)
			{
				var name = elem.Item1;
				var value = elem.Item2;
				var type = elem.Item3;
				var sign = elem.Item4;

				if (!sign) continue;

				switch (type)
				{
					case RestParameterSetType.String:
					case RestParameterSetType.Json:
					case RestParameterSetType.Guid:
						sigbuilder += "\n" + value;
						if (value.Length > MAX_GET_LENGTH)
							post.Add(new StringContent(value), name);
						else
							result += "&" + name + "=" + Uri.EscapeDataString(value);
						break;

					case RestParameterSetType.Int:
						sigbuilder += "\n" + value;
						if (value.Length > MAX_GET_LENGTH)
							post.Add(new StringContent(value), name);
						else
							result += "&" + name + "=" + value;
						break;

					case RestParameterSetType.Base64:
						sigbuilder += "\n" + value;
						var data64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(value))
							.Replace('+', '-')
							.Replace('\\', '_')
							.Replace('=', '.');

						if (value.Length > MAX_GET_LENGTH)
							post.Add(new StringContent(data64), name);
						else
							result += "&" + name + "=" + data64;
						break;

					case RestParameterSetType.Hash:
						var dataHash = value.ToUpper();
						sigbuilder += "\n" + dataHash;
						if (value.Length > MAX_GET_LENGTH)
							post.Add(new StringContent(dataHash), name);
						else
							result += "&" + name + "=" + dataHash;
						break;

					case RestParameterSetType.Compressed:
						sigbuilder += "\n" + value;
						var dataComp = CompressString(value)
							.Replace('+', '-')
							.Replace('\\', '_')
							.Replace('=', '.');
						if (value.Length > MAX_GET_LENGTH)
							post.Add(new StringContent(dataComp), name);
						else
							result += "&" + name + "=" + dataComp;
						break;

					default:
						SAMLog.Error("RPS::EnumSwitch_CPS", "type = " + type);
						break;
				}
			}

			var sig = MonoSAMGame.CurrentInst.Bridge.DoSHA256(sigbuilder);

			var get = "?msgk=" + sig + result;

			return Tuple.Create(get, post);
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
