using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.Persistance.DataFile
{
	public class DataFileTypeInfo
	{
		private static readonly Dictionary<string, DataFileTypeInfo> typeInfoCache = new Dictionary<string, DataFileTypeInfo>();
		
		public readonly string TypeName;
		public readonly IReadOnlyList<DataFileTypeInfoProperty> Properties;
		public readonly Func<BaseDataFile> Constructor;

		public DataFileTypeInfo(string t, IEnumerable<DataFileTypeInfoProperty> p, Func<BaseDataFile> c)
		{
			TypeName = t;
			Properties = p.ToList();
			Constructor = c;
		}

		public static bool ContainsType(string t)
		{
			return typeInfoCache.ContainsKey(t);
		}

		public static DataFileTypeInfo Get(string t)
		{
			return typeInfoCache[t];
		}

		public static void Add(string t, DataFileTypeInfo i)
		{
			typeInfoCache[t] = i;
		}

		public DataFileTypeInfoProperty FindProperty(string pname)
		{
			return Properties.FirstOrDefault(p => p.PropertyName == pname);
		}

		public BaseDataFile Create()
		{
			return Constructor();
		}
	}
}
