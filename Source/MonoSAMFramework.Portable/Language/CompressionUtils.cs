using System.IO;
using System.IO.Compression;

namespace MonoSAMFramework.Portable.Language
{
	public static class CompressionUtils
	{
		public static byte[] Compress(byte[] data)
		{
			using (var ids = new MemoryStream())
			{
				using (var gzs = new GZipStream(ids, CompressionMode.Compress))
				{
					gzs.Write(data, 0, data.Length);
				}
				return ids.ToArray();
			}
		}

		public static byte[] Decompress(byte[] data)
		{
			using (var cms = new MemoryStream(data))
			{
				using (var dms = new MemoryStream())
				{
					using (var gzs = new GZipStream(cms, CompressionMode.Decompress))
					{
						gzs.CopyTo(dms);
					}
					return dms.ToArray();
				}
			}
		}
	}
}
