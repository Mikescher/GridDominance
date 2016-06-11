using System;
using System.Text;

namespace MonoSAMFramework.Portable.FileHelper.DataFile
{
	public class DataFileStringWrapper : BaseDataFile
	{
		public const string TYPENAME = "WSTRING";
		protected override string GetTypeName() => TYPENAME;

		public string Value = "";

		public override string Serialize()
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(Value));
		}

		public override void Deserialize(string data)
		{
			try
			{
				var enc = Convert.FromBase64String(data);

				Value = Encoding.UTF8.GetString(enc, 0, enc.Length);
			}
			catch (Exception e)
			{
				throw new DeserializationException(e);
			}
		}

		public static BaseDataFile Create(string value)
		{
			return new DataFileStringWrapper{Value = value};
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileStringWrapper());
		}

		public static void RegisterIfNeeded()
		{
			if (!DataFileTypeInfo.ContainsType(TYPENAME))
				new DataFileStringWrapper().RegisterTypeInfo(TYPENAME);
		}
	}
}
