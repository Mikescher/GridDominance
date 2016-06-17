using MonoSAMFramework.Portable.Persistance.DataFileFormat;

namespace MonoSAMFramework.Portable.Persistance.DataFile
{
	public abstract class RootDataFile : BaseDataFile
	{
		private static readonly byte[] HEADER = {0x24, 0x3D}; //  $=

		protected abstract SemVersion ArchiveVersion { get; }

		public string SerializeToString()
		{
			var writer = new UTFBinWriter();

			writer.WriteRawPrintableByte(HEADER[0]);
			writer.WriteRawPrintableByte(HEADER[1]);

			writer.WriteVersion(ArchiveVersion);

			Serialize(writer, ArchiveVersion);

			return writer.ToFormattedOutput(80);
		}

		public void DeserializeFromString(string ar)
		{
			var reader = new UTFBinReader(ar.Replace("\r", "").Replace("\n", ""));

			var h0 = reader.ReadRawPrintableByte();
			var h1 = reader.ReadRawPrintableByte();

			if (h0 != HEADER[0] || h1 != HEADER[1])
			{
				throw new SAMPersistanceException("Deserialization failed, HEADER not recognized");
			}
			
			var fileVersion = reader.ReadVersion();

			if (fileVersion.IsLaterThan(ArchiveVersion))
			{
				throw new SAMPersistanceException($"Cannot deserialize newer archive version: {fileVersion} > {ArchiveVersion}");
			}

			Deserialize(reader, fileVersion);
		}

	}
}
