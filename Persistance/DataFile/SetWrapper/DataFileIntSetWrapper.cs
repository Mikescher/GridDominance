using MonoSAMFramework.Portable.Persistance.DataFileFormat;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.Persistance.DataFile.PrimitiveWrapper
{
	public class DataFileIntSetWrapper : BaseDataFile
	{
		public const string TYPENAME = "WINTSET";
		protected override string GetTypeName() => TYPENAME;

		public HashSet<int> Value = new HashSet<int>();

		public override void Serialize(IDataWriter writer, SemVersion currentVersion)
		{
			writer.WriteInteger(Value.Count);
			foreach (var v in Value)
			{
				writer.WriteInteger(v);
			}
		}

		public override void Deserialize(IDataReader reader, SemVersion archiveVersion)
		{
			int count = reader.ReadInteger();

			for (int i = 0; i < count; i++)
			{
				Value.Add(reader.ReadInteger());
			}
		}

		public static BaseDataFile Create(HashSet<int> value)
		{
			return new DataFileIntSetWrapper { Value = new HashSet<int>(value)};
		}

		public static BaseDataFile CreateFromEnumSet<T>(HashSet<T> value) where T : struct
		{
			return new DataFileIntSetWrapper { Value = new HashSet<int>(value.Select(p => p.GetHashCode())) }; // abusing the fact that GetHasCode of an 32bit enum returns the enum value
		}

		public HashSet<T> CastToEnumSet<T>() where T : struct
		{
			return new HashSet<T>(Value.Select(p => (T)(object)p));
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileIntSetWrapper());
		}

		public static void RegisterIfNeeded()
		{
			if (!DataFileTypeInfo.ContainsType(TYPENAME))
				new DataFileIntSetWrapper().RegisterTypeInfo(TYPENAME);
		}
	}
}
