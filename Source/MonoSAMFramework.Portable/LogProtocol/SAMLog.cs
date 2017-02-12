using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MonoSAMFramework.Portable.LogProtocol
{
	public static class SAMLog
	{
		private static readonly object lockobj = new object();

		//TODO Send logs 2 server or smth

		public static IReadOnlyList<SAMLogEntry> Entries
		{
			get
			{
				lock (lockobj)
				{
					return new ReadOnlyCollection<SAMLogEntry>(_log.ToList());
				}
			}
		}

		private static readonly List<SAMLogEntry> _log = new List<SAMLogEntry>();

		private static void Add(SAMLogEntry e)
		{
			lock (lockobj)
			{
				_log.Add(e);
			}
		}

		public static void FatalError(string id, string msgShort, string msgLong = null) => Add(new SAMLogEntry(SAMLogLevel.FATAL_ERROR, id, msgShort, msgLong ?? msgShort));
		public static void FatalError(string id, Exception e) => Add(new SAMLogEntry(SAMLogLevel.FATAL_ERROR, id, e.Message, e.ToString()));

		public static void Error(string id, string msgShort, string msgLong = null) => Add(new SAMLogEntry(SAMLogLevel.ERROR, id, msgShort, msgLong ?? msgShort));
		public static void Error(string id, Exception e) => Add(new SAMLogEntry(SAMLogLevel.ERROR, id, e.Message, e.ToString()));

		public static void Warning(string id, string msgShort, string msgLong = null) => Add(new SAMLogEntry(SAMLogLevel.WARNING, id, msgShort, msgLong ?? msgShort));
		public static void Warning(string id, Exception e) => Add(new SAMLogEntry(SAMLogLevel.WARNING, id, e.Message, e.ToString()));
	}
}