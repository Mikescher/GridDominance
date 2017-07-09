namespace GridDominance.Levelfileformat.Blueprint
{
	public interface ICannonBlueprint
	{
		int CannonID { get; }
		float X { get; }
		float Y { get; }
		float Diameter { get; }
		BulletPathBlueprint[] PrecalculatedPaths { get; }
		int Fraction { get; }
	}
}
