﻿using MonoSAMFramework.Portable.DeviceBridge;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using MonoSAMFramework.Portable.Language;
using MonoSAMFramework.Portable.LogProtocol;
using Newtonsoft.Json;
using System.Linq;

namespace MonoSAMFramework.Portable.Network.REST
{
	public sealed class RestParameterSet
	{
		private const int MAX_GET_LENGTH     = 128;
		private const int MAX_GET_LENGTH_BIG = 2048;

		public enum RestParameterSetType { String, Int, Base64, Hash, Compressed, Guid, Json, BigCompressed }

		private readonly List<Tuple<string, string, RestParameterSetType, bool>> dict = new List<Tuple<string, string, RestParameterSetType, bool>>();

		public void AddParameterString(string name, string value, bool signed = true)
		{
			dict.Add(Tuple.Create(name, value, RestParameterSetType.String, signed));
		}
		
		public void AddParameterJson(string name, object value, bool signed = true)
		{
			dict.Add(Tuple.Create(name, JsonConvert.SerializeObject(value, Formatting.None), RestParameterSetType.Json, signed));
		}

		public void AddParameterBigCompressed(string name, string value, bool signed = true)
		{
			dict.Add(Tuple.Create(name, value, RestParameterSetType.BigCompressed, signed));
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

		public Tuple<string, FormUrlEncodedContent> CreateParamString(string secret)
		{
			var post = new List<KeyValuePair<string, string>>();
			
			var sigbuilder = secret;

			string result = "";
			foreach (var elem in dict)
			{
				var name = elem.Item1;
				var value = elem.Item2;
				var type = elem.Item3;
				var sign = elem.Item4;

				switch (type)
				{
					case RestParameterSetType.String:
					case RestParameterSetType.Json:
					case RestParameterSetType.Guid:
						if (sign) sigbuilder += "\n" + value;

						if (value.Length > MAX_GET_LENGTH)
							post.Add(new KeyValuePair<string, string>(name, value));
						else
							result += "&" + name + "=" + Uri.EscapeDataString(value);
						break;

					case RestParameterSetType.Int:
						if (sign) sigbuilder += "\n" + value;

						if (value.Length > MAX_GET_LENGTH)
							post.Add(new KeyValuePair<string, string>(name, value));
						else
							result += "&" + name + "=" + value;
						break;

					case RestParameterSetType.Base64:
						if (sign) sigbuilder += "\n" + value;

						var data64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(value))
							.Replace('+', '-')
							.Replace('\\', '_')
							.Replace('=', '.');

						if (value.Length > MAX_GET_LENGTH)
							post.Add(new KeyValuePair<string, string>(name, data64));
						else
							result += "&" + name + "=" + data64;
						break;

					case RestParameterSetType.Hash:
						var dataHash = value.ToUpper();
						if (sign) sigbuilder += "\n" + dataHash;

						if (value.Length > MAX_GET_LENGTH)
							post.Add(new KeyValuePair<string, string>(name, dataHash));
						else
							result += "&" + name + "=" + dataHash;
						break;

					case RestParameterSetType.Compressed:
						if (sign) sigbuilder += "\n" + value;

						var dataComp = CompressString(value)
							.Replace('+', '-')
							.Replace('\\', '_')
							.Replace('=', '.');
						if (dataComp.Length > MAX_GET_LENGTH)
							post.Add(new KeyValuePair<string, string>(name, dataComp));
						else
							result += "&" + name + "=" + dataComp;
						break;

					case RestParameterSetType.BigCompressed:
						if (sign) sigbuilder += "\n" + value;

						var dataComp2 = CompressString(value)
							.Replace('+', '-')
							.Replace('\\', '_')
							.Replace('=', '.');
						if (dataComp2.Length > MAX_GET_LENGTH_BIG)
							post.Add(new KeyValuePair<string, string>(name, dataComp2));
						else
							result += "&" + name + "=" + dataComp2;
						break;


					default:
						SAMLog.Error("RPS::EnumSwitch_CPS", "type = " + type);
						break;
				}
			}

			var sig = MonoSAMGame.CurrentInst.Bridge.DoSHA256(sigbuilder);

			var get = "?msgk=" + sig + result;

			var postContent = post.Any() ? new FormUrlEncodedContent(post) : null;

			return Tuple.Create(get, postContent);
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

		public string GetDebugInfo(string secret)
		{
			StringBuilder b = new StringBuilder();
			b.AppendLine();

			foreach (var elem in dict)
			{
				var name = elem.Item1;
				var value = elem.Item2;
				var type = elem.Item3;
				var sign = elem.Item4 ? "X" : " ";

				b.AppendLine($"[{sign}] entry[{type}|{name}] = {Convert.ToBase64String(Encoding.UTF8.GetBytes(value))}");
			}

			var sigbuilder = secret;
			foreach (var elem in dict)
			{
				var name = elem.Item1;
				var value = elem.Item2;
				var type = elem.Item3;
				var sign = elem.Item4;

				switch (type)
				{
					case RestParameterSetType.String:
					case RestParameterSetType.Json:
					case RestParameterSetType.Guid:
						if (sign) sigbuilder += "\n" + value;
						break;

					case RestParameterSetType.Int:
						if (sign) sigbuilder += "\n" + value;
						break;

					case RestParameterSetType.Base64:
						if (sign) sigbuilder += "\n" + value;
						break;

					case RestParameterSetType.Hash:
						var dataHash = value.ToUpper();
						if (sign) sigbuilder += "\n" + dataHash;
						break;

					case RestParameterSetType.Compressed:
						if (sign) sigbuilder += "\n" + value;
						break;

					case RestParameterSetType.BigCompressed:
						if (sign) sigbuilder += "\n" + value;
						break;

					default:
						break;
				}
			}

			var sig = MonoSAMGame.CurrentInst.Bridge.DoSHA256(sigbuilder);

			b.AppendLine();
			b.AppendLine($"sig = {sig}");
			b.AppendLine($"sigbuilder = \n{ByteUtils.CompressStringForStorage(sigbuilder)}");

			return b.ToString();
		}
	}
}
