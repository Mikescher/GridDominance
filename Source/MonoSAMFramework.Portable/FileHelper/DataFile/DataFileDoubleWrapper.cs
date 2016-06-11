using System;
using System.Globalization;

namespace MonoSAMFramework.Portable.FileHelper.DataFile
{
	public class DataFileDoubleWrapper : BaseDataFile
	{
		public const string TYPENAME = "WDOUBLE";
		protected override string GetTypeName() => TYPENAME;

		public double Value;

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

		public static BaseDataFile Create(double value)
		{
			return new DataFileDoubleWrapper { Value = value};
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileDoubleWrapper());
		}

		public static void RegisterIfNeeded()
		{
			if (!DataFileTypeInfo.ContainsType(TYPENAME))
				new DataFileDoubleWrapper().RegisterTypeInfo(TYPENAME);
		}
	}
}
