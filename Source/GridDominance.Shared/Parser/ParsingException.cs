using System;

namespace GridDominance.Shared.Parser
{
	public class ParsingException : Exception
	{
		public readonly int LineNumber;

		public ParsingException(int line) : base(string.Empty)
		{
			LineNumber = line;
		}

		public ParsingException(int line, Exception e) : base(e.Message, e)
		{
			LineNumber = line;
		}

		public ParsingException(int line, string m) : base(m)
		{
			LineNumber = line;
		}

		public override string ToString()
		{
			return $"Parsing Error in line {LineNumber}:\r\n\r\n{base.ToString()}";
		}

		public string ToOutput()
		{
			return $"Parsing Error in line {LineNumber}:\r\n{base.Message}";
		}
	}
}
