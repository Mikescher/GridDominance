using System;

namespace MonoSAMFramework.Portable.FileHelper.DataFile
{
	public class DataFileGUIDWrapper : BaseDataFile
	{
		public const string TYPENAME = "WGUID";
		protected override string GetTypeName() => TYPENAME;

		public Guid Value;

		public override string Serialize()
		{
			return Value.ToString("D");
		}

		public override void Deserialize(string data)
		{
			try
			{
				Value = Guid.ParseExact(data, "D");
			}
			catch (Exception e)
			{
				throw new DeserializationException(e);
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
