using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MonoSAMFramework.Portable.Language
{
	public static class ByteUtils
	{
		public static string CompressBytesForStorage(byte[] bytes)
		{
			var compressed = CompressionUtils.Compress(bytes);

			var str = Convert.ToBase64String(compressed);

			var split = string.Join("\n", GetChunks(str, 120));

			return split;
		}

		public static byte[] DecompressBytesFromStorage(string input)
		{
			var str = input.Replace("\r", "").Replace("\n", "");

			var compressed = Convert.FromBase64String(str);

			var bytes = CompressionUtils.Decompress(compressed);

			return bytes;
		}

		public static string CompressStringForStorage(string str)
		{
			return CompressBytesForStorage(Encoding.UTF8.GetBytes(str));
		}

		public static string DecompressStringFromStorage(string str)
		{
			var b = DecompressBytesFromStorage(str);
			return Encoding.UTF8.GetString(b, 0, b.Length);
		}

		public static IEnumerable<string> GetChunks(string sourceString, int chunkLength)
		{
			using (var sr = new StringReader(sourceString))
			{
				var buffer = new char[chunkLength];
				int read;
				while ((read = sr.Read(buffer, 0, chunkLength)) > 0)
				{
					yield return new string(buffer, 0, read);
				}
			}
		}

		// http://stackoverflow.com/a/14333437/1761622
		public static string ByteToHexBitFiddle(byte[] bytes) // uppercase
		{
			if (bytes == null) return "{{NULL}}";
			char[] c = new char[bytes.Length * 2];
			int b;
			for (int i = 0; i < bytes.Length; i++)
			{
				b = bytes[i] >> 4;
				c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
				b = bytes[i] & 0xF;
				c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
			}
			return new string(c);
		}

		// http://stackoverflow.com/a/14333437/1761622
		public static string ByteToHexBitFiddleLowercase(byte[] bytes)
		{
			if (bytes == null) return "{{NULL}}";
			char[] c = new char[bytes.Length * 2];
			int b;
			for (int i = 0; i < bytes.Length; i++)
			{
				b = bytes[i] >> 4;
				c[i * 2] = (char)(87 + b + (((b - 10) >> 31) & -39));
				b = bytes[i] & 0xF;
				c[i * 2 + 1] = (char)(87 + b + (((b - 10) >> 31) & -39));
			}
			return new string(c);
		}

		// https://stackoverflow.com/a/9995303/1761622
		public static byte[] StringToByteArray(string hex)
		{
			if (hex.Length % 2 == 1)
				throw new Exception("The binary key cannot have an odd number of digits");

			byte[] arr = new byte[hex.Length >> 1];

			for (int i = 0; i < (hex.Length >> 1); ++i)
			{
				arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
			}

			return arr;
		}

		public static int GetHexVal(char hex)
		{
			int val = (int)hex;
			return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
		}
	}
}
