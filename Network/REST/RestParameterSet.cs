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

		public enum RestParameterSetType { String, Primitive, Base64, Hash, StringCompressed, Guid, Json, JsonBigCompressed, BigCompressed, BinaryCompressed }

		private struct PEntry
		{
			public readonly string Name;
			public readonly string Data;
			public readonly byte[] DataBin;
			public readonly RestParameterSetType ParamType;
			public readonly bool Signed;
			public readonly bool ForcePost;

			public PEntry(string n, string d, RestParameterSetType t, bool s, bool fp)
			{
				Name = n;
				Data = d;
				DataBin = null;
				ParamType = t;
				Signed = s;
				ForcePost = fp;
			}

			public PEntry(string n, byte[] d, RestParameterSetType t, bool s, bool fp)
			{
				Name = n;
				Data = null;
				DataBin = d;
				ParamType = t;
				Signed = s;
				ForcePost = fp;
			}
		}

		private readonly List<PEntry> dict = new List<PEntry>();

		public void AddParameterString(string name, string value, bool signed = true, bool forcePost = false)
		{
			dict.Add(new PEntry(name, value, RestParameterSetType.String, signed, forcePost));
		}
		
		public void AddParameterJson(string name, object value, bool signed = true, bool forcePost = false)
		{
			dict.Add(new PEntry(name, JsonConvert.SerializeObject(value, Formatting.None), RestParameterSetType.Json, signed, forcePost));
		}

		public void AddParameterJsonBigCompressed(string name, object value, bool signed = true, bool forcePost = false)
		{
			dict.Add(new PEntry(name, JsonConvert.SerializeObject(value, Formatting.None), RestParameterSetType.JsonBigCompressed, signed, forcePost));
		}

		public void AddParameterBigCompressed(string name, string value, bool signed = true, bool forcePost = false)
		{
			dict.Add(new PEntry(name, value, RestParameterSetType.BigCompressed, signed, forcePost));
		}

		public void AddParameterInt(string name, int value, bool signed = true, bool forcePost = false)
		{
			dict.Add(new PEntry(name, value.ToString(), RestParameterSetType.Primitive, signed, forcePost));
		}

		public void AddParameterLong(string name, long value, bool signed = true, bool forcePost = false)
		{
			dict.Add(new PEntry(name, value.ToString(), RestParameterSetType.Primitive, signed, forcePost));
		}

		public void AddParameterULong(string name, ulong value, bool signed = true, bool forcePost = false)
		{
			dict.Add(new PEntry(name, value.ToString(), RestParameterSetType.Primitive, signed, forcePost));
		}

		public void AddParameterBool(string name, bool value, bool signed = true, bool forcePost = false)
		{
			dict.Add(new PEntry(name, (value?"true":"false").ToString(), RestParameterSetType.Primitive, signed, forcePost));
		}

		public void AddParameterHash(string name, string value, bool signed = true, bool forcePost = false)
		{
			dict.Add(new PEntry(name, value, RestParameterSetType.Hash, signed, forcePost));
		}

		public void AddParameterBase64(string name, string value, bool signed = true, bool forcePost = false)
		{
			dict.Add(new PEntry(name, value, RestParameterSetType.Base64, signed, forcePost));
		}

		public void AddParameterCompressed(string name, string value, bool signed = true, bool forcePost = false)
		{
			dict.Add(new PEntry(name, value, RestParameterSetType.StringCompressed, signed, forcePost));
		}

		public void AddParameterGuid(string name, Guid value, bool signed = true, bool forcePost = false)
		{
			dict.Add(new PEntry(name, value.ToString("B"), RestParameterSetType.Guid, signed, forcePost));
		}

		public void AddParameterCompressedBinary(string name, byte[] value, bool signed = true, bool forcePost = false)
		{
			dict.Add(new PEntry(name, value, RestParameterSetType.BinaryCompressed, signed, forcePost));
		}

		public Tuple<string, HttpContent> CreateParamString(string secret)
		{
			var post = new List<KeyValuePair<string, string>>();
			
			var sigbuilder = secret;

			string result = "";
			foreach (var elem in dict)
			{
				switch (elem.ParamType)
				{
					case RestParameterSetType.String:
					case RestParameterSetType.Json:
					case RestParameterSetType.Guid:
						if (elem.Signed) sigbuilder += "\n" + elem.Data;

						if (elem.ForcePost || elem.Data.Length > MAX_GET_LENGTH)
							post.Add(new KeyValuePair<string, string>(elem.Name, elem.Data));
						else
							result += "&" + elem.Name + "=" + Uri.EscapeDataString(elem.Data);
						break;

					case RestParameterSetType.Primitive:
						if (elem.Signed) sigbuilder += "\n" + elem.Data;

						if (elem.ForcePost)
							post.Add(new KeyValuePair<string, string>(elem.Name, elem.Data));
						else
							result += "&" + elem.Name + "=" + elem.Data;
						break;

					case RestParameterSetType.Base64:
						if (elem.Signed) sigbuilder += "\n" + elem.Data;

						var data64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(elem.Data))
							.Replace('+', '-')
							.Replace('\\', '_')
							.Replace('=', '.');

						if (elem.ForcePost || elem.Data.Length > MAX_GET_LENGTH)
							post.Add(new KeyValuePair<string, string>(elem.Name, data64));
						else
							result += "&" + elem.Name + "=" + data64;
						break;

					case RestParameterSetType.Hash:
						var dataHash = elem.Data.ToUpper();
						if (elem.Signed) sigbuilder += "\n" + dataHash;

						if (elem.ForcePost || elem.Data.Length > MAX_GET_LENGTH)
							post.Add(new KeyValuePair<string, string>(elem.Name, dataHash));
						else
							result += "&" + elem.Name + "=" + dataHash;
						break;

					case RestParameterSetType.StringCompressed:
						if (elem.Signed) sigbuilder += "\n" + elem.Data;

						var dataComp = CompressString(elem.Data)
							.Replace('+', '-')
							.Replace('\\', '_')
							.Replace('=', '.');
						if (elem.ForcePost || dataComp.Length > MAX_GET_LENGTH)
							post.Add(new KeyValuePair<string, string>(elem.Name, dataComp));
						else
							result += "&" + elem.Name + "=" + dataComp;
						break;

					case RestParameterSetType.JsonBigCompressed:
					case RestParameterSetType.BigCompressed:
						if (elem.Signed) sigbuilder += "\n" + elem.Data;

						var dataComp2 = CompressString(elem.Data)
							.Replace('+', '-')
							.Replace('\\', '_')
							.Replace('=', '.');
						if (elem.ForcePost || dataComp2.Length > MAX_GET_LENGTH_BIG)
							post.Add(new KeyValuePair<string, string>(elem.Name, dataComp2));
						else
							result += "&" + elem.Name + "=" + dataComp2;
						break;

					case RestParameterSetType.BinaryCompressed:
						if (elem.Signed) sigbuilder += "\n" + ByteUtils.ByteToHexBitFiddle(elem.DataBin);

						var dataComp3 = CompressBinary(elem.DataBin)
							.Replace('+', '-')
							.Replace('\\', '_')
							.Replace('=', '.');
						if (elem.ForcePost || dataComp3.Length > MAX_GET_LENGTH_BIG)
							post.Add(new KeyValuePair<string, string>(elem.Name, dataComp3));
						else
							result += "&" + elem.Name + "=" + dataComp3;
						break;


					default:
						SAMLog.Error("RPS::EnumSwitch_CPS", "type = " + elem.ParamType);
						break;
				}
			}

			var sig = MonoSAMGame.CurrentInst.Bridge.DoSHA256(sigbuilder);
			
			var get = "?msgk=" + sig + result;

			if (post.Any())
			{
				get = get + "&xsampostredirect=" + "("+string.Join(";", post.Select(p => p.Key+":"+p.Value.Length))+")";
			}

			var postContent = post.Any() ? CreatePostData(post) : null;

			return Tuple.Create(get, postContent);
		}

		private static HttpContent CreatePostData(List<KeyValuePair<string, string>> data)
		{
			var content = new MultipartFormDataContent();
			foreach (var keyValuePair in data)
			{
				content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
			}
			return content;
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

		public static string CompressBinary(byte[] raw)
		{
			using (var msi = new MemoryStream(raw))
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
				b.AppendLine($"[{(elem.Signed ? "X" : " ")}] entry[{elem.ParamType}|{elem.Name}] = {Convert.ToBase64String(Encoding.UTF8.GetBytes(elem.Data))}");
			}

			var sigbuilder = secret;
			foreach (var elem in dict)
			{
				switch (elem.ParamType)
				{
					case RestParameterSetType.String:
					case RestParameterSetType.Json:
					case RestParameterSetType.Guid:
						if (elem.Signed) sigbuilder += "\n" + elem.Data;
						break;

					case RestParameterSetType.Primitive:
						if (elem.Signed) sigbuilder += "\n" + elem.Data;
						break;

					case RestParameterSetType.Base64:
						if (elem.Signed) sigbuilder += "\n" + elem.Data;
						break;

					case RestParameterSetType.Hash:
						var dataHash = elem.Data.ToUpper();
						if (elem.Signed) sigbuilder += "\n" + dataHash;
						break;

					case RestParameterSetType.StringCompressed:
						if (elem.Signed) sigbuilder += "\n" + elem.Data;
						break;

					case RestParameterSetType.JsonBigCompressed:
					case RestParameterSetType.BigCompressed:
						if (elem.Signed) sigbuilder += "\n" + elem.Data;
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
