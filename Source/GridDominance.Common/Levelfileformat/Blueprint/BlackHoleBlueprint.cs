using System.IO;

namespace GridDominance.Levelfileformat.Blueprint
{
	public sealed class BlackHoleBlueprint
	{
		public const float DEFAULT_POWER_FACTOR = 800f;

		public readonly float X; // center
		public readonly float Y;
		public readonly float Diameter;
		public readonly float Power; // neg=suck | pos=push

		public BlackHoleBlueprint(float x, float y, float d, float p)
		{
			X = x;
			Y = y;
			Diameter = d;
			Power = p;
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(LevelBlueprint.SERIALIZE_ID_BLACKHOLE);
			bw.Write(X);
			bw.Write(Y);
			bw.Write(Diameter);
			bw.Write(Power);
		}

		public static BlackHoleBlueprint Deserialize(BinaryReader br)
		{
			var x = br.ReadSingle();
			var y = br.ReadSingle();
			var d = br.ReadSingle();
			var p = br.ReadSingle();

			return new BlackHoleBlueprint(x, y, d, p);
		}
	}
}
