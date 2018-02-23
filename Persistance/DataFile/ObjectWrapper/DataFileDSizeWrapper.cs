using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Persistance.DataFileFormat;

namespace MonoSAMFramework.Portable.Persistance.DataFile.ObjectWrapper
{
	public class DataFileDSizeWrapper : BaseDataFile
	{
		public const string TYPENAME = "WSTRING";
		protected override string GetTypeName() => TYPENAME;

		public DSize Value = new DSize(0, 0);

		public override void Serialize(IDataWriter writer, SemVersion currentVersion)
		{
			writer.WriteInteger(Value.Width);
			writer.WriteInteger(Value.Height);
		}

		public override void Deserialize(IDataReader reader, SemVersion archiveVersion)
		{
			var ww = reader.ReadInteger();
			var hh = reader.ReadInteger();
			Value = new DSize(ww, hh);
		}

		public static BaseDataFile Create(DSize value)
		{
			return new DataFileDSizeWrapper { Value = value };
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileDSizeWrapper());
		}

		public static void RegisterIfNeeded()
		{
			if (!DataFileTypeInfo.ContainsType(TYPENAME))
				new DataFileDSizeWrapper().RegisterTypeInfo(TYPENAME);
		}
	}
}
