using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

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

		public static List<Func<String>> AdditionalLogInfo = new List<Func<string>>();

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

				if (System.Diagnostics.Debugger.IsAttached && e.Level == SAMLogLevel.ERROR) System.Diagnostics.Debugger.Break();
				if (System.Diagnostics.Debugger.IsAttached && e.Level == SAMLogLevel.FATAL_ERROR) System.Diagnostics.Debugger.Break();
#endif

				_log.Add(e);

				LogEvent?.Invoke(null, new LogEventArgs { Entry = e });
			}
		}

		public static void FatalError(string id, string msgShort, string msgLong = null) => Add(new SAMLogEntry(SAMLogLevel.FATAL_ERROR, id, msgShort, LongLog(msgLong)));
		public static void FatalError(string id, Exception e) => Add(new SAMLogEntry(SAMLogLevel.FATAL_ERROR, id, e.Message, LongLog(e)));

		public static void Error(string id, string msgShort, string msgLong = null) => Add(new SAMLogEntry(SAMLogLevel.ERROR, id, msgShort, LongLog(msgLong)));
		public static void Error(string id, Exception e) => Add(new SAMLogEntry(SAMLogLevel.ERROR, id, e.Message, LongLog(e)));

		public static void Warning(string id, string msgShort, string msgLong = null) => Add(new SAMLogEntry(SAMLogLevel.WARNING, id, msgShort, msgLong ?? string.Empty));
		public static void Warning(string id, Exception e) => Add(new SAMLogEntry(SAMLogLevel.WARNING, id, e.Message, e.ToString()));

		public static void Info(string id, string msgShort, string msgLong = null) => Add(new SAMLogEntry(SAMLogLevel.INFORMATION, id, msgShort, msgLong ?? string.Empty));
		public static void Info(string id, Exception e) => Add(new SAMLogEntry(SAMLogLevel.INFORMATION, id, e.Message, e.ToString()));

#if DEBUG
		public static void Debug(string msg) => Add(new SAMLogEntry(SAMLogLevel.DEBUG, "D", msg, msg));
#else
		[System.Diagnostics.Conditional("DEBUG")]
		public static void Debug(string msg) {}
#endif

		public static string LongLog(string msgLong = null)
		{
			StringBuilder b = new StringBuilder();

			b.AppendLine(msgLong ?? "-- No verbose message --");
			b.AppendLine();
			b.AppendLine(MonoSAMGame.CurrentInst.Bridge.EnvironmentStackTrace);
			try
			{
				foreach (var li in AdditionalLogInfo)
				{
					b.AppendLine();
					b.AppendLine(li());
				}
			}
			catch (Exception ex)
			{
				b.AppendLine("Could not append additional log info: " + ex.Message);
			}


			return b.ToString();
		}

		public static string LongLog(Exception e)
		{
			StringBuilder b = new StringBuilder();

			b.AppendLine(e.ToString());
			b.AppendLine(b.ToString());
			try
			{
				foreach (var li in AdditionalLogInfo)
				{
					b.AppendLine();
					b.AppendLine(li());
				}
			}
			catch (Exception ex)
			{
				b.AppendLine("Could not append additional log info: " + ex.Message);
			}

			return b.ToString();
		}
	}
}