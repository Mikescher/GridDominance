using MonoSAMFramework.Portable.Persistance.DataFileFormat;

namespace MonoSAMFramework.Portable.Persistance.DataFile.PrimitiveWrapper
{
	public class DataFileStringWrapper : BaseDataFile
	{
		public const string TYPENAME = "WSTRING";
		protected override string GetTypeName() => TYPENAME;

		public string Value = "";

		public override void Serialize(IDataWriter writer, SemVersion currentVersion)
		{
			writer.WriteString(Value);
		}

		public override void Deserialize(IDataReader reader, SemVersion archiveVersion)
		{
			Value = reader.ReadString();
		}

		public static BaseDataFile Create(string value)
		{
			return new DataFileStringWrapper{Value = value};
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileStringWrapper());
		}

		public static void RegisterIfNeeded()
		{
			if (!DataFileTypeInfo.ContainsType(TYPENAME))
				new DataFileStringWrapper().RegisterTypeInfo(TYPENAME);
		}
	}
}
