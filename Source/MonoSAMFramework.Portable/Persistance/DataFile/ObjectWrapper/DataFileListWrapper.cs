using System.Collections.Generic;
using MonoSAMFramework.Portable.Persistance.DataFileFormat;

namespace MonoSAMFramework.Portable.Persistance.DataFile.ObjectWrapper
{
	public class DataFileListWrapper<T> : BaseDataFile where T : BaseDataFile
	{
		private readonly string _typeName;
		private readonly DataFileTypeInfo _elemTypeInfo;
		protected override string GetTypeName() => _typeName;

		public List<T> Value = new List<T>();

		public DataFileListWrapper(DataFileTypeInfo elemTypeInfo)
		{
			_typeName = GetTypeName(elemTypeInfo);
			_elemTypeInfo = elemTypeInfo;
		}

		public override void Serialize(IDataWriter writer, SemVersion currentVersion)
		{
			writer.WriteInteger(Value.Count);
			foreach (var v in Value)
			{
				v.Serialize(writer, currentVersion);
			}
		}

		public override void Deserialize(IDataReader reader, SemVersion archiveVersion)
		{
			int count = reader.ReadInteger();

			for (int i = 0; i < count; i++)
			{
				var inst = _elemTypeInfo.Create();

				inst.Deserialize(reader, archiveVersion);

				Value.Add((T)inst);
			}
		}

		public static BaseDataFile Create(DataFileTypeInfo elemTypeInfo, List<T> value)
		{
			return new DataFileListWrapper<T>(elemTypeInfo) { Value = value};
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileListWrapper<T>(_elemTypeInfo));
		}

		public static void RegisterIfNeeded(DataFileTypeInfo elemTypeInfo)
		{
			if (!DataFileTypeInfo.ContainsType(GetTypeName(elemTypeInfo)))
				new DataFileListWrapper<T>(elemTypeInfo).RegisterTypeInfo(GetTypeName(elemTypeInfo));
		}

		public static string GetTypeName(DataFileTypeInfo elemTypeInfo)
		{
			return $"WLIST<{elemTypeInfo.TypeName}>";
		}
	}
}
