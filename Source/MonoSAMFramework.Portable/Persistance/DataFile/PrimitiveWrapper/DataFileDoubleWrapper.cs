using MonoSAMFramework.Portable.Persistance.DataFileFormat;

namespace MonoSAMFramework.Portable.Persistance.DataFile.PrimitiveWrapper
{
	public class DataFileDoubleWrapper : BaseDataFile
	{
		public const string TYPENAME = "WDOUBLE";
		protected override string GetTypeName() => TYPENAME;

		public double Value;

		public override void Serialize(IDataWriter writer, SemVersion currentVersion)
		{
			writer.WriteDouble(Value);
		}

		public override void Deserialize(IDataReader reader, SemVersion archiveVersion)
		{
			Value = reader.ReadDouble();
		}

		public static BaseDataFile Create(double value)
		{
			return new DataFileDoubleWrapper { Value = value};
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileDoubleWrapper());
		}

		public static void RegisterIfNeeded()
		{
			if (!DataFileTypeInfo.ContainsType(TYPENAME))
				new DataFileDoubleWrapper().RegisterTypeInfo(TYPENAME);
		}
	}
}
