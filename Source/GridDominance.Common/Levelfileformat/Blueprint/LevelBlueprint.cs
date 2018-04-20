using GridDominance.Common.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MonoSAMFramework.Portable.GameMath;

namespace GridDominance.Levelfileformat.Blueprint
{
	public sealed class LevelBlueprint
	{
		public const byte SCHEMA_VERSION = 2;

		public const int KI_TYPE_RAYTRACE    = 10; 
		public const int KI_TYPE_PRECALC     = 11;
		public const int KI_TYPE_PRESIMULATE = 12;

		public const int WRAPMODE_DEATH = 101;
		public const int WRAPMODE_DONUT = 102;
		public const int WRAPMODE_SOLID = 103;

		public const int KI_CONFIG_TRACE_MAX_BULLETBOUNCE          = 200;
		public const int KI_CONFIG_TRACE_MAX_LASERREFLECT          = 201;
		public const int KI_CONFIG_TRACE_RESOULUTION               = 202;
		public const int KI_CONFIG_TRACE_HITBOX_ENLARGE            = 203;
		public const int KI_CONFIG_SIMULATION_RESOLUTION           = 204;
		public const int KI_CONFIG_SIMULATION_SCATTERTRUST         = 205;
		public const int KI_CONFIG_SIMULATION_UPS                  = 206;
		public const int KI_CONFIG_SIMULATION_LIFETIME_FAC         = 207;
		public const int KI_CONFIG_TRACE_NO_LASER_CORNER_REFLECT   = 208;
		public const int KI_CONFIG_TRACE_REFRAC_CORNER_ENLARGE_FAC = 209;

		public const byte SERIALIZE_ID_CANNON          = 0x01; 
		public const byte SERIALIZE_ID_VOIDWALL        = 0x05;
		public const byte SERIALIZE_ID_VOIDCIRCLE      = 0x06;
		public const byte SERIALIZE_ID_GLASSBLOCK      = 0x07;
		public const byte SERIALIZE_ID_BLACKHOLE       = 0x09;
		public const byte SERIALIZE_ID_PORTAL          = 0x10;
		public const byte SERIALIZE_ID_LASERCANNON     = 0x11;
		public const byte SERIALIZE_ID_MIRRORBLOCK     = 0x12;
		public const byte SERIALIZE_ID_MIRRORCIRCLE    = 0x13;
		public const byte SERIALIZE_ID_BACKGROUNDTEXT  = 0x14;
		public const byte SERIALIZE_ID_MINIGUN         = 0x15;
		public const byte SERIALIZE_ID_SHIELDPROJECTOR = 0x16;
		public const byte SERIALIZE_ID_RELAY           = 0x17;
		public const byte SERIALIZE_ID_TRISHOT         = 0x18;
		public const byte SERIALIZE_ID_META            = 0x80;
		public const byte SERIALIZE_ID_META_CUSTOM     = 0x82;
		public const byte SERIALIZE_ID_SCHEMA          = 0xAA;
		public const byte SERIALIZE_ID_EOF             = 0xFF;

		public readonly List<CannonBlueprint>          BlueprintCannons         = new List<CannonBlueprint>();
		public readonly List<VoidWallBlueprint>        BlueprintVoidWalls       = new List<VoidWallBlueprint>();
		public readonly List<VoidCircleBlueprint>      BlueprintVoidCircles     = new List<VoidCircleBlueprint>();
		public readonly List<GlassBlockBlueprint>      BlueprintGlassBlocks     = new List<GlassBlockBlueprint>();
		public readonly List<BlackHoleBlueprint>       BlueprintBlackHoles      = new List<BlackHoleBlueprint>();
		public readonly List<PortalBlueprint>          BlueprintPortals         = new List<PortalBlueprint>();
		public readonly List<LaserCannonBlueprint>     BlueprintLaserCannons    = new List<LaserCannonBlueprint>();
		public readonly List<MirrorBlockBlueprint>     BlueprintMirrorBlocks    = new List<MirrorBlockBlueprint>();
		public readonly List<MirrorCircleBlueprint>    BlueprintMirrorCircles   = new List<MirrorCircleBlueprint>();
		public readonly List<BackgroundTextBlueprint>  BlueprintBackgroundText  = new List<BackgroundTextBlueprint>();
		public readonly List<MinigunBlueprint>         BlueprintMinigun         = new List<MinigunBlueprint>();
		public readonly List<ShieldProjectorBlueprint> BlueprintShieldProjector = new List<ShieldProjectorBlueprint>();
		public readonly List<RelayCannonBlueprint>     BlueprintRelayCannon     = new List<RelayCannonBlueprint>();
		public readonly List<TrishotCannonBlueprint>   BlueprintTrishotCannon   = new List<TrishotCannonBlueprint>();

