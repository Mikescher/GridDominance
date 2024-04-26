using MonoSAMFramework.Portable.Persistance.DataFileFormat;

namespace MonoSAMFramework.Portable.Persistance.DataFile.PrimitiveWrapper
{
	public class DataFileLongWrapper : BaseDataFile
	{
		public const string TYPENAME = "WLONG";
		protected override string GetTypeName() => TYPENAME;

		public long Value;

		public override void Serialize(IDataWriter writer, SemVersion currentVersion)
		{
			writer.WriteLong(Value);
		}

		public override void Deserialize(IDataReader reader, SemVersion archiveVersion)
		{
			Value = reader.ReadLong();
		}

		public static DataFileLongWrapper Create(long value)
		{
			return new DataFileLongWrapper { Value = value};
		}

		public static DataFileLongWrapper Create() => Create(0);

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileLongWrapper());
		}

		public static void RegisterIfNeeded()
		{
			if (!DataFileTypeInfo.ContainsType(TYPENAME))
				new DataFileLongWrapper().RegisterTypeInfo(TYPENAME);
		}
	}
}
