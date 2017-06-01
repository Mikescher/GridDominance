using System.IO;

namespace GridDominance.Levelfileformat.Blueprint
{
	public sealed class PortalBlueprint
	{
		public const float DEFAULT_WIDTH = 8f;

		public readonly float X; // center
		public readonly float Y;
		public readonly float Length;
		public readonly float Normal; // in degree
		public readonly short Group;
		public readonly bool Side; // Input|Output

		public PortalBlueprint(float x, float y, float l, float nrm, short g, bool s)
		{
			X = x;
			Y = y;
			Length = l;
			Normal = nrm;
			Group = g;
			Side = s;
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(LevelBlueprint.SERIALIZE_ID_PORTAL);
			bw.Write(X);
			bw.Write(Y);
			bw.Write(Length);
			bw.Write(Normal);
			bw.Write(Group);
			bw.Write(Side);
		}

		public static PortalBlueprint Deserialize(BinaryReader br)
		{
			var x = br.ReadSingle();
			var y = br.ReadSingle();
			var l = br.ReadSingle();
			var n = br.ReadSingle();
			var g = br.ReadInt16();
			var s = br.ReadBoolean();

			return new PortalBlueprint(x, y, l, n, g, s);
		}
	}
}
