using System;
using MonoSAMFramework.Portable.Persistance.DataFileFormat;
using System.Collections.Generic;

namespace MonoSAMFramework.Portable.Persistance.DataFile.PrimitiveWrapper
{
	public class DataFileGuidSetWrapper : BaseDataFile
	{
		public const string TYPENAME = "WGUIDSET";
		protected override string GetTypeName() => TYPENAME;

		public HashSet<Guid> Value = new HashSet<Guid>();

		public override void Serialize(IDataWriter writer, SemVersion currentVersion)
		{
			writer.WriteInteger(Value.Count);
			foreach (var v in Value)
			{
				writer.WriteUUID(v);
			}
		}

		public override void Deserialize(IDataReader reader, SemVersion archiveVersion)
		{
			int count = reader.ReadInteger();

			for (int i = 0; i < count; i++)
			{
				Value.Add(reader.ReadUUID());
			}
		}

		public static BaseDataFile Create(HashSet<Guid> value)
		{
			return new DataFileGuidSetWrapper { Value = new HashSet<Guid>(value)};
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileGuidSetWrapper());
		}

		public static void RegisterIfNeeded()
		{
			if (!DataFileTypeInfo.ContainsType(TYPENAME))
				new DataFileGuidSetWrapper().RegisterTypeInfo(TYPENAME);
		}
	}
}
