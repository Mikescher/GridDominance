using System.IO;
using System.IO.IsolatedStorage;
using MonoSAMFramework.Portable.DeviceBridge;

namespace GridDominance.UWP.Impl
{
	class WinPhoneFileHelper : FileHelper
	{
		public override void WriteData(string fileid, string data)
		{
			var store = IsolatedStorageFile.GetUserStoreForApplication();

			var fs = store.CreateFile(fileid);
			using (StreamWriter sw = new StreamWriter(fs))
			{
				sw.Write(data);
			}
		}

		public override string ReadDataOrNull(string fileid)
		{
			var store = IsolatedStorageFile.GetUserStoreForApplication();

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
	}
}