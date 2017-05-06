using System.IO;

namespace GridDominance.Levelfileformat.Blueprint
{
	public sealed class VoidCircleBlueprint
	{
		public readonly float X; // center
		public readonly float Y;
		public readonly float Diameter;

		public VoidCircleBlueprint(float x, float y, float d)
		{
			X = x;
			Y = y;
			Diameter = d;
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(LevelBlueprint.SERIALIZE_ID_VOIDCIRCLE);
			bw.Write(X);
			bw.Write(Y);
			bw.Write(Diameter);
		}

		public static VoidCircleBlueprint Deserialize(BinaryReader br)
		{
			var x = br.ReadSingle();
			var y = br.ReadSingle();
			var d = br.ReadSingle();

			return new VoidCircleBlueprint(x, y, d);
		}
	}
}
