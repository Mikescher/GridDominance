using System.IO;

namespace GridDominance.Levelfileformat.Blueprint
{
	public sealed class BackgroundTextBlueprint
	{
		public const byte CONFIG_SHAKE          = 0x01;
		public const byte CONFIG_ONLYD1         = 0x02;
		public const byte CONFIG_ONLY_UNCLEARED = 0x04;
		public const byte CONFIG_REDFLASH       = 0x08;

		public readonly float X; // center
		public readonly float Y;
		public readonly float Width;
		public readonly float Height;
		public readonly float Rotation; // in degree
		public readonly int L10NText;
		public readonly ushort Config;

		public BackgroundTextBlueprint(float x, float y, float w, float h, float r, int t, ushort c)
		{
			X = x;
			Y = y;
			Width = w;
			Height = h;
			Rotation = r;
			L10NText = t;
			Config = c;
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(LevelBlueprint.SERIALIZE_ID_BACKGROUNDTEXT);
			bw.Write(X);
			bw.Write(Y);
			bw.Write(Width);
			bw.Write(Height);
			bw.Write(Rotation);
			bw.Write(L10NText);
			bw.Write(Config);
		}

		public static BackgroundTextBlueprint Deserialize(BinaryReader br)
		{
			var x = br.ReadSingle();
			var y = br.ReadSingle();
			var w = br.ReadSingle();
			var h = br.ReadSingle();
			var r = br.ReadSingle();
			var t = br.ReadInt32();
			var c = br.ReadUInt16();

			return new BackgroundTextBlueprint(x, y, w, h, r, t, c);
		}
	}
}
