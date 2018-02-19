using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
