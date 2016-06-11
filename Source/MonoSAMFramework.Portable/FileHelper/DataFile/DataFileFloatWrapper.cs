using System;
using System.Globalization;

namespace MonoSAMFramework.Portable.FileHelper.DataFile
{
	public class DataFileFloatWrapper : BaseDataFile
	{
		public const string TYPENAME = "WFLOAT";
		protected override string GetTypeName() => TYPENAME;

		public float Value;

		public override string Serialize()
		{
			return Value.ToString(CultureInfo.InvariantCulture);
		}

		public override void Deserialize(string data)
		{
			try
			{
				Value = float.Parse(data, CultureInfo.InvariantCulture);
			}
			catch (Exception e)
			{
				throw new DeserializationException(e);
			}
		}

		public static BaseDataFile Create(float value)
		{
			return new DataFileFloatWrapper { Value = value};
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileFloatWrapper());
		}

		public static void RegisterIfNeeded()
		{
			if (!DataFileTypeInfo.ContainsType(TYPENAME))
				new DataFileFloatWrapper().RegisterTypeInfo(TYPENAME);
		}
	}
}
