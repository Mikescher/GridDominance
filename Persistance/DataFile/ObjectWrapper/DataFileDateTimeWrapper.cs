using System;
using MonoSAMFramework.Portable.Persistance.DataFileFormat;

namespace MonoSAMFramework.Portable.Persistance.DataFile.ObjectWrapper
{
	public class DataFileDateTimeWrapper : BaseDataFile
	{
		public const string TYPENAME = "WDATETIME";
		protected override string GetTypeName() => TYPENAME;

		public DateTime Value = DateTime.MinValue;

		public override void Serialize(IDataWriter writer, SemVersion currentVersion)
		{
			writer.WriteLong(Value.ToUniversalTime().Ticks);
		}

		public override void Deserialize(IDataReader reader, SemVersion archiveVersion)
		{
			Value  = new DateTime(reader.ReadLong(), DateTimeKind.Utc);
		}

		public static BaseDataFile Create(DateTime value)
		{
			return new DataFileDateTimeWrapper { Value = value};
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileDateTimeWrapper());
		}

		public static void RegisterIfNeeded()
		{
			if (!DataFileTypeInfo.ContainsType(TYPENAME))
				new DataFileDateTimeWrapper().RegisterTypeInfo(TYPENAME);
		}
	}
}
