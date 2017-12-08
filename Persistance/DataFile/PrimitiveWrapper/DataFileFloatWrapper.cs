using MonoSAMFramework.Portable.Persistance.DataFileFormat;

namespace MonoSAMFramework.Portable.Persistance.DataFile.PrimitiveWrapper
{
	public class DataFileFloatWrapper : BaseDataFile
	{
		public const string TYPENAME = "WFLOAT";
		protected override string GetTypeName() => TYPENAME;

		public float Value;

		public override void Serialize(IDataWriter writer, SemVersion currentVersion)
		{
			writer.WriteDouble(Value);
		}

		public override void Deserialize(IDataReader reader, SemVersion archiveVersion)
		{
			Value = (float) reader.ReadDouble();
		}

		public static DataFileFloatWrapper Create(float value)
		{
			return new DataFileFloatWrapper { Value = value };
		}

		public static DataFileFloatWrapper Create() => Create(0);

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileFloatWrapper());
		}

		public static void RegisterIfNeeded()
		{
			if (!DataFileTypeInfo.ContainsType(TYPENAME))
				new DataFileFloatWrapper().RegisterTypeInfo(TYPENAME);
		}
	}
}
