namespace MonoSAMFramework.Portable.LogProtocol
{
	public sealed class SAMLogEntry
	{
		public readonly SAMLogLevel Level;
		public readonly string Type;
		public readonly string MessageShort;
		public readonly string MessageLong;

		public SAMLogEntry(SAMLogLevel lvl, string type, string mshort, string mlong)
		{
			Level = lvl;
			Type = type;
			MessageShort = mshort;
			MessageLong = mlong;
		}
	}
}
