namespace GridDominance.Levelformat.Parser
{
	public struct LPCannon
	{
		public readonly float X;
		public readonly float Y;
		public readonly float Radius;
		public readonly int Player;

		public LPCannon(float x, float y, float rad, int p)
		{
			X = x;
			Y = y;
			Radius = rad;
			Player = p;
		}
	}
}
