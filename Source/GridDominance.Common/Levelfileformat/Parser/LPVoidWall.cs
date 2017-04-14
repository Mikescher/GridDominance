using System;

namespace GridDominance.Levelfileformat.Parser
{
	public struct LPVoidWall
	{
		public readonly float X;
		public readonly float Y;
		public readonly float Length;
		public readonly float Rotation; // in degree

		public LPVoidWall(float x, float y, float l, float rot)
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
	}
}
