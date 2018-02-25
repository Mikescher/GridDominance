using System.Collections.Generic;

namespace MonoSAMFramework.Portable.DeviceBridge
{
	//see https://github.com/SupSuper/MonoGame-SaveManager 
	public abstract class FileHelper
	{
		public abstract void WriteData(string fileid, string data);
		public abstract void WriteBinData(string fileid, byte[] data);
		public abstract string ReadDataOrNull(string fileid);
		public abstract byte[] ReadBinDataOrNull(string fileid);
		public abstract bool DeleteDataIfExist(string fileid);
		public abstract List<string> ListData();

		public static FileHelper Inst { get; private set; }

		public static void RegisterSystemSecificHandler(FileHelper h)
		{
			Inst = h;
		}
	}
}
