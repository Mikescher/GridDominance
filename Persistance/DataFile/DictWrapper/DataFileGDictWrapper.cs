using System;
using System.Collections.Generic;
using MonoSAMFramework.Portable.Persistance.DataFileFormat;

namespace MonoSAMFramework.Portable.Persistance.DataFile.DictWrapper
{
	public class DataFileGDictWrapper<T> : BaseDataFile where T : BaseDataFile
	{
		private readonly string _typeName;
		private readonly DataFileTypeInfo _elemTypeInfo;
		protected override string GetTypeName() => _typeName;

		public Dictionary<Guid, T> Value = new Dictionary<Guid, T>();

		public DataFileGDictWrapper(DataFileTypeInfo elemTypeInfo)
		{
			_typeName = GetTypeName(elemTypeInfo);
			_elemTypeInfo = elemTypeInfo;
		}

		public override void Serialize(IDataWriter writer, SemVersion currentVersion)
		{
			writer.WriteInteger(Value.Count);
			foreach (var v in Value)
			{
				writer.WriteUUID(v.Key);
				v.Value.Serialize(writer, currentVersion);
			}
		}

		public override void Deserialize(IDataReader reader, SemVersion archiveVersion)
		{
			int count = reader.ReadInteger();

			for (int i = 0; i < count; i++)
			{
				var key = reader.ReadUUID();

				var inst = _elemTypeInfo.Create();

				inst.Deserialize(reader, archiveVersion);

				Value[key] = (T)inst;
			}
		}

		public static BaseDataFile Create(DataFileTypeInfo elemTypeInfo, Dictionary<Guid, T> value)
		{
			return new DataFileGDictWrapper<T>(elemTypeInfo) { Value = value};
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileGDictWrapper<T>(_elemTypeInfo));
		}

		public static void RegisterIfNeeded(DataFileTypeInfo elemTypeInfo)
		{
			if (!DataFileTypeInfo.ContainsType(GetTypeName(elemTypeInfo)))
				new DataFileGDictWrapper<T>(elemTypeInfo).RegisterTypeInfo(GetTypeName(elemTypeInfo));
		}

		public static string GetTypeName(DataFileTypeInfo elemTypeInfo)
		{
			return $"WGDICT<{elemTypeInfo.TypeName}>";
		}
	}
}
