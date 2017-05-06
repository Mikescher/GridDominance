using System.IO;

namespace GridDominance.Levelfileformat.Blueprint
{
	public sealed class GlassBlockBlueprint
	{
		public const float DEFAULT_WIDTH = 24; // TILE_WIDTH * 0.333f

		public readonly float X; // center
		public readonly float Y;
		public readonly float Width;
		public readonly float Height;

		public GlassBlockBlueprint(float x, float y, float w, float h)
		{
			X = x;
			Y = y;
			Width = w;
			Height = h;
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(LevelBlueprint.SERIALIZE_ID_GLASSBLOCK);
			bw.Write(X);
			bw.Write(Y);
			bw.Write(Width);
			bw.Write(Height);
		}

		public static GlassBlockBlueprint Deserialize(BinaryReader br)
		{
			var x = br.ReadSingle();
			var y = br.ReadSingle();
			var w = br.ReadSingle();
			var h = br.ReadSingle();

			return new GlassBlockBlueprint(x, y, w, h);
		}
	}
}
