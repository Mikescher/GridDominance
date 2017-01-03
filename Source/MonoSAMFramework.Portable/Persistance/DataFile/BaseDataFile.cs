using MonoSAMFramework.Portable.Persistance.DataFile.PrimitiveWrapper;
using MonoSAMFramework.Portable.Persistance.DataFileFormat;
using System;
using System.Collections.Generic;

namespace MonoSAMFramework.Portable.Persistance.DataFile
{
	public abstract class BaseDataFile
	{
		private List<DataFileTypeInfoProperty> registerPropertyCollector;
		private Func<BaseDataFile> registerConstructorCollector;

		public virtual void Serialize(IDataWriter writer, SemVersion currentVersion)
		{
			var ownTypeInfo = GetTypeInfo();
			
			foreach (var property in ownTypeInfo.Properties)
			{
				if (property.MinimalVersion.IsLaterThan(currentVersion))
					throw new SAMPersistanceException("Cannot serialize property from the future: " + property.PropertyName + " v"+property.MinimalVersion);

				property.Getter(this).Serialize(writer, currentVersion);
			}
		}

		public virtual void Deserialize(IDataReader reader, SemVersion archiveVersion)
		{
			var ownTypeInfo = GetTypeInfo();

			foreach (var property in ownTypeInfo.Properties)
			{
				if (property.MinimalVersion.IsLaterThan(archiveVersion))
				{
					// Skip deserialization - property did not exist in that time
				}
				else
				{
					var inst = property.Create();

					inst.Deserialize(reader, archiveVersion);

					property.Setter(this, inst);
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

		protected void RegisterProperty<TThis, TProp>(SemVersion version, string name, Func<TProp> ctr, Func<TThis, TProp> get, Action<TThis, TProp> set)
			where TThis : BaseDataFile
			where TProp : BaseDataFile
		{
			Func<BaseDataFile, BaseDataFile> g = o => get((TThis)o);
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, (TProp)v);
			
			var gen = ctr().GetTypeInfo();

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(version, name, gen.TypeName, g, s));
		}

		protected void RegisterProperty<TThis>(SemVersion version, string name, Func<TThis, string> get, Action<TThis, string> set) 
			where TThis : BaseDataFile
		{
			Func<BaseDataFile, BaseDataFile> g = o => DataFileStringWrapper.Create(get((TThis)o));
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, ((DataFileStringWrapper)v).Value);

			DataFileStringWrapper.RegisterIfNeeded();

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(version, name, DataFileStringWrapper.TYPENAME, g, s));
		}

		protected void RegisterProperty<TThis>(SemVersion version, string name, Func<TThis, int> get, Action<TThis, int> set) 
			where TThis : BaseDataFile
		{
			Func<BaseDataFile, BaseDataFile> g = o => DataFileIntWrapper.Create(get((TThis)o));
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, ((DataFileIntWrapper)v).Value);

