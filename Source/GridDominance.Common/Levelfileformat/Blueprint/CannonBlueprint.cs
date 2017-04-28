using System;

namespace GridDominance.Levelfileformat.Blueprint
{
	public struct CannonBlueprint
	{
		public readonly float X;
		public readonly float Y;
		public readonly float Diameter;
		public readonly int Player;
		public readonly float Rotation; // in degree
		public readonly int CannonID;

		public CannonBlueprint(float x, float y, float d, int p, float rot, int cid)
		{
			X = x;
			Y = y;
			Diameter = d;
			Player = p;
			CannonID = cid;

			if (rot < 0)
				Rotation = (float) (Math.Atan2(320 - Y, 512 - X) / Math.PI * 180);
			else
				Rotation = rot;
		}
	}
}
