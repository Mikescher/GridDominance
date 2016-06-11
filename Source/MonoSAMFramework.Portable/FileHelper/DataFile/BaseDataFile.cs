using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoSAMFramework.Portable.FileHelper.DataFile
{
	public abstract class BaseDataFile
	{
		private List<DataFileTypeInfoProperty> registerPropertyCollector;
		private Func<BaseDataFile> registerConstructorCollector;

		public virtual string Serialize()
		{
			var ownTypeInfo = GetTypeInfo();

			StringBuilder builder = new StringBuilder();

			builder.Append("{\n");
			foreach (var property in ownTypeInfo.Properties)
			{
				builder.Append($"{property.PropertyName}:{property.Getter(this).Serialize()}\n");
			}
			builder.Append("}");

			return builder.ToString();
		}

		public virtual void Deserialize(string data)
		{
			var ownTypeInfo = GetTypeInfo();

			var lines = data.Split('\n');
			if (lines.Length < 2 || lines.First() != "{" || lines.Last() != "}")
				throw new DeserializationException("Syntax error in propertyfield");

			for (int i = 1; i < lines.Length - 1; i++)
			{
				var parts = lines[i].Split(':');
				if (parts.Length != 2)
					throw new DeserializationException("Syntax error in propertyline");

				var prop = ownTypeInfo.FindProperty(parts[0]);

				if (prop == null)
					throw new DeserializationException("Invalid propertyname in propertyfield: '" + parts[0] + "'");

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

					prop.Setter(this, prop.CreateDeserialized(collector.ToString()));
				}
				else
				{
					prop.Setter(this, prop.CreateDeserialized(parts[1]));
				}
			}
		}

		#region Typeinfo Configuration

		public DataFileTypeInfo GetTypeInfo()
		{
			string typename = GetTypeName();

			if (!DataFileTypeInfo.ContainsType(typename))
			{
				RegisterTypeInfo(typename);
			}
			
			return DataFileTypeInfo.Get(typename);
		}

		public void RegisterTypeInfo(string typename)
		{
			registerPropertyCollector = new List<DataFileTypeInfoProperty>();
			registerConstructorCollector = null;

			Configure();

			if (registerConstructorCollector == null)
				throw new ArgumentException("No Serialization constructor specified");

			DataFileTypeInfo.Add(typename, new DataFileTypeInfo(typename, registerPropertyCollector, registerConstructorCollector));
		}

		protected abstract void Configure();
		protected abstract string GetTypeName();

		protected void RegisterConstructor(Func<BaseDataFile> ctr)
		{
			registerConstructorCollector = ctr;
		}

		protected void RegisterProperty<TThis, TProp>(string name, Func<TProp> ctr, Func<TThis, TProp> get, Action<TThis, TProp> set)
			where TThis : BaseDataFile
			where TProp : BaseDataFile
		{
			Func<BaseDataFile, BaseDataFile> g = o => get((TThis)o);
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, (TProp)v);
			
			var gen = ctr().GetTypeInfo();

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(name, gen.TypeName, g, s));
		}

		protected void RegisterProperty<TThis>(string name, Func<TThis, string> get, Action<TThis, string> set) 
			where TThis : BaseDataFile
		{
			Func<BaseDataFile, BaseDataFile> g = o => DataFileStringWrapper.Create(get((TThis)o));
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, ((DataFileStringWrapper)v).Value);

			DataFileStringWrapper.RegisterIfNeeded();

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(name, DataFileStringWrapper.TYPENAME, g, s));
		}

		protected void RegisterProperty<TThis>(string name, Func<TThis, int> get, Action<TThis, int> set) 
			where TThis : BaseDataFile
		{
			Func<BaseDataFile, BaseDataFile> g = o => DataFileIntWrapper.Create(get((TThis)o));
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, ((DataFileIntWrapper)v).Value);

			DataFileIntWrapper.RegisterIfNeeded();

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(name, DataFileIntWrapper.TYPENAME, g, s));
		}

		protected void RegisterProperty<TThis>(string name, Func<TThis, float> get, Action<TThis, float> set) 
			where TThis : BaseDataFile
		{
			Func<BaseDataFile, BaseDataFile> g = o => DataFileFloatWrapper.Create(get((TThis)o));
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, ((DataFileFloatWrapper)v).Value);

			DataFileFloatWrapper.RegisterIfNeeded();

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(name, DataFileFloatWrapper.TYPENAME, g, s));
		}

		protected void RegisterProperty<TThis>(string name, Func<TThis, double> get, Action<TThis, double> set) 
			where TThis : BaseDataFile
		{
			Func<BaseDataFile, BaseDataFile> g = o => DataFileDoubleWrapper.Create(get((TThis)o));
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, ((DataFileDoubleWrapper)v).Value);

			DataFileDoubleWrapper.RegisterIfNeeded();

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(name, DataFileDoubleWrapper.TYPENAME, g, s));
		}

		protected void RegisterProperty<TThis>(string name, Func<TThis, Guid> get, Action<TThis, Guid> set) 
			where TThis : BaseDataFile
		{
			Func<BaseDataFile, BaseDataFile> g = o => DataFileGUIDWrapper.Create(get((TThis)o));
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, ((DataFileGUIDWrapper)v).Value);

			DataFileGUIDWrapper.RegisterIfNeeded();

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(name, DataFileGUIDWrapper.TYPENAME, g, s));
		}

		protected void RegisterPropertyList<TThis, TElem>(string name, Func<TElem> ctr, Func<TThis, List<TElem>> get, Action<TThis, List<TElem>> set)
			where TThis : BaseDataFile
			where TElem : BaseDataFile
		{
			var gen = ctr().GetTypeInfo();

			Func<BaseDataFile, BaseDataFile> g = o => DataFileListWrapper<TElem>.Create(gen, get((TThis)o));
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, ((DataFileListWrapper<TElem>)v).Value);

			DataFileListWrapper<TElem>.RegisterIfNeeded(gen);

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(name, DataFileListWrapper<TElem>.GetTypeName(gen), g, s));
		}

		protected void RegisterPropertyStringDictionary<TThis, TElem>(string name, Func<TElem> ctr, Func<TThis, Dictionary<string, TElem>> get, Action<TThis, Dictionary<string, TElem>> set)
			where TThis : BaseDataFile
			where TElem : BaseDataFile
		{
			var gen = ctr().GetTypeInfo();

			Func<BaseDataFile, BaseDataFile> g = o => DataFileSDictWrapper<TElem>.Create(gen, get((TThis)o));
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, ((DataFileSDictWrapper<TElem>)v).Value);

			DataFileSDictWrapper<TElem>.RegisterIfNeeded(gen);

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(name, DataFileSDictWrapper<TElem>.GetTypeName(gen), g, s));
		}
			
		protected void RegisterPropertyEnumSet<TThis, TEnum>(string name, Func<TThis, HashSet<TEnum>> get, Action<TThis, HashSet<TEnum>> set)
			where TThis : BaseDataFile
			where TEnum : struct
		{
			Func<BaseDataFile, BaseDataFile> g = o => DataFileIntSetWrapper.CreateFromEnumSet(get((TThis)o));
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, ((DataFileIntSetWrapper)v).CastToEnumSet<TEnum>());

			DataFileIntSetWrapper.RegisterIfNeeded();

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(name, DataFileIntSetWrapper.TYPENAME, g, s));
		}

		#endregion
	}
}
