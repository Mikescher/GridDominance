using MonoSAMFramework.Portable.Persistance.DataFileFormat;
using System;
using System.Collections.Generic;

namespace MonoSAMFramework.Portable.Persistance.DataFile.PrimitiveWrapper
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
				writer.WriteFixedLengthNonEscapedASCII(v.Key.ToString("N").ToUpper(), 32);
				v.Value.Serialize(writer, currentVersion);
			}
		}

		public override void Deserialize(IDataReader reader, SemVersion archiveVersion)
		{
			int count = reader.ReadInteger();

			for (int i = 0; i < count; i++)
			{
				var key = Guid.ParseExact(reader.ReadFixedLengthNonEscapedASCII(32), "N");

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
