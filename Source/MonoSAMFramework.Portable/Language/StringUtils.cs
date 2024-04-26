using System.Text;

namespace MonoSAMFramework.Portable.Language
{
	public static class StringUtils
	{
		public static string BytesToString(Encoding enc, byte[] arr)
		{
			return enc.GetString(arr, 0, arr.Length);
		}
	}
}
