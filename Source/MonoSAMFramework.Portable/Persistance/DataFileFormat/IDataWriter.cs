namespace MonoSAMFramework.Portable.Persistance.DataFileFormat
{
	public interface IDataWriter
	{
		void WriteInteger(int v);
		void WriteString(string v);
		void WriteDouble(double v);
		void WriteBool(bool v);
		void WriteVersion(SemVersion v);
		void WriteRawPrintableByte(byte v);
		void WriteFixedLengthNonEscapedASCII(string s, int length);
	}
}
