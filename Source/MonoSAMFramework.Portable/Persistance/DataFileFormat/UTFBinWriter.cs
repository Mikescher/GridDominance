using System.Globalization;
using System.Text;

namespace MonoSAMFramework.Portable.Persistance.DataFileFormat
{
	public class UTFBinWriter : IDataWriter
	{
		private readonly StringBuilder builder;

		public UTFBinWriter()
		{
			builder = new StringBuilder();
		}

		public void WriteInteger(int v)
		{
			if (v == 0)
			{
				WriteSimpleUnsignedInteger(0, 2);
				return;
			}

			var data = v.ToString();
			WriteSimpleUnsignedInteger(data.Length, 2);
			builder.Append(data);
		}

		public void WriteString(string v)
		{
			var proc = PersistanceHelper.EscapeString(v);

			WriteInteger(proc.Length);
			builder.Append(proc);
		}

		public void WriteDouble(double v)
		{
			var str = v.ToString("r", CultureInfo.InvariantCulture);

			WriteSimpleUnsignedInteger(str.Length, 2);
			builder.Append(str);
		}

		public void WriteBool(bool v)
		{
			builder.Append(v ? "1" : "0");
		}

		public void WriteVersion(SemVersion v)
		{
			WriteSimpleUnsignedInteger(v.Mayor, 5);
			WriteSimpleUnsignedInteger(v.Minor, 5);
			WriteSimpleUnsignedInteger(v.Patch, 5);
		}

		public void WriteRawPrintableByte(byte v)
		{
			builder.Append((char)v);
		}

		public void WriteFixedLengthNonEscapedASCII(string s, int length)
		{
			if (s.Length != length)
				throw new DataWriterException($"The given string {s} has not the given fixed length {length}");

			builder.Append(s);
		}

		private void WriteSimpleUnsignedInteger(int v, int len)
		{
			var data = v.ToString().PadLeft(len, '0');

			if (data.Length > len) throw new DataWriterException($"cant write simple integer of length {len} with value {v}");

			builder.Append(data);
		}

		public string ToFormattedOutput(string header, int columnLength)
		{
			var rawData = header + builder;

			int lines = 1 + rawData.Length / columnLength;

			StringBuilder result = new StringBuilder(lines * columnLength);

			for (int i = 0; i < lines; i++)
			{
				if (i + 1 < lines)
				{
					result.Append(rawData.Substring(i * columnLength, columnLength));
					result.Append('\n');
				}
				else
				{
					result.Append(rawData.Substring(i * columnLength, rawData.Length - i * columnLength));
				}
			}

			return result.ToString();
		}

		public string GetHashOfCurrentState()
		{
			return PersistanceHelper.CreateFileIntegrityHash(builder.ToString());
		}
	}
}