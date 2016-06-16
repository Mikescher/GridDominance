namespace MonoSAMFramework.Portable.FileHelper.Writer
{
	public interface IDataReader
	{
		int ReadInteger();
		string ReadASCII();
		double ReadDouble();
		bool ReadBool();
	}
}
