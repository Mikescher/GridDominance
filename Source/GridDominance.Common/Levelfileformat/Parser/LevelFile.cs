using GridDominance.Levelformat.Parser;
using GridDominance.SAMScriptParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GridDominance.Levelfileformat.Parser
{
	public class LevelFile
	{
		private const byte SERIALIZE_ID_CANNON      = 0x01; 
		private const byte SERIALIZE_ID_NAME        = 0x02; 
		private const byte SERIALIZE_ID_DESCRIPTION = 0x03; 
		private const byte SERIALIZE_ID_GUID        = 0x04;
		private const byte SERIALIZE_ID_EOF         = 0xFF;

		private readonly SSParser parser = new SSParser();
		private readonly string content;
		private readonly Func<string, string> includeFinderFunction;

		public readonly List<LPCannon> BlueprintCannons = new List<LPCannon>();
		public Guid UniqueID { get; private set; } = Guid.Empty;
		public string Name { get; private set; } = "";
		public string FullName { get; private set; } = "";
		private float _scaleFactorX = 1f;
		private float _scaleFactorY = 1f;

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
			parser.DefineMethod("scale", SetScale);
			parser.DefineMethod("define", DefineAlias);
			parser.DefineMethod("cannon", AddCannon);
			parser.DefineMethod("include", IncludeSource);
			parser.DefineMethod("init", InitLevel);
		}

		#region Parsing

		public void Parse()
		{
			Parse("__root__", content);
		}

		public void Parse(string fileName, string fileContent)
		{
			BlueprintCannons.Clear();
			_scaleFactorX = 1f;
			_scaleFactorY = 1f;
			parser.Parse(fileName, fileContent);

			if (string.IsNullOrWhiteSpace(Name))
				throw new Exception("Level needs a valid name");

			if (UniqueID == Guid.Empty)
				throw new Exception("Level needs a valid UUID");
		}
		
		private void DefineAlias(List<string> methodParameter)
		{
			var key = parser.ExtractValueParameter(methodParameter, 0);
			var val = parser.ExtractValueParameter(methodParameter, 1);

			parser.AddAlias(key, val);
		}

		private void AddCannon(List<string> methodParameter)
		{
			var size     = parser.ExtractNumberParameter(methodParameter, 0);
			var player   = parser.ExtractIntegerParameter(methodParameter, 1);
			var posX     = parser.ExtractVec2fParameter(methodParameter, 2).Item1 * _scaleFactorX;
			var posY     = parser.ExtractVec2fParameter(methodParameter, 2).Item2 * _scaleFactorY;
			var rotation = parser.ExtractNumberParameter(methodParameter, 3, -1);

			BlueprintCannons.Add(new LPCannon(posX, posY, size, player, rotation));
		}

		private void IncludeSource(List<string> methodParameter)
		{
			var fileName = parser.ExtractStringParameter(methodParameter, 0);
			var fileContent = includeFinderFunction(fileName);

			if (fileContent == null) throw new Exception("Include not found: " + fileName);

			Parse(fileName, fileContent);
		}

		private void InitLevel(List<string> methodParameter)
		{
			var levelname = parser.ExtractStringParameter(methodParameter, 0);
			var leveldesc = parser.ExtractStringParameter(methodParameter, 1);
			var levelguid = parser.ExtractGuidParameter(methodParameter, 2);

			Name = levelname;
			FullName = leveldesc;
			UniqueID = levelguid;
		}

		private void SetScale(List<string> methodParameter)
		{
			_scaleFactorX = parser.ExtractNumberParameter(methodParameter, 0);
			_scaleFactorY = parser.ExtractNumberParameter(methodParameter, 1);
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
