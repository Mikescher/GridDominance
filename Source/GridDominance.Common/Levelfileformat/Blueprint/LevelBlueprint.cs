using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GridDominance.Levelfileformat.Blueprint
{
	public sealed class LevelBlueprint
	{
		public const int KI_TYPE_RAYTRACE    = 10; 
		public const int KI_TYPE_PRECALC     = 11;
		public const int KI_TYPE_PRESIMULATE = 12;

		public const byte SERIALIZE_ID_CANNON       = 0x01; 
		public const byte SERIALIZE_ID_VOIDWALL     = 0x05;
		public const byte SERIALIZE_ID_VOIDCIRCLE   = 0x06;
		public const byte SERIALIZE_ID_GLASSBLOCK   = 0x07;
		public const byte SERIALIZE_ID_BLACKHOLE    = 0x09;
		public const byte SERIALIZE_ID_PORTAL       = 0x10;
		public const byte SERIALIZE_ID_LASERCANNON  = 0x11;
		public const byte SERIALIZE_ID_MIRRORBLOCK  = 0x12;
		public const byte SERIALIZE_ID_MIRRORCIRCLE = 0x13;
		public const byte SERIALIZE_ID_META         = 0x80;
		public const byte SERIALIZE_ID_EOF          = 0xFF;

		public readonly List<CannonBlueprint>       BlueprintCannons       = new List<CannonBlueprint>();
		public readonly List<VoidWallBlueprint>     BlueprintVoidWalls     = new List<VoidWallBlueprint>();
		public readonly List<VoidCircleBlueprint>   BlueprintVoidCircles   = new List<VoidCircleBlueprint>();
		public readonly List<GlassBlockBlueprint>   BlueprintGlassBlocks   = new List<GlassBlockBlueprint>();
		public readonly List<BlackHoleBlueprint>    BlueprintBlackHoles    = new List<BlackHoleBlueprint>();
		public readonly List<PortalBlueprint>       BlueprintPortals       = new List<PortalBlueprint>();
		public readonly List<LaserCannonBlueprint>  BlueprintLaserCannons  = new List<LaserCannonBlueprint>();
		public readonly List<MirrorBlockBlueprint>  BlueprintMirrorBlocks  = new List<MirrorBlockBlueprint>();
		public readonly List<MirrorCircleBlueprint> BlueprintMirrorCircles = new List<MirrorCircleBlueprint>();

		public IEnumerable<ICannonBlueprint> AllCannons => BlueprintCannons.Cast<ICannonBlueprint>().Concat(BlueprintLaserCannons);

		public Guid UniqueID     = Guid.Empty;
		public string Name       = "";
		public string FullName   = "";

		public byte KIType       = KI_TYPE_RAYTRACE;

		public float LevelWidth  = 16 * 64;
		public float LevelHeight = 10 * 64;

		public float LevelViewX = 8 * 64;
		public float LevelViewY = 5 * 64;

		public LevelBlueprint()
		{
			//
		}
		
		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write(SERIALIZE_ID_META);
			bw.Write(Name);
			bw.Write(FullName);
			bw.Write(UniqueID.ToByteArray());
			bw.Write(KIType);
			bw.Write(LevelWidth);
			bw.Write(LevelHeight);
			bw.Write(LevelViewX);
			bw.Write(LevelViewY);

			for (int i = 0; i < BlueprintCannons.Count;       i++) BlueprintCannons[i].Serialize(bw);
			for (int i = 0; i < BlueprintVoidWalls.Count;     i++) BlueprintVoidWalls[i].Serialize(bw);
			for (int i = 0; i < BlueprintVoidCircles.Count;   i++) BlueprintVoidCircles[i].Serialize(bw);
			for (int i = 0; i < BlueprintGlassBlocks.Count;   i++) BlueprintGlassBlocks[i].Serialize(bw);
			for (int i = 0; i < BlueprintBlackHoles.Count;    i++) BlueprintBlackHoles[i].Serialize(bw);
			for (int i = 0; i < BlueprintPortals.Count;       i++) BlueprintPortals[i].Serialize(bw);
			for (int i = 0; i < BlueprintLaserCannons.Count;  i++) BlueprintLaserCannons[i].Serialize(bw);
			for (int i = 0; i < BlueprintMirrorBlocks.Count;  i++) BlueprintMirrorBlocks[i].Serialize(bw);
			for (int i = 0; i < BlueprintMirrorCircles.Count; i++) BlueprintMirrorCircles[i].Serialize(bw);

			bw.Write(SERIALIZE_ID_EOF);

			bw.Write((byte)0xB1);
			bw.Write((byte)0x6B);
			bw.Write((byte)0x00);
			bw.Write((byte)0xB5);
		}

		public void BinaryDeserialize(BinaryReader br)
		{
			byte[] id = new byte[1];
			while (br.Read(id, 0, 1) > 0)
			{
				switch (id[0])
				{
					case SERIALIZE_ID_CANNON:
						BlueprintCannons.Add(CannonBlueprint.Deserialize(br));
						break;

					case SERIALIZE_ID_VOIDWALL:
						BlueprintVoidWalls.Add(VoidWallBlueprint.Deserialize(br));
						break;

					case SERIALIZE_ID_VOIDCIRCLE:
						BlueprintVoidCircles.Add(VoidCircleBlueprint.Deserialize(br));
						break;

					case SERIALIZE_ID_GLASSBLOCK:
						BlueprintGlassBlocks.Add(GlassBlockBlueprint.Deserialize(br));
						break;

					case SERIALIZE_ID_BLACKHOLE:
						BlueprintBlackHoles.Add(BlackHoleBlueprint.Deserialize(br));
						break;

					case SERIALIZE_ID_PORTAL:
						BlueprintPortals.Add(PortalBlueprint.Deserialize(br));
						break;

					case SERIALIZE_ID_LASERCANNON:
						BlueprintLaserCannons.Add(LaserCannonBlueprint.Deserialize(br));
						break;

					case SERIALIZE_ID_MIRRORBLOCK:
						BlueprintMirrorBlocks.Add(MirrorBlockBlueprint.Deserialize(br));
						break;

					case SERIALIZE_ID_MIRRORCIRCLE:
						BlueprintMirrorCircles.Add(MirrorCircleBlueprint.Deserialize(br));
						break;

					case SERIALIZE_ID_META:
						Name        = br.ReadString();
						FullName    = br.ReadString();
						UniqueID    = new Guid(br.ReadBytes(16));
						KIType      = br.ReadByte();
						LevelWidth  = br.ReadSingle();
						LevelHeight = br.ReadSingle();
						LevelViewX  = br.ReadSingle();
						LevelViewY  = br.ReadSingle();
						break;

					case SERIALIZE_ID_EOF:
					{
						if (string.IsNullOrWhiteSpace(Name)) throw new Exception("Level needs a valid name");
						if (UniqueID == Guid.Empty) throw new Exception("Level needs a valid UUID");

						if (br.ReadByte() != 0xB1) throw new Exception("Missing footer byte 1");
						if (br.ReadByte() != 0x6B) throw new Exception("Missing footer byte 2");
						if (br.ReadByte() != 0x00) throw new Exception("Missing footer byte 3");
						if (br.ReadByte() != 0xB5) throw new Exception("Missing footer byte 4");

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

		public void ValidOrThrow()
		{
			if (string.IsNullOrWhiteSpace(Name))
				throw new Exception("Level needs a valid name");

			if (UniqueID == Guid.Empty)
				throw new Exception("Level needs a valid UUID");

			if (AllCannons.Count() != AllCannons.Select(c => c.CannonID).Distinct().Count()) throw new Exception("Duplicate CannonID");

			foreach (var c in BlueprintCannons)
			{
				if (c.Diameter <= 0) throw new Exception("Cannon with diameter <= 0");
			}

			foreach (var w in BlueprintVoidWalls)
			{
				if (w.Length <= 0) throw new Exception("Voidwall with length <= 0");
			}

			foreach (var c in BlueprintVoidCircles)
			{
				if (c.Diameter <= 0) throw new Exception("Voidcircle with diameter <= 0");
			}

			foreach (var b in BlueprintGlassBlocks)
			{
				if (b.Width <= 0) throw new Exception("Glassblock with width <= 0");
				if (b.Height <= 0) throw new Exception("Glassblock with height <= 0");
			}

			foreach (var h in BlueprintBlackHoles)
			{
				if (h.Diameter <= 0) throw new Exception("Blackhole with diameter <= 0");
			}

			foreach (var p1 in BlueprintPortals)
			{
				//var match = BlueprintPortals.Where(p2 => p2.Group == p1.Group).Where(p2 => p2.Side == !p1.Side).Any();
				//if (!match) throw new Exception($"Portalgroup {p1.Group} has no matching in/out");
				if (p1.Length <= 0) throw new Exception("Portal with length <= 0");
			}

			foreach (var c in BlueprintMirrorCircles)
			{
				if (c.Diameter <= 0) throw new Exception("Mirrorcircle with diameter <= 0");
			}

			foreach (var b in BlueprintMirrorBlocks)
			{
				if (b.Width <= 0) throw new Exception("Mirrorblock with width <= 0");
				if (b.Height <= 0) throw new Exception("Mirrorblock with height <= 0");
			}

			foreach (var c in BlueprintLaserCannons)
			{
				if (c.Diameter <= 0) throw new Exception("Lasercannon with diameter <= 0");
			}
		}
	}
}
