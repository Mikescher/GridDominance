namespace GridDominance.Levelfileformat.Blueprint
{
	public struct VoidCircleBlueprint
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
	}
}
