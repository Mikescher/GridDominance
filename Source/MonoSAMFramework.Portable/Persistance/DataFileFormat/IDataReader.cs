using System;

namespace MonoSAMFramework.Portable.Persistance.DataFileFormat
{
	public interface IDataReader
	{
		int ReadInteger();
		long ReadLong();
		string ReadString();
		float ReadFloat();
		double ReadDouble();
		bool ReadBool();
		SemVersion ReadVersion();
		byte ReadRawPrintableByte();
		string ReadFixedLengthNonEscapedASCII(int length);
		Guid ReadUUID();
	}
}
