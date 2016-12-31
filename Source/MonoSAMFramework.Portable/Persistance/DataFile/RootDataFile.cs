using MonoSAMFramework.Portable.Persistance.DataFileFormat;

namespace MonoSAMFramework.Portable.Persistance.DataFile
{
	public abstract class RootDataFile : BaseDataFile
	{
		private const string HEADER = "$=";

		private const int HEADER_LENGTH = 2 + PersistanceHelper.INTEGRITY_HASH_LEN;

		protected abstract SemVersion ArchiveVersion { get; }

		public string SerializeToString(int maxLen = 80)
		{
			var writer = new UTFBinWriter();

			writer.WriteVersion(ArchiveVersion);

			Serialize(writer, ArchiveVersion);

			var hash = writer.GetHashOfCurrentState();

			return writer.ToFormattedOutput(HEADER + hash, maxLen);
		}

		public void DeserializeFromString(string ar)
		{
			if (ar.Length < HEADER_LENGTH)
				throw new SAMPersistanceException("Deserialization failed, HEADER too short");

			var header = ar.Substring(0, HEADER_LENGTH);
			var reader = new UTFBinReader(ar, HEADER_LENGTH);
			
			if (header[0] != HEADER[0] || header[1] != HEADER[1])
				throw new SAMPersistanceException("Deserialization failed, HEADER not recognized");

			if (reader.GetHashOfInput() != ar.Substring(2, PersistanceHelper.INTEGRITY_HASH_LEN))
				throw new SAMPersistanceException("Deserialization failed, Checksum mismatch");
			
			var fileVersion = reader.ReadVersion();

			if (fileVersion.IsLaterThan(ArchiveVersion))
				throw new SAMPersistanceException($"Cannot deserialize newer archive version: {fileVersion} > {ArchiveVersion}");

			Deserialize(reader, fileVersion);
		}

	}
}
