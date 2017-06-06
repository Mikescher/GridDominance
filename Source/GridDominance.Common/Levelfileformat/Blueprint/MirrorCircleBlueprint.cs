using System.IO;

namespace GridDominance.Levelfileformat.Blueprint
{
	public sealed class MirrorCircleBlueprint
	{
		public readonly float X; // center
		public readonly float Y;
		public readonly float Diameter;

		public MirrorCircleBlueprint(float x, float y, float d)
		{
			X = x;
			Y = y;
			Diameter = d;
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(LevelBlueprint.SERIALIZE_ID_MIRRORCIRCLE);
			bw.Write(X);
			bw.Write(Y);
			bw.Write(Diameter);
		}

		public static MirrorCircleBlueprint Deserialize(BinaryReader br)
		{
			var x = br.ReadSingle();
			var y = br.ReadSingle();
			var d = br.ReadSingle();

			return new MirrorCircleBlueprint(x, y, d);
		}
	}
}
