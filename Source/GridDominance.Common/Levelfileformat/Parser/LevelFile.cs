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
		private const byte SERIALIZE_ID_VOIDWALL    = 0x05;
		private const byte SERIALIZE_ID_EOF         = 0xFF;

		public readonly List<LPCannon>   BlueprintCannons   = new List<LPCannon>();
		public readonly List<LPVoidWall> BlueprintVoidWalls = new List<LPVoidWall>();

		public Guid UniqueID { get; set; } = Guid.Empty;
		public string Name { get; set; } = "";
		public string FullName { get; set; } = "";
		
		public LevelFile()
		{
			//
		}
		
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

			foreach (var wall in BlueprintVoidWalls)
			{
				bw.Write(SERIALIZE_ID_VOIDWALL);
				bw.Write(wall.X);
				bw.Write(wall.Y);
				bw.Write(wall.Length);
				bw.Write(wall.Rotation);
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
					case SERIALIZE_ID_VOIDWALL:
					{
						var x = br.ReadSingle();
						var y = br.ReadSingle();
						var l = br.ReadSingle();
						var r = br.ReadSingle();

						BlueprintVoidWalls.Add(new LPVoidWall(x, y, l, r));

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
		
		public static bool IsIncludeMatch(string a, string b)
		{
			const StringComparison icic = StringComparison.CurrentCultureIgnoreCase;

			if (string.Equals(a, b, icic)) return true;

			if (a.LastIndexOf('.') > 0) a = a.Substring(0, a.LastIndexOf('.'));
			if (b.LastIndexOf('.') > 0) b = b.Substring(0, b.LastIndexOf('.'));

			return string.Equals(a, b, icic);
		}
		
		public string GenerateASCIIMap() //TODO Show VoidWall (and perhaps better algorithm for more structures later)
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
	}
}
