using System;

namespace MonoSAMFramework.Portable.FileHelper.DataFile
{
	public class DataFileIntWrapper : BaseDataFile
	{
		public const string TYPENAME = "WINT";
		protected override string GetTypeName() => TYPENAME;

		public int Value;

		public override string Serialize()
		{
			return Convert.ToString(Value);
		}

		public override void Deserialize(string data)
		{
			try
			{
				Value = int.Parse(data);
			}
			catch (Exception e)
			{
				throw new DeserializationException(e);
			}
		}

		public static BaseDataFile Create(int value)
		{
			return new DataFileIntWrapper { Value = value};
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileIntWrapper());
		}

		public static void RegisterIfNeeded()
		{
			if (!DataFileTypeInfo.ContainsType(TYPENAME))
				new DataFileIntWrapper().RegisterTypeInfo(TYPENAME);
		}
	}
}
