using MonoSAMFramework.Portable.MathHelper.Cryptography;
using System.Text;

namespace MonoSAMFramework.Portable.Persistance.DataFileFormat
{
	public static class PersistanceHelper
	{
		public const int INTEGRITY_HASH_LEN = 8;

		public static string UnescapeString(string raw)
		{
			StringBuilder unesc = new StringBuilder(raw.Length);

			for (int i = 0; i < raw.Length; i++)
			{
				if (raw[i] == '\\')
				{
					if (i + 1 >= raw.Length)
						throw new DataWriterException("Unexpected EOF found");

					char ec0 = raw[i + 1];

					if (ec0 == '\\')
					{
						// escaped backslash
						unesc.Append('\\');
					}
					else if (ec0 == '_')
					{
						// 4 nibble UTF
						if (i + 5 >= raw.Length)
							throw new DataWriterException("Unexpected EOF found");

						char rep0 = raw[i + 2];
						char rep1 = raw[i + 3];
						char rep2 = raw[i + 4];
						char rep3 = raw[i + 5];

						unesc.Append(GetCharFromNibbles(rep0, rep1, rep2, rep3));

						i += 5;
					}
					else
					{
						// 2 nibble ASCII 
						if (i + 2 >= raw.Length)
							throw new DataWriterException("Unexpected EOF found");

						char ec1 = raw[i + 2];

						unesc.Append(GetCharFromNibbles('0', '0', ec0, ec1));

						i += 2;
					}
				}
				else
				{
					unesc.Append(raw[i]);
				}
			}

			return unesc.ToString();
		}

		public static string EscapeString(string str)
		{
			StringBuilder esc = new StringBuilder();

			foreach (char chr in str)
			{
				if (chr == '\\')
				{
					esc.Append(@"\\");
				}
				else if (chr < 32)
				{
					esc.Append(string.Format("\\{0:X2}", (int)chr));
				}
				else if (chr <= '~')
				{
					esc.Append(chr);
				}
				else if (chr < 256)
				{
					esc.Append(string.Format("\\{0:X2}", (int)chr));
				}
				else
				{
					esc.Append(string.Format("\\_{0:X4}", (int)chr));
				}
			}

			return esc.ToString();
		}

		public static string CreateFileIntegrityHash(string data)
		{
			return MD5.GetHashString(data).Substring(0, INTEGRITY_HASH_LEN);
		}

		private static char GetCharFromNibbles(char n0, char n1, char n2, char n3)
		{
			int v = 
				(GetNibbleValue(n0) << 0xC) |
				(GetNibbleValue(n1) << 0x8) |
				(GetNibbleValue(n2) << 0x4) |
				(GetNibbleValue(n3) << 0x0);

			return (char) v;
		}

		private static int GetNibbleValue(char nibble)
		{
			if (nibble >= '0' && nibble <= '9')
				return nibble - '0';
			else if (nibble >= 'A' && nibble <= 'F')
				return 10 + nibble - 'A';
			else
				throw new DataWriterException("the nibble chr(" + (int)nibble + ") is not valid");
		}
	}
}
