using MonoSAMFramework.Portable.Persistance.DataFileFormat;
using System;

namespace MonoSAMFramework.Portable.Persistance.DataFile.PrimitiveWrapper
{
	public class DataFileEnumWrapper<TEnum> : BaseDataFile where TEnum : struct, IComparable, IFormattable // IEnum
	{
		public static readonly string TYPENAME = $"WENUM<{typeof(TEnum).Name}>";
		protected override string GetTypeName() => TYPENAME;

		public TEnum Value;

		public override void Serialize(IDataWriter writer, SemVersion currentVersion)
		{
			writer.WriteInteger(Value.GetHashCode());
		}

		public override void Deserialize(IDataReader reader, SemVersion archiveVersion)
		{
			Value = (TEnum)(object)reader.ReadInteger();
		}

		public static DataFileEnumWrapper<TEnum> Create(TEnum value)
		{
			return new DataFileEnumWrapper<TEnum> { Value = value};
		}
		
		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileEnumWrapper<TEnum>());
		}

		public static void RegisterIfNeeded()
		{
			if (!DataFileTypeInfo.ContainsType(TYPENAME))
				new DataFileEnumWrapper<TEnum>().RegisterTypeInfo(TYPENAME);
		}
	}
}