		public IEnumerable<ICannonBlueprint> AllCannons => 
			Enumerable.Empty<ICannonBlueprint>()
			.Concat(BlueprintCannons)
			.Concat(BlueprintLaserCannons)
			.Concat(BlueprintMinigun)
			.Concat(BlueprintShieldProjector)
			.Concat(BlueprintRelayCannon)
			.Concat(BlueprintTrishotCannon);

		public Dictionary<int, float> ParseConfiguration;
		
		public Guid UniqueID     = Guid.Empty;
		public string Name       = "";
		public string FullName   = "";
		public int CustomMusic   = -1;

		public byte KIType       = KI_TYPE_RAYTRACE;
		public byte WrapMode     = WRAPMODE_DEATH;

		public float LevelWidth  = 16 * 64;
		public float LevelHeight = 10 * 64;

		public float LevelViewX  = 8 * 64;
		public float LevelViewY  = 5 * 64;

		public ulong CustomMeta_MinLevelIntVersion = 0;
		public DateTimeOffset CustomMeta_Timestamp = DateTimeOffset.MinValue;
		public int CustomMeta_UserID = -1;
		public long CustomMeta_LevelID = -1;

		public LevelBlueprint()
		{
			//
		}
		
		public void BinarySerialize(BinaryWriter bw, bool custom, ulong minversion, int userid, long customlid)
		{
			bw.Write(SERIALIZE_ID_SCHEMA);
			bw.Write(SCHEMA_VERSION);

			bw.Write(SERIALIZE_ID_META);
			bw.Write(Name);
			bw.Write(FullName);
			bw.Write(UniqueID.ToByteArray());
			bw.Write(KIType);
			bw.Write(WrapMode);
			bw.Write(LevelWidth);
			bw.Write(LevelHeight);
			bw.Write(LevelViewX);
			bw.Write(LevelViewY);

			for (int i = 0; i < BlueprintCannons.Count;         i++) BlueprintCannons[i].Serialize(bw);
			for (int i = 0; i < BlueprintVoidWalls.Count;       i++) BlueprintVoidWalls[i].Serialize(bw);
			for (int i = 0; i < BlueprintVoidCircles.Count;     i++) BlueprintVoidCircles[i].Serialize(bw);
			for (int i = 0; i < BlueprintGlassBlocks.Count;     i++) BlueprintGlassBlocks[i].Serialize(bw);
			for (int i = 0; i < BlueprintBlackHoles.Count;      i++) BlueprintBlackHoles[i].Serialize(bw);
			for (int i = 0; i < BlueprintPortals.Count;         i++) BlueprintPortals[i].Serialize(bw);
			for (int i = 0; i < BlueprintLaserCannons.Count;    i++) BlueprintLaserCannons[i].Serialize(bw);
			for (int i = 0; i < BlueprintMirrorBlocks.Count;    i++) BlueprintMirrorBlocks[i].Serialize(bw);
			for (int i = 0; i < BlueprintMirrorCircles.Count;   i++) BlueprintMirrorCircles[i].Serialize(bw);
			for (int i = 0; i < BlueprintBackgroundText.Count;  i++) BlueprintBackgroundText[i].Serialize(bw);
			for (int i = 0; i < BlueprintMinigun.Count;         i++) BlueprintMinigun[i].Serialize(bw);
			for (int i = 0; i < BlueprintShieldProjector.Count; i++) BlueprintShieldProjector[i].Serialize(bw);
			for (int i = 0; i < BlueprintRelayCannon.Count;     i++) BlueprintRelayCannon[i].Serialize(bw);
			for (int i = 0; i < BlueprintTrishotCannon.Count;   i++) BlueprintTrishotCannon[i].Serialize(bw);

			if (custom)
			{
				bw.Write(SERIALIZE_ID_META_CUSTOM);
				bw.Write(CustomMusic); 
				bw.Write(minversion); 
				bw.Write(userid);
				long unixTimestamp = (Int64) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
				bw.Write(unixTimestamp);
				bw.Write(customlid);
			}

			bw.Write(SERIALIZE_ID_EOF);

			bw.Write((byte)0xB1);
			bw.Write((byte)0x6B);
			bw.Write((byte)0x00);
			bw.Write((byte)0xB5);
		}

