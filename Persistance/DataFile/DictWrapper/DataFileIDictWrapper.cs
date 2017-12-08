using MonoSAMFramework.Portable.Persistance.DataFileFormat;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.Persistance.DataFile.PrimitiveWrapper
{
	public class DataFileIDictWrapper<T> : BaseDataFile where T : BaseDataFile
	{
		private readonly string _typeName;
		private readonly DataFileTypeInfo _elemTypeInfo;
		protected override string GetTypeName() => _typeName;

		public Dictionary<int, T> Value = new Dictionary<int, T>();

		public DataFileIDictWrapper(DataFileTypeInfo elemTypeInfo)
		{
			_typeName = GetTypeName(elemTypeInfo);
			_elemTypeInfo = elemTypeInfo;
		}

		public override void Serialize(IDataWriter writer, SemVersion currentVersion)
		{
			writer.WriteInteger(Value.Count);
			foreach (var v in Value)
			{
				writer.WriteInteger(v.Key);
				v.Value.Serialize(writer, currentVersion);
			}
		}

		public override void Deserialize(IDataReader reader, SemVersion archiveVersion)
		{
			int count = reader.ReadInteger();

			for (int i = 0; i < count; i++)
			{
				var key = reader.ReadInteger();

				var inst = _elemTypeInfo.Create();

				inst.Deserialize(reader, archiveVersion);

				Value[key] = (T)inst;
			}
		}

		public static BaseDataFile Create(DataFileTypeInfo elemTypeInfo, Dictionary<int, T> value)
		{
			return new DataFileIDictWrapper<T>(elemTypeInfo) { Value = value};
		}

		public static BaseDataFile CreateFromEnumDict<TEnum>(DataFileTypeInfo elemTypeInfo, Dictionary<TEnum, T> value) where TEnum: struct
		{
			return new DataFileIDictWrapper<T>(elemTypeInfo) { Value = new Dictionary<int, T>(value.ToDictionary(p => p.Key.GetHashCode(), q => q.Value)) }; // abusing the fact that GetHasCode of an 32bit enum returns the enum value
		}

		public Dictionary<TEnum, T> CastToEnumDict<TEnum>() where TEnum : struct
		{
			return new Dictionary<TEnum, T>(Value.ToDictionary(p => (TEnum)(object)p.Key, p => p.Value));
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileIDictWrapper<T>(_elemTypeInfo));
		}

		public static void RegisterIfNeeded(DataFileTypeInfo elemTypeInfo)
		{
			if (!DataFileTypeInfo.ContainsType(GetTypeName(elemTypeInfo)))
				new DataFileIDictWrapper<T>(elemTypeInfo).RegisterTypeInfo(GetTypeName(elemTypeInfo));
		}

		public static string GetTypeName(DataFileTypeInfo elemTypeInfo)
		{
			return $"WIDICT<{elemTypeInfo.TypeName}>";
		}
	}
}
