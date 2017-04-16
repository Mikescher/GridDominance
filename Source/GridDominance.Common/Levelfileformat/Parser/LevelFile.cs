using System;
using System.Collections.Generic;
using System.IO;

namespace GridDominance.Levelfileformat.Parser
{
	public class LevelFile
	{
		private const byte SERIALIZE_ID_CANNON      = 0x01; 
		private const byte SERIALIZE_ID_NAME        = 0x02; 
		private const byte SERIALIZE_ID_DESCRIPTION = 0x03; 
		private const byte SERIALIZE_ID_GUID        = 0x04;
		private const byte SERIALIZE_ID_VOIDWALL    = 0x05;
		private const byte SERIALIZE_ID_VOIDCIRCLE  = 0x06;
		private const byte SERIALIZE_ID_EOF         = 0xFF;

		public readonly List<LPCannon>     BlueprintCannons     = new List<LPCannon>();
		public readonly List<LPVoidWall>   BlueprintVoidWalls   = new List<LPVoidWall>();
		public readonly List<LPVoidCircle> BlueprintVoidCircles = new List<LPVoidCircle>();

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
				bw.Write(cannon.Diameter);
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

			foreach (var wall in BlueprintVoidCircles)
			{
				bw.Write(SERIALIZE_ID_VOIDCIRCLE);
				bw.Write(wall.X);
				bw.Write(wall.Y);
				bw.Write(wall.Diameter);
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
						var d = br.ReadSingle();
						var a = br.ReadSingle();

						BlueprintCannons.Add(new LPCannon(x, y, d, p, a));

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
					case SERIALIZE_ID_VOIDCIRCLE:
					{
						var x = br.ReadSingle();
						var y = br.ReadSingle();
						var d = br.ReadSingle();

						BlueprintVoidCircles.Add(new LPVoidCircle(x, y, d));

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
	}
}