		public ulong CalcCheckSum()
		{
			using (var ms = new MemoryStream())
			using (var bw = new BinaryWriter(ms))
			{
				BinarySerialize(bw, false, 0, -1, 0);
				return BitConverter.ToUInt64(MD5.GetHash(ms.ToArray()), 0);
			}
		}

		public void BinaryDeserialize(BinaryReader br)
		{
			CustomMeta_MinLevelIntVersion = 0;
			CustomMeta_UserID = -1;
			CustomMeta_LevelID = -1;
			CustomMeta_Timestamp = DateTimeOffset.MinValue;
			
			byte schema = 0;
			
			var eof = false;
			byte[] id = new byte[1];
			while (br.Read(id, 0, 1) > 0)
			{
				switch (id[0])
				{
					case SERIALIZE_ID_SCHEMA:
						schema = br.ReadByte();
						if (schema > SCHEMA_VERSION)
						{
							throw new Exception($"schema not supported ({schema} > {SCHEMA_VERSION})");
						}
						break;

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

					case SERIALIZE_ID_BACKGROUNDTEXT:
						BlueprintBackgroundText.Add(BackgroundTextBlueprint.Deserialize(br));
						break;

					case SERIALIZE_ID_MINIGUN:
						BlueprintMinigun.Add(MinigunBlueprint.Deserialize(br));
						break;

					case SERIALIZE_ID_SHIELDPROJECTOR:
						BlueprintShieldProjector.Add(ShieldProjectorBlueprint.Deserialize(br));
						break;

					case SERIALIZE_ID_RELAY:
						BlueprintRelayCannon.Add(RelayCannonBlueprint.Deserialize(br));
						break;

					case SERIALIZE_ID_TRISHOT:
						BlueprintTrishotCannon.Add(TrishotCannonBlueprint.Deserialize(br));
						break;

					case SERIALIZE_ID_META:
						Name        = br.ReadString();
						FullName    = br.ReadString();
						UniqueID    = new Guid(br.ReadBytes(16));
						KIType      = br.ReadByte();
						WrapMode    = br.ReadByte();
						LevelWidth  = br.ReadSingle();
						LevelHeight = br.ReadSingle();
						LevelViewX  = br.ReadSingle();
						LevelViewY  = br.ReadSingle();
						break;

					case SERIALIZE_ID_META_CUSTOM:
						CustomMusic                   = br.ReadInt32();
						CustomMeta_MinLevelIntVersion = br.ReadUInt64();
						CustomMeta_UserID             = br.ReadInt32();
						CustomMeta_Timestamp          = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(br.ReadInt64());
						CustomMeta_LevelID            = br.ReadInt64();
						break;

					case SERIALIZE_ID_EOF:
						if (br.ReadByte() != 0xB1) throw new Exception("Missing footer byte 1");
						if (br.ReadByte() != 0x6B) throw new Exception("Missing footer byte 2");
						if (br.ReadByte() != 0x00) throw new Exception("Missing footer byte 3");
						if (br.ReadByte() != 0xB5) throw new Exception("Missing footer byte 4");

						eof = true;
						break;

					default:
						throw new Exception("Unknown binary ID:" + id[0]);
				}
				if (eof) break;
			}
			
			if (!eof) throw new Exception("Unexpected binary file end");

			if (schema == 0)
			{
				// no schema
			}
			else if (schema == 1)
			{
				if (CustomMeta_LevelID > 0)
				{
					// Fix wrong LevelID Serialization in schema-1 :/

					var uid = ByteMath.SplitGuid(UniqueID);

					var a = uid.Item1;
					var b = uid.Item2;
					var c = uid.Item3;
					var d = uid.Item4;
					var e = uid.Item5;

					e = Convert.ToUInt64($"{e:X}", 10);

					UniqueID = ByteMath.JoinGuid(a, b, c, d, e);
				}
			}

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

			if (!new Regex(@"^[A-Za-z0-9]+\-[A-Za-z0-9]+$").IsMatch(Name))
				throw new Exception("Levelname has invalid format");

			if (UniqueID == Guid.Empty)
				throw new Exception("Level needs a valid UUID");

			if (AllCannons.Count() != AllCannons.Select(c => c.CannonID).Distinct().Count()) throw new Exception("Duplicate CannonID");

			if (! new[]{ KI_TYPE_PRECALC, KI_TYPE_PRESIMULATE, KI_TYPE_RAYTRACE }.Contains(KIType)) throw new Exception("Unknown KIType");
			if (! new[]{ WRAPMODE_DEATH, WRAPMODE_DONUT, WRAPMODE_SOLID }.Contains(WrapMode)) throw new Exception("Unknown WrapMode");

			foreach (var c in AllCannons)
			{
				if (c.Diameter <= 0.001) throw new Exception("Cannon with diameter <= 0");
				if (float.IsNaN(c.Diameter) || float.IsInfinity(c.Diameter)) throw new Exception("Cannon with diameter [NaN]");
			}

			foreach (var w in BlueprintVoidWalls)
			{
				if (w.Length <= 0.001) throw new Exception("Voidwall with length <= 0");
				if (float.IsNaN(w.Length) || float.IsInfinity(w.Length)) throw new Exception("Voidwall with length [NaN]");
			}

			foreach (var c in BlueprintVoidCircles)
			{
				if (c.Diameter <= 0.001) throw new Exception("Voidcircle with diameter <= 0");
				if (float.IsNaN(c.Diameter) || float.IsInfinity(c.Diameter)) throw new Exception("Voidcircle with diameter [NaN]");
			}

			foreach (var b in BlueprintGlassBlocks)
			{
				if (b.Width <= 0.001) throw new Exception("Glassblock with width <= 0");
				if (b.Height <= 0.001) throw new Exception("Glassblock with height <= 0");

				if (float.IsNaN(b.Width) || float.IsInfinity(b.Width)) throw new Exception("Glassblock with width [NaN]");
				if (float.IsNaN(b.Height) || float.IsInfinity(b.Height)) throw new Exception("Glassblock with height [NaN]");
			}

			foreach (var b in BlueprintBackgroundText)
			{
				if (b.Width <= 0.001) throw new Exception("Text with width <= 0");
				if (b.Height <= 0.001) throw new Exception("Text with height <= 0");

				if (float.IsNaN(b.Width) || float.IsInfinity(b.Width)) throw new Exception("Text with width [NaN]");
				if (float.IsNaN(b.Height) || float.IsInfinity(b.Height)) throw new Exception("Text with height [NaN]");
			}

			foreach (var h in BlueprintBlackHoles)
			{
				if (h.Diameter <= 0.001) throw new Exception("Blackhole with diameter <= 0");
				if (float.IsNaN(h.Diameter) || float.IsInfinity(h.Diameter)) throw new Exception("Blackhole with diameter [NaN]");
			}

			foreach (var p1 in BlueprintPortals)
			{
				//var match = BlueprintPortals.Where(p2 => p2.Group == p1.Group).Where(p2 => p2.Side == !p1.Side).Any();
				//if (!match) throw new Exception($"Portalgroup {p1.Group} has no matching in/out");
				if (p1.Length <= 0.001) throw new Exception("Portal with length <= 0");
				if (float.IsNaN(p1.Length) || float.IsInfinity(p1.Length)) throw new Exception("Portal with length [NaN]");

			}

			foreach (var c in BlueprintMirrorCircles)
			{
				if (c.Diameter <= 0.001) throw new Exception("Mirrorcircle with diameter <= 0");
				if (float.IsNaN(c.Diameter) || float.IsInfinity(c.Diameter)) throw new Exception("Mirrorcircle with diameter [NaN]");
			}

			foreach (var b in BlueprintMirrorBlocks)
			{
				if (b.Width <= 0.001) throw new Exception("Mirrorblock with width <= 0");
				if (b.Height <= 0.001) throw new Exception("Mirrorblock with height <= 0");

				if (float.IsNaN(b.Width) || float.IsInfinity(b.Width)) throw new Exception("Mirrorblock with width [NaN]");
				if (float.IsNaN(b.Height) || float.IsInfinity(b.Height)) throw new Exception("Mirrorblock with height [NaN]");
			}
		}

		public void GetConfig(int id, ref float d)
		{
			if (ParseConfiguration != null && ParseConfiguration.ContainsKey(id))
				d = ParseConfiguration[id];
		}

		public void GetConfig(int id, ref int d)
		{
			if (ParseConfiguration != null && ParseConfiguration.ContainsKey(id))
				d = (int)ParseConfiguration[id];
		}

		public void GetConfig(int id, ref bool d)
		{
			if (ParseConfiguration != null && ParseConfiguration.ContainsKey(id))
				d = ((int)ParseConfiguration[id]) != 0;
		}
	}
}
