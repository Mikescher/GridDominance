using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.FileHelper.DataFile
{
	public class DataFileIntSetWrapper : BaseDataFile
	{
		public const string TYPENAME = "WINTSET";
		protected override string GetTypeName() => TYPENAME;

		public HashSet<int> Value = new HashSet<int>();

		public override string Serialize()
		{
			return string.Join("|", Value);
		}

		public override void Deserialize(string data)
		{
			var split = data.Split('|');

			foreach (var number in split)
			{
				try
				{
					Value.Add(int.Parse(number));
				}
				catch (Exception e)
				{
					throw new DeserializationException(e);
				}
			}
		}

		public static BaseDataFile Create<T>(HashSet<int> value) where T : struct
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
