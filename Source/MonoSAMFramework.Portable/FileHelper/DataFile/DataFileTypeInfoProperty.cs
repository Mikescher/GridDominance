using System;

namespace MonoSAMFramework.Portable.FileHelper.DataFile
{
	public class DataFileTypeInfoProperty
	{
		public readonly string PropertyName;
		public readonly string TypeName;

		public readonly Func<BaseDataFile, BaseDataFile> Getter;
		public readonly Action<BaseDataFile, BaseDataFile> Setter;

		public DataFileTypeInfoProperty(string n, string t, Func<BaseDataFile, BaseDataFile> g, Action<BaseDataFile, BaseDataFile> s)
		{
			PropertyName = n;
			TypeName = t;
			Getter = g;
			Setter = s;
		}

		public BaseDataFile CreateDeserialized(string data)
		{
			return DataFileTypeInfo.Get(TypeName).CreateDeserialized(data);
		}
	}
}
