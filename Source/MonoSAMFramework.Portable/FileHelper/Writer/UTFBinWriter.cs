using System.Globalization;
using System.Text;

namespace MonoSAMFramework.Portable.FileHelper.Writer
{
	public class UTFBinWriter : IDataWriter
	{
		private StringBuilder builder;

		public UTFBinWriter()
		{
			builder = new StringBuilder();
		}

		public void WriteInteger(int v)
		{
			var data = v.ToString();
			WriteSimpleInteger(data.Length, 2);
			builder.Append(data);
		}

		public void WriteASCII(string v)
		{
			var proc = EscapeASCII(v);

			WriteInteger(proc.Length);
			builder.Append(proc);
		}

		public void WriteDouble(double v)
		{
			var str = v.ToString("r", CultureInfo.InvariantCulture);

			WriteSimpleInteger(str.Length, 2);
			builder.Append(str);
		}

		public void WriteBool(bool v)
		{
			builder.Append(v ? "1" : "0");
		}

		private void WriteSimpleInteger(int v, int len)
		{
			var data = v.ToString().PadLeft(len, '0');

			if (data.Length > len) throw new DataWriterException("cant write simple integer of length " + len + " with value " + v);

			builder.Append(data);
		}

		private string EscapeASCII(string str)
		{
			StringBuilder esc = new StringBuilder();

			foreach (var chr in str)
			{
				if (chr == '\\')
				{
					esc.Append("\\5C");
				}
				else if (chr < 32)
				{
					esc.Append(string.Format("\\{0:X2}", (int)chr));
				}
				else if (chr > '~')
				{
					throw new DataWriterException("the character " + (int) chr + " is not valid for serialization");
				}
				else
				{
					esc.Append(chr);
				}
			}

			return esc.ToString();
		}
	}
}