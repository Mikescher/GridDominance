namespace MonoSAMFramework.Portable.FileHelper.Writer
{
	public interface IDataWriter
	{
		void WriteInteger(int v);
		void WriteASCII(string v);
		void WriteDouble(double v);
		void WriteBool(bool v);
	}
}
