using System;
using System.IO;

namespace GridDominance.Levelfileformat.Blueprint
{
	public sealed class VoidWallBlueprint
	{
		public const float DEFAULT_WIDTH = 8f;

		public readonly float X; // center
		public readonly float Y;
		public readonly float Length;
		public readonly float Rotation; // in degree

		public VoidWallBlueprint(float x, float y, float l, float rot)
		{
			X = x;
			Y = y;
			Length = l;
			Rotation = rot;

			if (rot < 0)
				Rotation = (float) (Math.Atan2(320 - Y, 512 - X) / Math.PI * 180);
			else
				Rotation = rot;
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(LevelBlueprint.SERIALIZE_ID_VOIDWALL);
			bw.Write(X);
			bw.Write(Y);
			bw.Write(Length);
			bw.Write(Rotation);
		}

		public static VoidWallBlueprint Deserialize(BinaryReader br)
		{
			var x = br.ReadSingle();
			var y = br.ReadSingle();
			var l = br.ReadSingle();
			var r = br.ReadSingle();

			return new VoidWallBlueprint(x, y, l, r);
		}
	}
}
