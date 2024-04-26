using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Persistance.DataFileFormat;

namespace MonoSAMFramework.Portable.Persistance.DataFile.ObjectWrapper
{
	public class DataFileFPointWrapper : BaseDataFile
	{
		public const string TYPENAME = "WFPOINT";
		protected override string GetTypeName() => TYPENAME;

		public FPoint Value = FPoint.NaN;

		public override void Serialize(IDataWriter writer, SemVersion currentVersion)
		{
			writer.WriteFloat(Value.X);
			writer.WriteFloat(Value.Y);
		}

		public override void Deserialize(IDataReader reader, SemVersion archiveVersion)
		{
			var xx = reader.ReadFloat();
			var yy = reader.ReadFloat();
			Value  = new FPoint(xx, yy);
		}

		public static BaseDataFile Create(FPoint value)
		{
			return new DataFileFPointWrapper { Value = value};
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileFPointWrapper());
		}

		public static void RegisterIfNeeded()
		{
			if (!DataFileTypeInfo.ContainsType(TYPENAME))
				new DataFileFPointWrapper().RegisterTypeInfo(TYPENAME);
		}
	}
}
