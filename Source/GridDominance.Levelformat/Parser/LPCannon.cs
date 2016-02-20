using System;

namespace GridDominance.Levelformat.Parser
{
	public struct LPCannon
	{
		public readonly float X;
		public readonly float Y;
		public readonly float Scale;
		public readonly int Player;
		public readonly float Rotation; // in degree

		public LPCannon(float x, float y, float s, int p, float rot)
		{
			X = x;
			Y = y;
			Scale = s;
			Player = p;

			if (rot < 0)
				Rotation = (float) (Math.Atan2(320 - Y, 512 - X) / Math.PI * 180);
			else
				Rotation = rot;
		}
	}
}
