using System;

namespace GridDominance.SAMScriptParser
{
	public class ParsingException : Exception
	{
		public readonly String Filename;
		public readonly int LineNumber;

		public ParsingException(string filename, int line) : base(string.Empty)
		{
			Filename = filename;
			LineNumber = line;
		}

		public ParsingException(string filename, int line, Exception e) : base(e.Message, e)
		{
			Filename = filename;
			LineNumber = line;
		}

		public ParsingException(string filename, int line, string m) : base(m)
		{
			Filename = filename;
			LineNumber = line;
		}

		public override string ToString()
		{
			return $"Parsing Error in {Filename}:{LineNumber}:\r\n\r\n{base.ToString()}";
		}

		public string ToOutput()
		{
			return $"Parsing Error in {Filename}:{LineNumber}:\r\n{Message}";
		}
	}
}
