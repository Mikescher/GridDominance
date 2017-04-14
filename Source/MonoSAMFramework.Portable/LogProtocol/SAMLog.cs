using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MonoSAMFramework.Portable.LogProtocol
{
	public static class SAMLog
	{
		private static readonly object lockobj = new object();

		public class LogEventArgs
		{
			public SAMLogLevel Level => Entry.Level;
			public SAMLogEntry Entry;
		}
		
		public delegate void LogEventHandler(object sender, LogEventArgs e);
		public static event LogEventHandler LogEvent;

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
#if DEBUG
				System.Diagnostics.Debug.WriteLine($"[{e.Level}] {e.MessageShort}\r\n{e.MessageLong}");
#endif

				_log.Add(e);

				LogEvent?.Invoke(null, new LogEventArgs{Entry = e});
			}
		}

		public static void FatalError(string id, string msgShort, string msgLong = null) => Add(new SAMLogEntry(SAMLogLevel.FATAL_ERROR, id, msgShort, msgLong ?? msgShort));
		public static void FatalError(string id, Exception e) => Add(new SAMLogEntry(SAMLogLevel.FATAL_ERROR, id, e.Message, e.ToString()));

		public static void Error(string id, string msgShort, string msgLong = null) => Add(new SAMLogEntry(SAMLogLevel.ERROR, id, msgShort, msgLong ?? msgShort));
		public static void Error(string id, Exception e) => Add(new SAMLogEntry(SAMLogLevel.ERROR, id, e.Message, e.ToString()));

		public static void Warning(string id, string msgShort, string msgLong = null) => Add(new SAMLogEntry(SAMLogLevel.WARNING, id, msgShort, msgLong ?? msgShort));
		public static void Warning(string id, Exception e) => Add(new SAMLogEntry(SAMLogLevel.WARNING, id, e.Message, e.ToString()));

		public static void Info(string id, string msgShort, string msgLong = null) => Add(new SAMLogEntry(SAMLogLevel.INFORMATION, id, msgShort, msgLong ?? msgShort));
		public static void Info(string id, Exception e) => Add(new SAMLogEntry(SAMLogLevel.INFORMATION, id, e.Message, e.ToString()));

#if DEBUG
		public static void Debug(string msg) => Add(new SAMLogEntry(SAMLogLevel.DEBUG, "D", msg, msg));
#else
		[System.Diagnostics.Conditional("DEBUG")]
		public static void Debug(string msg) {}
#endif

	}
}