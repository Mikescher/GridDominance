using System.IO;

namespace GridDominance.Levelfileformat.Blueprint
{
	public sealed class MirrorBlockBlueprint
	{
		public const float DEFAULT_WIDTH = 24; // TILE_WIDTH * 0.333f

		public readonly float X; // center
		public readonly float Y;
		public readonly float Width;
		public readonly float Height;
		public readonly float Rotation; // in degree

		public MirrorBlockBlueprint(float x, float y, float w, float h, float r)
		{
			X = x;
			Y = y;
			Width = w;
			Height = h;
			Rotation = r;
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(LevelBlueprint.SERIALIZE_ID_MIRRORBLOCK);
			bw.Write(X);
			bw.Write(Y);
			bw.Write(Width);
			bw.Write(Height);
			bw.Write(Rotation);
		}

		public static MirrorBlockBlueprint Deserialize(BinaryReader br)
		{
			var x = br.ReadSingle();
			var y = br.ReadSingle();
			var w = br.ReadSingle();
			var h = br.ReadSingle();
			var r = br.ReadSingle();

			return new MirrorBlockBlueprint(x, y, w, h, r);
		}
	}
}
