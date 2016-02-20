namespace Leveleditor.Parser
{
	struct LPCannon
	{
		public readonly double X;
		public readonly double Y;
		public readonly double Radius;
		public readonly int Player;

		public LPCannon(double x, double y, double rad, int p)
		{
			X = x;
			Y = y;
			Radius = rad;
			Player = p;
		}
	}
}
