using MonoSAMFramework.Portable.Persistance.DataFileFormat;

namespace MonoSAMFramework.Portable.Persistance.DataFile.PrimitiveWrapper
{
	public class DataFileIntWrapper : BaseDataFile
	{
		public const string TYPENAME = "WINT";
		protected override string GetTypeName() => TYPENAME;

		public int Value;

		public override void Serialize(IDataWriter writer, SemVersion currentVersion)
		{
			writer.WriteInteger(Value);
		}

		public override void Deserialize(IDataReader reader, SemVersion archiveVersion)
		{
			Value = reader.ReadInteger();
		}

		public static BaseDataFile Create(int value)
		{
			return new DataFileIntWrapper { Value = value};
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileIntWrapper());
		}

		public static void RegisterIfNeeded()
		{
			if (!DataFileTypeInfo.ContainsType(TYPENAME))
				new DataFileIntWrapper().RegisterTypeInfo(TYPENAME);
		}
	}
}
