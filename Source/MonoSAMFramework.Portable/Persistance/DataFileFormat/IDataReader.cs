using System;

namespace MonoSAMFramework.Portable.Persistance.DataFileFormat
{
	public interface IDataReader
	{
		int ReadInteger();
		string ReadString();
		double ReadDouble();
		bool ReadBool();
		SemVersion ReadVersion();
		byte ReadRawPrintableByte();
		string ReadFixedLengthNonEscapedASCII(int length);
		Guid ReadUUID();
	}
}
