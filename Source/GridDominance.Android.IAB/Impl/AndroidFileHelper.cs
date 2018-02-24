using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using MonoSAMFramework.Portable.DeviceBridge;

namespace GridDominance.Android.Impl
{
	class AndroidFileHelper : FileHelper
	{
		public override void WriteData(string fileid, string data)
		{
			var store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);

			var fs = store.CreateFile(fileid);
			using (StreamWriter sw = new StreamWriter(fs))
			{
				sw.Write(data);
			}
		}

		public override string ReadDataOrNull(string fileid)
		{
			var store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);

			if (store.FileExists(fileid))
			{
				var fs = store.OpenFile(fileid, FileMode.Open);
				using (StreamReader sr = new StreamReader(fs))
				{
					return sr.ReadToEnd();
				}
			}
			else
			{
				return null;
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