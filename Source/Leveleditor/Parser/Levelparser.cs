using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Leveleditor.Parser
{
	class Levelparser
	{
		private static readonly Regex REX_COMMAND = new Regex(@"(?<ident>[A-Za-z_]+)\s*\((((?<param>[^,\(\)]+)\s*,\s*)*(?<param>[^,\(\)]+))?\)");
		private static readonly Regex REX_EXPRESSION = new Regex(@"(\d*\.\d+)|(\d+)|([A-Za-z_]+)|[\+\-\*/]");

		private readonly string content;
		private readonly Dictionary<string, double> constants = new Dictionary<string, double>();
		private readonly Dictionary<string, Action<List<string>>> actions;

		public readonly List<LPCannon> BlueprintCannons = new List<LPCannon>();

		public Levelparser(string levelContent)
		{
			content = levelContent;

			actions = new Dictionary<string, Action<List<string>>>
			{
				{ "define", DefineConstant },
				{ "cannon", AddCannon },
			};
		}

		#region Parsing

		public void Parse()
		{
			using (StringReader sr = new StringReader(content))
			{
				string rawline;
				int lineNumber = 0;
				while ((rawline = sr.ReadLine()) != null)
				{
					lineNumber++;
					try
					{
						var line = Uncomment(rawline);
						if (line == string.Empty) continue;

						ParseLine(line);
					}
					catch (Exception e)
					{
						throw new ParsingException(lineNumber, e);
					}
				}
			}
		}

		private void ParseLine(string content)
		{
			var match = REX_COMMAND.Match(content);

			if (! match.Success) throw new Exception("Regex match failed");

			var identifier = match.Groups["ident"].Value.ToLower();
			var parameter = match.Groups["param"].Captures.Cast<Capture>().Select(p => p.Value).ToList();

			actions[identifier](parameter);
		}

		private string Uncomment(string line)
		{
			int start = -1;
			int end = -1;
			for (int i = 0; i < line.Length; i++)
			{
				if (line[i] == '#')
				{
					if (start < 0) return string.Empty;
					return line.Substring(start, end - start);
				}
				else if (line[i] == ' ')
				{
					continue;
				}
				else if (line[i] == '\t')
				{
					continue;
				}
				else
				{
					if (start == -1) start = i;
					end = i+1;
				}
			}

			if (start == end) return string.Empty;
			return line.Substring(start, end - start);
		}

		#endregion

		#region Helper

		private string ExtractStringParameter(IReadOnlyList<string> methodParameter, int idx)
		{
			if (idx >= methodParameter.Count)
				throw new Exception($"Not enough parameter (missing param {idx})");

			return methodParameter[idx];
		}
		
		private int ExtractIntegerParameter(IReadOnlyList<string> methodParameter, int idx)
		{
			if (idx >= methodParameter.Count)
				throw new Exception($"Not enough parameter (missing param {idx})");

			double dConstResult;
			if (constants.TryGetValue(methodParameter[idx].ToLower(), out dConstResult))
			{
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (dConstResult % 1 != 0) throw new Exception($"Constant {methodParameter[idx]} must be integer");

				return (int) dConstResult;
			}

			return int.Parse(methodParameter[idx]);
		}

		private double ExtractNumberParameter(IReadOnlyList<string> methodParameter, int idx)
		{
			if (idx >= methodParameter.Count)
				throw new Exception($"Not enough parameter (missing param {idx})");

			double constResult;

			if (constants.TryGetValue(methodParameter[idx].ToLower(), out constResult))
				return constResult;

			if (double.TryParse(methodParameter[idx], NumberStyles.Float, CultureInfo.InvariantCulture, out constResult))
				return constResult;

			var tokens = REX_EXPRESSION.Matches(methodParameter[idx]).OfType<Match>().Select(p => p.Value).ToList();

			double value = GetDoubleConstant(tokens[0]);

			// We don NOT support operator precedence - everythin is left to right
			// this is not a full math parser, just a little convenience
			for (int i = 1; i < tokens.Count; i+=2)
			{
				switch (tokens[i][0])
				{
					case '*':
						value = value * GetDoubleConstant(tokens[i + 1]);
						break;
					case '+':
						value = value + GetDoubleConstant(tokens[i + 1]);
						break;
					case '-':
						value = value - GetDoubleConstant(tokens[i + 1]);
						break;
					case '/':
						value = value / GetDoubleConstant(tokens[i + 1]);
						break;
					default:
						throw new Exception("Unkwon math symbol: " + tokens[i]);
				}
			}

			return value;
		}
		
		private double GetDoubleConstant(string rep)
		{
			double constResult;

			if (constants.TryGetValue(rep.ToLower(), out constResult)) return constResult;

			return double.Parse(rep, NumberStyles.Float, CultureInfo.InvariantCulture);
		}

		#endregion

		#region LevelMethods

		private void DefineConstant(IReadOnlyList<string> methodParameter)
		{
			var key = ExtractStringParameter(methodParameter, 0).ToLower();
			var val = ExtractNumberParameter(methodParameter, 1);

			constants.Add(key, val);
		}

		private void AddCannon(IReadOnlyList<string> methodParameter)
		{
			var size   = ExtractNumberParameter(methodParameter, 0);
			var player = ExtractIntegerParameter(methodParameter, 1);
			var posX   = ExtractNumberParameter(methodParameter, 2);
			var posY   = ExtractNumberParameter(methodParameter, 3);

			BlueprintCannons.Add(new LPCannon(posX, posY, size, player));
		}

		#endregion
	}
}
