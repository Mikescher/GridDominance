using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoSAMFramework.Portable.FileHelper.DataFile
{
	public class DataFileSDictWrapper<T> : BaseDataFile where T : BaseDataFile
	{
		private readonly string _typeName;
		private readonly DataFileTypeInfo _elemTypeInfo;
		protected override string GetTypeName() => _typeName;

		public Dictionary<string, T> Value = new Dictionary<string, T>();

		public DataFileSDictWrapper(DataFileTypeInfo elemTypeInfo)
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
				builder.Append($"{SerializeKey(v.Key)}:{v.Value.Serialize()}\n");
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
				var parts = lines[i].Split(':');
				if (parts.Length != 2)
					throw new DeserializationException("Syntax error in dictionaryline");

				if (parts[1].StartsWith("{"))
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

					Value.Add(DeserializeKey(parts[0]), (T)_elemTypeInfo.CreateDeserialized(collector.ToString()));
				}
				else
				{
					Value.Add(DeserializeKey(parts[0]), (T)_elemTypeInfo.CreateDeserialized(parts[1]));
				}

			}
		}


		private string SerializeKey(string key)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
		}

		private string DeserializeKey(string data)
		{
			try
			{
				var enc = Convert.FromBase64String(data);

				return Encoding.UTF8.GetString(enc, 0, enc.Length);
			}
			catch (Exception e)
			{
				throw new DeserializationException(e);
			}
		}

		public static BaseDataFile Create(DataFileTypeInfo elemTypeInfo, Dictionary<string, T> value)
		{
			return new DataFileSDictWrapper<T>(elemTypeInfo) { Value = value};
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new DataFileSDictWrapper<T>(_elemTypeInfo));
		}

		public static void RegisterIfNeeded(DataFileTypeInfo elemTypeInfo)
		{
			if (!DataFileTypeInfo.ContainsType(GetTypeName(elemTypeInfo)))
				new DataFileSDictWrapper<T>(elemTypeInfo).RegisterTypeInfo(GetTypeName(elemTypeInfo));
		}

		public static string GetTypeName(DataFileTypeInfo elemTypeInfo)
		{
			return $"WSDICT<{elemTypeInfo.TypeName}>";
		}
	}
}
