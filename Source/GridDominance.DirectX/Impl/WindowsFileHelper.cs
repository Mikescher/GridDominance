using System.Collections.Generic;
using MonoSAMFramework.Portable.DeviceBridge;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;

namespace GridDominance.Windows
{
	class WindowsFileHelper : FileHelper
	{
		public override void WriteData(string fileid, string data)
		{
			var store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);

			using (var fs = store.CreateFile(fileid))
			{
				using (StreamWriter sw = new StreamWriter(fs))
				{
					sw.Write(data);
				}
			}
		}

		public override void WriteBinData(string fileid, byte[] data)
		{
			var store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);

			using (var fs = store.CreateFile(fileid))
			{
				fs.Write(data, 0, data.Length);
			}
		}

		public override string ReadDataOrNull(string fileid)
		{
			var store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);

			if (store.FileExists(fileid))
			{
				using (var fs = store.OpenFile(fileid, FileMode.Open))
				{
					using (StreamReader sr = new StreamReader(fs))
					{
						return sr.ReadToEnd();
					}
				}
			}
			else
			{
				return null;
			}
		}

		public override byte[] ReadBinDataOrNull(string fileid)
		{
			var store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);

			if (!store.FileExists(fileid)) return null;

			using (var fs = store.OpenFile(fileid, FileMode.Open))
			{
				using (MemoryStream ms = new MemoryStream())
				{
					fs.CopyTo(ms);
					return ms.ToArray();
				}
			}
		}

		public override bool DeleteDataIfExist(string fileid)
		{
			var store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);

			if (store.FileExists(fileid))
			{
				store.DeleteFile(fileid);
				return true;
			}
			else
			{
				return false;
			}
		}

		public override List<string> ListData()
		{
			var store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);

			return store.GetFileNames().ToList();
		}
	}
}
