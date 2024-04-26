using MonoSAMFramework.Portable.Persistance.DataFileFormat;

namespace MonoSAMFramework.Portable.Persistance.DataFile.PrimitiveWrapper
{
	public class DataFileBoolWrapper : BaseDataFile
	{
		public const string TYPENAME = "WBOOL";
		protected override string GetTypeName() => TYPENAME;

		public bool Value;

		public override void Serialize(IDataWriter writer, SemVersion currentVersion)
		{
			writer.WriteBool(Value);
		}

		public override void Deserialize(IDataReader reader, SemVersion archiveVersion)
		{
			Value = reader.ReadBool();
		}

		public static BaseDataFile Create(bool value)
		{
			return new DataFileBoolWrapper { Value = value };
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileBoolWrapper());
		}

		public static void RegisterIfNeeded()
		{
			if (!DataFileTypeInfo.ContainsType(TYPENAME))
				new DataFileBoolWrapper().RegisterTypeInfo(TYPENAME);
		}
	}
}