			DataFileIntWrapper.RegisterIfNeeded();

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(version, name, DataFileIntWrapper.TYPENAME, g, s));
		}

		protected void RegisterProperty<TThis>(SemVersion version, string name, Func<TThis, float> get, Action<TThis, float> set) 
			where TThis : BaseDataFile
		{
			Func<BaseDataFile, BaseDataFile> g = o => DataFileFloatWrapper.Create(get((TThis)o));
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, ((DataFileFloatWrapper)v).Value);

			DataFileFloatWrapper.RegisterIfNeeded();

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(version, name, DataFileFloatWrapper.TYPENAME, g, s));
		}

		protected void RegisterProperty<TThis>(SemVersion version, string name, Func<TThis, double> get, Action<TThis, double> set) 
			where TThis : BaseDataFile
		{
			Func<BaseDataFile, BaseDataFile> g = o => DataFileDoubleWrapper.Create(get((TThis)o));
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, ((DataFileDoubleWrapper)v).Value);

			DataFileDoubleWrapper.RegisterIfNeeded();

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(version, name, DataFileDoubleWrapper.TYPENAME, g, s));
		}

		protected void RegisterProperty<TThis>(SemVersion version, string name, Func<TThis, Guid> get, Action<TThis, Guid> set) 
			where TThis : BaseDataFile
		{
			Func<BaseDataFile, BaseDataFile> g = o => DataFileGUIDWrapper.Create(get((TThis)o));
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, ((DataFileGUIDWrapper)v).Value);

			DataFileGUIDWrapper.RegisterIfNeeded();

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(version, name, DataFileGUIDWrapper.TYPENAME, g, s));
		}

		protected void RegisterProperty<TThis>(SemVersion version, string name, Func<TThis, bool> get, Action<TThis, bool> set)
			where TThis : BaseDataFile
		{
			Func<BaseDataFile, BaseDataFile> g = o => DataFileBoolWrapper.Create(get((TThis)o));
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, ((DataFileBoolWrapper)v).Value);

			DataFileBoolWrapper.RegisterIfNeeded();

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(version, name, DataFileStringWrapper.TYPENAME, g, s));
		}

		protected void RegisterPropertyList<TThis, TElem>(SemVersion version, string name, Func<TElem> ctr, Func<TThis, List<TElem>> get, Action<TThis, List<TElem>> set)
			where TThis : BaseDataFile
			where TElem : BaseDataFile
		{
			var gen = ctr().GetTypeInfo();

			Func<BaseDataFile, BaseDataFile> g = o => DataFileListWrapper<TElem>.Create(gen, get((TThis)o));
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, ((DataFileListWrapper<TElem>)v).Value);

			DataFileListWrapper<TElem>.RegisterIfNeeded(gen);

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(version, name, DataFileListWrapper<TElem>.GetTypeName(gen), g, s));
		}

		protected void RegisterPropertyStringDictionary<TThis, TElem>(SemVersion version, string name, Func<TElem> ctr, Func<TThis, Dictionary<string, TElem>> get, Action<TThis, Dictionary<string, TElem>> set)
			where TThis : BaseDataFile
			where TElem : BaseDataFile
		{
			var gen = ctr().GetTypeInfo();

			Func<BaseDataFile, BaseDataFile> g = o => DataFileSDictWrapper<TElem>.Create(gen, get((TThis)o));
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, ((DataFileSDictWrapper<TElem>)v).Value);

			DataFileSDictWrapper<TElem>.RegisterIfNeeded(gen);

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(version, name, DataFileSDictWrapper<TElem>.GetTypeName(gen), g, s));
		}
			
		protected void RegisterPropertyEnumSet<TThis, TEnum>(SemVersion version, string name, Func<TThis, HashSet<TEnum>> get, Action<TThis, HashSet<TEnum>> set)
			where TThis : BaseDataFile
			where TEnum : struct
		{
			Func<BaseDataFile, BaseDataFile> g = o => DataFileIntSetWrapper.CreateFromEnumSet(get((TThis)o));
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, ((DataFileIntSetWrapper)v).CastToEnumSet<TEnum>());

			DataFileIntSetWrapper.RegisterIfNeeded();

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(version, name, DataFileIntSetWrapper.TYPENAME, g, s));
		}

		protected void RegisterPropertyGuidictionary<TThis, TElem>(SemVersion version, string name, Func<TElem> ctr, Func<TThis, Dictionary<Guid, TElem>> get, Action<TThis, Dictionary<Guid, TElem>> set)
			where TThis : BaseDataFile
			where TElem : BaseDataFile
		{
			var gen = ctr().GetTypeInfo();

			Func<BaseDataFile, BaseDataFile> g = o => DataFileGDictWrapper<TElem>.Create(gen, get((TThis)o));
			Action<BaseDataFile, BaseDataFile> s = (o, v) => set((TThis)o, ((DataFileGDictWrapper<TElem>)v).Value);

			DataFileGDictWrapper<TElem>.RegisterIfNeeded(gen);

			registerPropertyCollector.Add(new DataFileTypeInfoProperty(version, name, DataFileGDictWrapper<TElem>.GetTypeName(gen), g, s));
		}

		#endregion
	}
}
