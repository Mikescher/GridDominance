using System;
using System.IO;

namespace GridDominance.Levelfileformat.Blueprint
{
	public sealed class ShieldProjectorBlueprint : ICannonBlueprint
	{
		public readonly float X; // center
		public readonly float Y;
		public readonly float Diameter;
		public readonly int Player;
		public readonly float Rotation; // in degree
		public readonly byte CannonID;

		public BulletPathBlueprint[] PrecalculatedPaths;

		int ICannonBlueprint.CannonID => CannonID;
		float ICannonBlueprint.X => X;
		float ICannonBlueprint.Y => Y;
		float ICannonBlueprint.Diameter => Diameter;
		BulletPathBlueprint[] ICannonBlueprint.PrecalculatedPaths => PrecalculatedPaths;
		int ICannonBlueprint.Fraction => Player;

		public ShieldProjectorBlueprint(float x, float y, float d, int p, float rot, byte cid, BulletPathBlueprint[] bp)
		{
			X = x;
			Y = y;
			Diameter = d;
			Player = p;
			CannonID = cid;
			PrecalculatedPaths = bp;

			if (rot < 0)
				Rotation = (float)(Math.Atan2(320 - Y, 512 - X) / Math.PI * 180);
			else
				Rotation = rot;
		}

		public ShieldProjectorBlueprint(float x, float y, float d, int p, float rot, byte cid)
		{
			X = x;
			Y = y;
			Diameter = d;
			Player = p;
			CannonID = cid;
			PrecalculatedPaths = new BulletPathBlueprint[0];

			if (rot < 0)
				Rotation = (float)(Math.Atan2(320 - Y, 512 - X) / Math.PI * 180);
			else
				Rotation = rot;
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(LevelBlueprint.SERIALIZE_ID_SHIELDPROJECTOR);
			bw.Write((int)CannonID);
			bw.Write(Player);
			bw.Write(X);
			bw.Write(Y);
			bw.Write(Diameter);
			bw.Write(Rotation);

			bw.Write((short)PrecalculatedPaths.Length);
			foreach (var path in PrecalculatedPaths) path.Serialize(bw);
		}

		public static ShieldProjectorBlueprint Deserialize(BinaryReader br)
		{
			var i = br.ReadInt32();
			var p = br.ReadInt32();
			var x = br.ReadSingle();
			var y = br.ReadSingle();
			var d = br.ReadSingle();
			var a = br.ReadSingle();

			var pathCount = br.ReadInt16();
			var b = new BulletPathBlueprint[pathCount];
			for (int j = 0; j < pathCount; j++) b[j] = BulletPathBlueprint.Deserialize(br);

			return new ShieldProjectorBlueprint(x, y, d, p, a, (byte)i, b);
		}
	}
}
