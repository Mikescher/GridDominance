using System.Globalization;
using System.Text;

namespace MonoSAMFramework.Portable.FileHelper.Writer
{
	public class UTFBinReader : IDataReader
	{
		private readonly int datalength; // next char
		private readonly string data;
		private int position; // next char

		public UTFBinReader(string input)
		{
			data = input;
			datalength = input.Length;
			position = 0;
		}

		public int ReadInteger()
		{
			var length = ReadSimpleInteger(2);

			return ReadSimpleInteger(length);
		}

		private int ReadSimpleInteger(int length)
		{
			int r = 0;

			for (int i = 0; i < length; i++)
			{
				if (position >= datalength)
					throw new DataWriterException("Unexpected EOF found");

				char chr = data[position];

				if (chr < '0' || chr > '9')
					throw new DataWriterException("the character chr(" + (int)chr + ") is not a digit for SimpleInteger deserialization");

				r = r * 10 + (chr - '0');
				position++;
			}

			return r;
		}

		public string ReadASCII()
		{
			int len = ReadInteger();

			string raw = ReadRawString(len);

			return UnescapeASCII(raw);
		}

		public double ReadDouble()
		{
			var len = ReadSimpleInteger(2);

			var raw = ReadRawString(len);

			double r;
			if (double.TryParse(raw, NumberStyles.None, CultureInfo.InvariantCulture, out r))
				return r;

			throw new DataWriterException("the string '" + raw + "' is not a valid value for Double deserialization");
		}

		public bool ReadBool()
		{
			if (position >= datalength)
				throw new DataWriterException("Unexpected EOF found");

			char chr = data[position];
			position++;

			if (chr == '0')
				return false;
			if (chr == '1')
				return true;

			throw new DataWriterException("the character chr(" + (int)chr + ") is not a valid value for Boolean deserialization");
		}

		private string ReadRawString(int len)
		{
			if (position >= datalength)
				throw new DataWriterException("Unexpected EOF found");
			if (position + len > datalength)
				throw new DataWriterException("Unexpected EOF found");

			string r = data.Substring(position, len);

			position += len;

			return r;
		}

		private string UnescapeASCII(string raw)
		{
			StringBuilder unesc = new StringBuilder(raw.Length);

			for (int i = 0; i < raw.Length; i++)
			{
				if (raw[i] == '\\')
				{
					if (i + 2 >= raw.Length)
						throw new DataWriterException("Unexpected EOF found");

					char msn = raw[i + 1];
					char lsn = raw[i + 1];

					int msv;
					int lsv;

					if (msn >= '0' && msn <= '9')
						msv = msn - '0';
					else if (msn >= 'A' && msn <= 'F')
						msv = 10 + msn - 'A';
					else
						throw new DataWriterException("the character chr(" + (int)msn + ") is not a valid value for ASCII Unescape most-significant-nibble");

					if (lsn >= '0' && lsn <= '9')
						lsv = lsn - '0';
					else if (lsn >= 'A' && lsn <= 'F')
						lsv = 10 + lsn - 'A';
					else
						throw new DataWriterException("the character chr(" + (int)lsn + ") is not a valid value for ASCII Unescape lease-significant-nibble");

					unesc.Append((char) ((msv << 4) | lsv));

					i += 2;
				}
				else
				{
					unesc.Append(raw[i]);
				}
			}

			return unesc.ToString();
		}
	}
}
