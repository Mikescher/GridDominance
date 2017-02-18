using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.LogProtocol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.DebugTools
{
	class DebugTextLogLine : IDebugTextDisplayLineProvider
	{
		private readonly List<DebugTextDisplayLine> _lines;
		private static SAMLogEntry _lastReadEntry = null;

		public DebugTextLogLine()
		{
			_lines = new List<DebugTextDisplayLine>();
		}

		public void Update()
		{
			foreach (var entry in SAMLog.Entries.Reverse().TakeWhile(p => p != _lastReadEntry).Reverse())
			{
				_lines.Add(CreateLine(entry));
				_lastReadEntry = entry;
			}
		}

		public IEnumerable<DebugTextDisplayLine> GetLines()
		{
			return _lines;
		}

		public bool RemoveZombies()
		{
			for (int i = _lines.Count - 1; i >= 0; i--)
			{
				if (!_lines[i].IsAlive) _lines.RemoveAt(i);
			}

			return true;
		}

		private static DebugTextDisplayLine CreateLine(SAMLogEntry e)
		{
			DebugTextDisplayLine line;

			switch (e.Level)
			{
				case SAMLogLevel.FATAL_ERROR:
					line = new DebugTextDisplayLine(() => $"[{e.Type}] {e.MessageShort}");
					line.SetBackground(Color.Red);
					line.SetColor(Color.Black);
					return line;

				case SAMLogLevel.ERROR:
					line = new DebugTextDisplayLine(() => $"[{e.Type}] {e.MessageShort}");
					line.SetBackground(Color.Red * 0.6f);
					line.SetColor(Color.Black);
					return line;

				case SAMLogLevel.WARNING:
					line = new DebugTextDisplayLine(() => $"[{e.Type}] {e.MessageShort}");
					line.SetBackground(Color.DarkOrange * 0.6f);
					line.SetColor(Color.Black);
					line.SetLifetime(60);
					return line;

				case SAMLogLevel.INFORMATION:
					line = new DebugTextDisplayLine(() => $"[{e.Type}] {e.MessageShort}");
					line.SetBackground(Color.DodgerBlue * 0.5f);
					line.SetColor(Color.Black);
					line.SetLifetime(30);
					return line;

				case SAMLogLevel.DEBUG:
					line = new DebugTextDisplayLine(() => $"[{e.Type}] {e.MessageShort}");
					line.SetBackground(Color.GreenYellow * 0.5f);
					line.SetColor(Color.Black);
					line.SetLifetime(10);
					line.SetSpawntime(0.5f);
					line.SetDecaytime(2);
					return line;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
