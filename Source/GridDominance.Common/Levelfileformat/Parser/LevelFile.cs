using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GridDominance.Levelformat.Parser
{
	public class LevelFile
	{
		private static readonly Regex REX_COMMAND = new Regex(@"^(?<ident>[A-Za-z_]+)\s*\((((?<param>[^,\(\)]+)\s*,\s*)*(?<param>[^,\(\)]+))?\)$");
		private static readonly Regex REX_EXPRESSION = new Regex(@"(\d*\.\d+)|(\d+)|([A-Za-z_]+)|[\+\-\*/]");

		private const byte SERIALIZE_ID_CANNON      = 0x01; 
		private const byte SERIALIZE_ID_NAME        = 0x02; 
		private const byte SERIALIZE_ID_DESCRIPTION = 0x03; 
		private const byte SERIALIZE_ID_GUID        = 0x04;
		private const byte SERIALIZE_ID_EOF         = 0xFF;

		private readonly string content;
		private readonly Dictionary<string, float> constants = new Dictionary<string, float>();
		private Dictionary<string, Action<List<string>>> actions;
		private readonly Func<string, string> includeFinderFunction;

		public readonly List<LPCannon> BlueprintCannons = new List<LPCannon>();
		public Guid UniqueID { get; private set; } = Guid.Empty;
		public string Name { get; private set; } = "";
		public string FullName { get; private set; } = "";

		public LevelFile()
		{
			content = string.Empty;
			includeFinderFunction = s => null; 

			Init();
		}

		public LevelFile(string levelContent, Func<string, string> includeFunction)
		{
			content = levelContent;
			includeFinderFunction = includeFunction;

			Init();
		}

		private void Init()
		{
			actions = new Dictionary<string, Action<List<string>>>
			{
				{"define", DefineConstant},
				{"cannon", AddCannon},
				{"include", IncludeSource},
				{"init", InitLevel},
			};
		}

		#region Parsing

		public void Parse()
		{
			Parse("__root__", content);
		}

		public void Parse(string fileName, string fileContent)
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

			if (string.IsNullOrWhiteSpace(Name))
				throw new Exception("Level needs a valid name");

			if (UniqueID == Guid.Empty)
				throw new Exception("Level needs a valid UUID");
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
				if (line[i] == ' ')
				{
					continue;
				}
				if (line[i] == '\t')
				{
					continue;
				}
				
				if (start == -1) start = i;
				end = i+1;
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

			var x = methodParameter[idx];

			if (x.Length < 2) throw new Exception("String parse exception");
			if (!x.StartsWith("\"")) throw new Exception("String parse exception");
			if (!x.EndsWith("\"")) throw new Exception("String parse exception");

			return x.Substring(1, x.Length - 2);
		}

		private string ExtractValueParameter(List<string> methodParameter, int idx)
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

		private Guid ExtractGuidParameter(List<string> methodParameter, int idx)
		{
			if (idx >= methodParameter.Count)
				throw new Exception($"Not enough parameter (missing param {idx})");

			var x = methodParameter[idx];

			Guid g;
			if (Guid.TryParse(x, out g)) return g;

			throw new Exception($"GUID parameter {idx} has invalid syntax: '{x}'");
		}

		#endregion

		#region LevelMethods

		private void DefineConstant(List<string> methodParameter)
		{
			var key = ExtractValueParameter(methodParameter, 0).ToLower();
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

		private void IncludeSource(List<string> methodParameter)
		{
			var fileName = ExtractStringParameter(methodParameter, 0);
			var fileContent = includeFinderFunction(fileName);

			if (fileContent == null) throw new Exception("Include not found: " + fileName);

			Parse(fileName, fileContent);
		}

		private void InitLevel(List<string> methodParameter)
		{
			var levelname = ExtractStringParameter(methodParameter, 0);
			var leveldesc = ExtractStringParameter(methodParameter, 1);
			var levelguid = ExtractGuidParameter(methodParameter, 2);

			Name = levelname;
			FullName = leveldesc;
			UniqueID = levelguid;
		}

		#endregion

		#region Pipeline Serialize

		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(SERIALIZE_ID_NAME);
			bw.Write(Name);

			bw.Write(SERIALIZE_ID_DESCRIPTION);
			bw.Write(FullName);

			bw.Write(SERIALIZE_ID_GUID);
			bw.Write(UniqueID.ToByteArray());

			foreach (var cannon in BlueprintCannons)
			{
				bw.Write(SERIALIZE_ID_CANNON);
				bw.Write(cannon.Player);
				bw.Write(cannon.X);
				bw.Write(cannon.Y);
				bw.Write(cannon.Scale);
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
					case SERIALIZE_ID_NAME:
					{
						Name = br.ReadString();

						break;
					}
					case SERIALIZE_ID_DESCRIPTION:
					{
						FullName = br.ReadString();

						break;
					}
					case SERIALIZE_ID_GUID:
					{ 
						UniqueID = new Guid(br.ReadBytes(16));

						break;
					}
					case SERIALIZE_ID_EOF:
					{
						if (string.IsNullOrWhiteSpace(Name)) throw new Exception("Level needs a valid name");
						if (UniqueID == Guid.Empty) throw new Exception("Level needs a valid UUID");

						return;
					}

					default:
					{
						throw new Exception("Unknown binary ID:" + id[0]);
					}
				}
			}

			throw new Exception("Unexpected binary file end");
		}

		#endregion

		#region Static Helper
		
		public static bool IsIncludeMatch(string a, string b)
		{
			const StringComparison icic = StringComparison.CurrentCultureIgnoreCase;

			if (string.Equals(a, b, icic)) return true;

			if (a.LastIndexOf('.') > 0) a = a.Substring(0, a.LastIndexOf('.'));
			if (b.LastIndexOf('.') > 0) b = b.Substring(0, b.LastIndexOf('.'));

			return string.Equals(a, b, icic);
		}

		#endregion

		#region Output

		public string GenerateASCIIMap()
		{
			var builder = new StringBuilder();

			builder.AppendLine("#<map>");
			builder.AppendLine("#");
			builder.AppendLine("#            0 1 2 3 4 5 6 7 8 9 A B C D E F");
			builder.AppendLine("#          # # # # # # # # # # # # # # # # # #");
			for (int y = 0; y < 10; y++)
			{
				builder.Append("#        ");
				builder.Append(y);
				builder.Append(" #");

				for (int x = 0; x < 16; x++)
				{
					builder.Append(" ");
					if (BlueprintCannons.Any(p => (int) Math.Round(p.X / 64) == (x + 0) && (int) Math.Round(p.Y / 64) == (y + 0)))
						builder.Append("/");
					else if (BlueprintCannons.Any(p => (int)Math.Round(p.X / 64) == (x + 1) && (int)Math.Round(p.Y / 64) == (y + 0)))
						builder.Append("\\");
					else if (BlueprintCannons.Any(p => (int)Math.Round(p.X / 64) == (x + 0) && (int)Math.Round(p.Y / 64) == (y + 1)))
						builder.Append("\\");
					else if (BlueprintCannons.Any(p => (int)Math.Round(p.X / 64) == (x + 1) && (int)Math.Round(p.Y / 64) == (y + 1)))
						builder.Append("/");
					else
						builder.Append(" ");
				}

				builder.AppendLine(" #");
			}
			builder.AppendLine("#          # # # # # # # # # # # # # # # # # #");
			builder.AppendLine("#");
			builder.Append("#</map>");

			return builder.ToString();
		}

		#endregion
	}
}
