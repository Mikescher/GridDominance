using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GridDominance.Levelformat.Parser
{
	public class LevelFile
	{
		private static readonly Regex REX_COMMAND = new Regex(@"^(?<ident>[A-Za-z_]+)\s*\((((?<param>[^,\(\)]+)\s*,\s*)*(?<param>[^,\(\)]+))?\)$");
		private static readonly Regex REX_EXPRESSION = new Regex(@"(\d*\.\d+)|(\d+)|([A-Za-z_]+)|[\+\-\*/]");

		private const byte SERIALIZE_ID_CANNON = 0x01;
		private const byte SERIALIZE_ID_EOF    = 0xFF;

		private readonly string content;
		private readonly Dictionary<string, float> constants = new Dictionary<string, float>();
		private readonly Dictionary<string, Action<List<string>>> actions;

		public readonly List<LPCannon> BlueprintCannons = new List<LPCannon>();

		public LevelFile(string levelContent)
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

		private void ParseLine(string cntLine)
		{
			var match = REX_COMMAND.Match(cntLine);

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

		private string ExtractStringParameter(List<string> methodParameter, int idx)
		{
			if (idx >= methodParameter.Count)
				throw new Exception($"Not enough parameter (missing param {idx})");

			return methodParameter[idx];
		}
		
		private int ExtractIntegerParameter(List<string> methodParameter, int idx)
		{
			if (idx >= methodParameter.Count)
				throw new Exception($"Not enough parameter (missing param {idx})");

			float dConstResult;
			if (constants.TryGetValue(methodParameter[idx].ToLower(), out dConstResult))
			{
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (dConstResult % 1 != 0) throw new Exception($"Constant {methodParameter[idx]} must be integer");

				return (int) dConstResult;
			}

			return int.Parse(methodParameter[idx]);
		}

		private float ExtractNumberParameter(List<string> methodParameter, int idx, int? defaultValue = null)
		{
			if (idx >= methodParameter.Count)
			{
				if (defaultValue != null) return defaultValue.Value;
				throw new Exception($"Not enough parameter (missing param {idx})");
			}

			float constResult;

			if (constants.TryGetValue(methodParameter[idx].ToLower(), out constResult))
				return constResult;

			if (float.TryParse(methodParameter[idx], NumberStyles.Float, CultureInfo.InvariantCulture, out constResult))
				return constResult;

			var tokens = REX_EXPRESSION.Matches(methodParameter[idx]).OfType<Match>().Select(p => p.Value).ToList();

			float value = GetFloatConstant(tokens[0]);

			// We don NOT support operator precedence - everythin is left to right
			// this is not a full math parser, just a little convenience
			for (int i = 1; i < tokens.Count; i+=2)
			{
				switch (tokens[i][0])
				{
					case '*':
						value = value * GetFloatConstant(tokens[i + 1]);
						break;
					case '+':
						value = value + GetFloatConstant(tokens[i + 1]);
						break;
					case '-':
						value = value - GetFloatConstant(tokens[i + 1]);
						break;
					case '/':
						value = value / GetFloatConstant(tokens[i + 1]);
						break;
					default:
						throw new Exception("Unkwon math symbol: " + tokens[i]);
				}
			}

			return value;
		}
		
		private float GetFloatConstant(string rep)
		{
			float constResult;

			if (constants.TryGetValue(rep.ToLower(), out constResult)) return constResult;

			return float.Parse(rep, NumberStyles.Float, CultureInfo.InvariantCulture);
		}

		#endregion

		#region LevelMethods

		private void DefineConstant(List<string> methodParameter)
		{
			var key = ExtractStringParameter(methodParameter, 0).ToLower();
			var val = ExtractNumberParameter(methodParameter, 1);

			constants.Add(key, val);
		}

		private void AddCannon(List<string> methodParameter)
		{
			var size     = ExtractNumberParameter(methodParameter, 0);
			var player   = ExtractIntegerParameter(methodParameter, 1);
			var posX     = ExtractNumberParameter(methodParameter, 2);
			var posY     = ExtractNumberParameter(methodParameter, 3);
			var rotation = ExtractNumberParameter(methodParameter, 4, -1);

			BlueprintCannons.Add(new LPCannon(posX, posY, size, player, rotation));
		}

		#endregion

		#region Pipeline Serialize

		public void BinarySerialize(BinaryWriter bw)
		{
			foreach (var cannon in BlueprintCannons)
			{
				bw.Write(SERIALIZE_ID_CANNON);
				bw.Write(cannon.Player);
				bw.Write(cannon.X);
				bw.Write(cannon.Y);
				bw.Write(cannon.Radius);
				bw.Write(cannon.Rotation);
			}

			bw.Write(SERIALIZE_ID_EOF);
		}

		public void BinaryDeserialize(BinaryReader br)
		{
			byte[] id = new byte[1];
			while (br.Read(id, 0, 1) > 0)
			{
				switch (id[0])
				{
					case SERIALIZE_ID_CANNON:
					{
						var p = br.ReadInt32();
						var x = br.ReadSingle();
						var y = br.ReadSingle();
						var r = br.ReadSingle();
						var a = br.ReadSingle();

						BlueprintCannons.Add(new LPCannon(x, y, r, p, a));

						break;
					}
					case SERIALIZE_ID_EOF:
					{
						return;
					}

					default:
					{
						throw new Exception("Unkwown binary ID:" + id[0]);
					}
				}
			}

			throw new Exception("Unexpected binary file end");
		}

		#endregion
	}
}
