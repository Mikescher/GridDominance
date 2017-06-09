using MonoSAMFramework.Portable.Persistance.DataFileFormat;
using System;

namespace MonoSAMFramework.Portable.Persistance.DataFile.PrimitiveWrapper
{
	public class DataFileGUIDWrapper : BaseDataFile
	{
		public const string TYPENAME = "WGUID";
		protected override string GetTypeName() => TYPENAME;

		public Guid Value;

		public override void Serialize(IDataWriter writer, SemVersion currentVersion)
		{
			writer.WriteUUID(Value);
		}

		public override void Deserialize(IDataReader reader, SemVersion archiveVersion)
		{
			try
			{
				Value = reader.ReadUUID();
			}
			catch (Exception e)
			{
				throw new SAMPersistanceException(e);
			}
		}

		public static BaseDataFile Create(Guid value)
		{
			return new DataFileGUIDWrapper { Value = value};
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileGUIDWrapper());
		}

		public static void RegisterIfNeeded()
		{
			if (!DataFileTypeInfo.ContainsType(TYPENAME))
				new DataFileGUIDWrapper().RegisterTypeInfo(TYPENAME);
		}
	}
}
