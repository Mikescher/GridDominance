using System;
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
			data = data.Replace('-', '0');
			builder.Append(data);
		}

		public void WriteUnsignedInteger32(int v)
		{
			var data = v.ToString();
			WriteSimpleUnsignedInteger(data.Length - 1, 1);
			builder.Append(data);
		}

		public void WriteString(string v)
		{
			var mode = PersistanceHelper.GetStringSerializationMode(v);
			WriteSimpleUnsignedInteger(mode, 1);
			WriteInteger(v.Length);

			if (mode == 0)
			{
				foreach (var chr in v) WriteSimpleUnsignedInteger(chr - 32, 2);
			}
			else if (mode == 1)
			{
				foreach (var chr in v) WriteFixedLengthNonEscapedASCII(string.Format("{0:X2}", (int)chr), 2);
			}
			else if (mode == 2)
			{
				foreach (var chr in v) WriteUnsignedInteger32(chr);
			}
			else if (mode == 3)
			{
				foreach (var chr in v) WriteUnsignedInteger32(chr);
			}
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

		public void WriteUUID(Guid g)
		{
			var str = g.ToString("N");
			foreach (var chr in str)
			{
				if (chr >= '0' && chr <= '8')
					builder.Append(chr);
				else if (chr == '9')
					builder.Append("90");
				else
					builder.Append("9" + ((chr - 'a') + 1));
			}
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