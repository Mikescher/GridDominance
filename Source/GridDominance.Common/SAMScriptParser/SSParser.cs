using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GridDominance.SAMScriptParser
{
	public abstract class SSParser
	{
		private static readonly Regex REX_EXPRESSION = new Regex(@"(\d*\.\d+)|(\d+)|([A-Za-z_]+)|[\+\-\*/]");
		private static readonly Regex REX_NAMEDPARAM = new Regex(@"^\s*(?<name>[a-zA-Z0-9_]+)\s*=\s*(?<value>[^\s]+)$");

		private readonly Dictionary<string, Action<List<string>>> _actions;
		private readonly Dictionary<string, string> _aliasDict = new Dictionary<string, string>();

		protected SSParser()
		{
			_actions = new Dictionary<string, Action<List<string>>>();
		}

		protected void DefineMethod(string id, Action<List<string>> a)
		{
			_actions.Add(id.ToLower(), a);
		}

		protected void AddAlias(string key, string val)
		{
			_aliasDict.Add(key.ToLower(), val);
		}

		protected void StartParse(string fileName, string fileContent)
		{
			_aliasDict.Clear();

			InnerParse(fileName, fileContent);
		}

		protected void SubParse(string fileName, string fileContent)
		{
			InnerParse(fileName, fileContent);
		}

		private void InnerParse(string fileName, string fileContent)
		{
			using (StringReader sr = new StringReader(fileContent))
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
						throw new ParsingException(fileName, lineNumber, e);
					}
				}
			}
		}

		private void ParseLine(string cntLine)
		{
			cntLine = cntLine.Trim();

			int idx = 0;
			var identifierBuilder = new StringBuilder();
			for (; idx < cntLine.Length && cntLine[idx] != '('; idx++)
			{
				identifierBuilder.Append(cntLine[idx]);
			}
			if (idx >= cntLine.Length) throw new Exception("Could not parse line - no method call");

			var identifier = identifierBuilder.ToString().Trim().ToLower();

			idx++;

			List<string> parameter = new List<string>();
			while (idx < cntLine.Length && cntLine[idx] != ')')
			{
				parameter.Add(ParseComplexParam(cntLine, ref idx).Trim());
			}

			if (idx < cntLine.Length) throw new Exception("Could not parse line - content after parenthesis valley");

			_actions[identifier](parameter);
		}

		private string ParseComplexParam(string str, ref int idx)
		{
			StringBuilder b = new StringBuilder();
			int depth = 0;
			bool escape = false;
			while (idx < str.Length)
			{
				if (escape)
				{
					b.Append(str[idx]);
					if (str[idx] == '"') escape = false;
					idx++;
					continue;
				}

				switch (str[idx])
				{
					case '(':
					case '{':
					case '[':
					case '<':
						b.Append(str[idx]);
						depth++;
						idx++;
						continue;
					case ')':
					case '}':
					case ']':
					case '>':
						if (depth == 0) { idx++; return b.ToString(); }
						b.Append(str[idx]);
						idx++;
						depth--;
						continue;
					case ',':
						if (depth == 0) { idx++; return b.ToString(); }
						b.Append(str[idx]);
						idx++;
						continue;
					case '"':
						b.Append(str[idx]);
						escape = true;
						idx++;
						continue;
					default:
						b.Append(str[idx]);
						idx++;
						continue;
				}
			}

			throw new Exception("Could not parse param - unexpected EOL");
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
				if (line[i] == ' ')
				{
					continue;
				}
				if (line[i] == '\t')
				{
					continue;
				}

				if (start == -1) start = i;
				end = i + 1;
			}

			if (start == end) return string.Empty;
			return line.Substring(start, end - start);
		}

		protected string ExtractStringParameter(List<string> methodParameter, int idx)
		{
			var v = ExtractValueParameter(methodParameter, idx);

			if (v.Length < 2) throw new Exception("String parse exception");
			if (!v.StartsWith("\"")) throw new Exception("String parse exception");
			if (!v.EndsWith("\"")) throw new Exception("String parse exception");

			return v.Substring(1, v.Length - 2);
		}

		protected string ExtractValueParameter(List<string> methodParameter, string name, string defaultValue = null)
		{
			foreach (var v in methodParameter)
			{
				var m = REX_NAMEDPARAM.Match(v);
				if (!m.Success) continue;

				if (name.ToLower().Trim() == m.Groups["name"].Value.ToLower().Trim()) return DeRef(m.Groups["value"].Value);
			}

			if (defaultValue != null) return defaultValue;
			throw new Exception($"Not enough parameter (missing param '{name}')");
		}

		protected string ExtractValueParameter(List<string> methodParameter, int idx, string defaultValue = null)
		{
			if (idx >= methodParameter.Count)
			{
				if (defaultValue != null) return defaultValue;
				throw new Exception($"Not enough parameter (missing param {idx})");
			}

			var v = methodParameter[idx].Trim();
			var m = REX_NAMEDPARAM.Match(v);
			if (!m.Success) return DeRef(v);

			return DeRef(m.Groups["value"].Value);
		}

		protected int ExtractIntegerParameter(List<string> methodParameter, int idx)
		{
			var v = ExtractValueParameter(methodParameter, idx);
			return int.Parse(v);
		}

		protected int ExtractIntegerParameter(List<string> methodParameter, string name, int? defaultValue = null)
		{
			var v = defaultValue.HasValue ? ExtractValueParameter(methodParameter, name, defaultValue.Value.ToString()) : ExtractValueParameter(methodParameter, name);

			return int.Parse(v);
		}

		protected float ExtractNumberParameter(List<string> methodParameter, int idx, float? defaultValue = null)
		{
			if (idx >= methodParameter.Count)
			{
				if (defaultValue != null) return defaultValue.Value;
				throw new Exception($"Not enough parameter (missing param {idx})");
			}

			var v = ExtractValueParameter(methodParameter, idx);
			return EvaluateFloatExpr(v);
		}

		protected float ExtractNumberParameter(List<string> methodParameter, string name)
		{
			var v = ExtractValueParameter(methodParameter, name);
			return EvaluateFloatExpr(v);
		}

		protected Guid ExtractGuidParameter(List<string> methodParameter, int idx)
		{
			var v = ExtractValueParameter(methodParameter, idx);

			Guid g;
			if (Guid.TryParse(v, out g)) return g;

			throw new Exception($"GUID parameter {idx} has invalid syntax: '{v}'");
		}

		protected Guid ExtractGuidParameter(List<string> methodParameter, string name)
		{
			var v = ExtractValueParameter(methodParameter, name);

			Guid g;
			if (Guid.TryParse(v, out g)) return g;

			throw new Exception($"GUID parameter '{name}' has invalid syntax: '{v}'");
		}

		protected Tuple<float, float> ExtractVec2fParameter(List<string> methodParameter, int idx)
		{
			var v = ExtractListParameter(methodParameter, idx, '[', ']');

			if (v.Count != 2) throw new Exception("Vec2f needs to have exactly 2 components");

			var t1 = EvaluateFloatExpr(v[0]);
			var t2 = EvaluateFloatExpr(v[1]);

			return Tuple.Create(t1, t2);
		}

		protected Tuple<float, float> ExtractVec2fParameter(List<string> methodParameter, string name)
		{
			var v = ExtractListParameter(methodParameter, name, '[', ']');

			if (v.Count != 2) throw new Exception("Vec2f needs to have exactly 2 components");

			var t1 = EvaluateFloatExpr(v[0]);
			var t2 = EvaluateFloatExpr(v[1]);

			return Tuple.Create(t1, t2);
		}

		protected List<string> ExtractListParameter(List<string> methodParameter, int idx, char cstart, char cend)
		{
			var v = ExtractValueParameter(methodParameter, idx);

			if (v[0] != cstart) throw new Exception("Syntax of sublist invalid");

			int ipos = 1;
			List<string> list = new List<string>();
			while (ipos < v.Length && v[ipos] != cend)
			{
				list.Add(ParseComplexParam(v, ref ipos).Trim());
			}
			if (ipos < v.Length) throw new Exception("Could not parse parameter sublist - content after last char");

			return list;
		}

		protected List<string> ExtractListParameter(List<string> methodParameter, string name, char cstart, char cend)
		{
			var v = ExtractValueParameter(methodParameter, name);

			if (v[0] != cstart) throw new Exception("Syntax of sublist invalid");

			int ipos = 1;
			List<string> list = new List<string>();
			while (ipos < v.Length && v[ipos] != cend)
			{
				list.Add(ParseComplexParam(v, ref ipos).Trim());
			}
			if (ipos < v.Length) throw new Exception("Could not parse parameter sublist - content after last char");

			return list;
		}

		private float EvaluateFloatExpr(string expr)
		{
			var v = DeRef(expr);

			float constResult;
			if (float.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out constResult))
				return constResult;

			var tokens = REX_EXPRESSION.Matches(v).OfType<Match>().Select(p => p.Value).ToList();

			float value = float.Parse(DeRef(tokens[0]), NumberStyles.Float, CultureInfo.InvariantCulture);

			// We don NOT support operator precedence - everythin is left to right
			// this is not a full math parser, just a little convenience
			for (int i = 1; i < tokens.Count; i += 2)
			{
				switch (tokens[i][0])
				{
					case '*':
						value = value * float.Parse(DeRef(tokens[i + 1]), NumberStyles.Float, CultureInfo.InvariantCulture);
						break;
					case '+':
						value = value + float.Parse(DeRef(tokens[i + 1]), NumberStyles.Float, CultureInfo.InvariantCulture);
						break;
					case '-':
						value = value - float.Parse(DeRef(tokens[i + 1]), NumberStyles.Float, CultureInfo.InvariantCulture);
						break;
					case '/':
						value = value / float.Parse(DeRef(tokens[i + 1]), NumberStyles.Float, CultureInfo.InvariantCulture);
						break;
					default:
						throw new Exception("Unkwon math symbol: " + tokens[i]);
				}
			}

			return value;
		}

		private string DeRef(string r, int d = 16)
		{
			if (d <= 0) throw new Exception("Alias nesting too deep: " + r);
			if (_aliasDict.ContainsKey(r.ToLower())) return DeRef(_aliasDict[r.ToLower()], d-1);
			return r;
		}
	}
}
