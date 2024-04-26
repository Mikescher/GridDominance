using System;

namespace MonoSAMFramework.Portable.Persistance.DataFile
{
	public class DataFileTypeInfoProperty
	{
		public readonly string PropertyName;
		public readonly string TypeName;
		public readonly SemVersion MinimalVersion; // did not exist before this version

		public readonly Func<BaseDataFile, BaseDataFile> Getter;
		public readonly Action<BaseDataFile, BaseDataFile> Setter;

		public DataFileTypeInfoProperty(SemVersion v, string n, string t, Func<BaseDataFile, BaseDataFile> g, Action<BaseDataFile, BaseDataFile> s)
		{
			MinimalVersion = v;
			PropertyName = n;
			TypeName = t;
			Getter = g;
			Setter = s;
		}

		public BaseDataFile Create()
		{
			return DataFileTypeInfo.Get(TypeName).Create();
		}
	}
}
