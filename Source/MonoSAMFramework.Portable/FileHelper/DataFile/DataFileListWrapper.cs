using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoSAMFramework.Portable.FileHelper.DataFile
{
	public class DataFileListWrapper<T> : BaseDataFile where T : BaseDataFile
	{
		private readonly string _typeName;
		private readonly DataFileTypeInfo _elemTypeInfo;
		protected override string GetTypeName() => _typeName;

		public List<T> Value = new List<T>();

		public DataFileListWrapper(DataFileTypeInfo elemTypeInfo)
		{
			_typeName = GetTypeName(elemTypeInfo);
			_elemTypeInfo = elemTypeInfo;
		}

		public override string Serialize()
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("{\n");
			foreach (var v in Value)
			{
				builder.Append(v.Serialize() + "\n");
			}
			builder.Append("}");

			return builder.ToString();
		}

		public override void Deserialize(string data)
		{
			var lines = data.Split('\n');
			if (lines.Length < 2 || lines.First() != "{" || lines.Last() != "}")
				throw new DeserializationException("Syntax error in list");

			for (int i = 1; i < lines.Length - 1; i++)
			{
				int braceDepth = 1;
				StringBuilder collector = new StringBuilder();
				collector.Append("{");
				while (braceDepth > 0)
				{
					i++;
					if (i == lines.Length - 1)
						throw new DeserializationException("Non matching dyck language");

					if (lines[i].Contains("{"))
						braceDepth++;
					else if (lines[i].Contains("}"))
						braceDepth--;

					collector.Append("\n" + lines[i]);
				}

				Value.Add((T)_elemTypeInfo.CreateDeserialized(collector.ToString()));
			}
		}

		public static BaseDataFile Create(DataFileTypeInfo elemTypeInfo, List<T> value)
		{
			return new DataFileListWrapper<T>(elemTypeInfo) { Value = value};
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileListWrapper<T>(_elemTypeInfo));
		}

		public static void RegisterIfNeeded(DataFileTypeInfo elemTypeInfo)
		{
			if (!DataFileTypeInfo.ContainsType(GetTypeName(elemTypeInfo)))
				new DataFileListWrapper<T>(elemTypeInfo).RegisterTypeInfo(GetTypeName(elemTypeInfo));
		}

		public static string GetTypeName(DataFileTypeInfo elemTypeInfo)
		{
			return $"WLIST<{elemTypeInfo.TypeName}>";
		}
	}
}
