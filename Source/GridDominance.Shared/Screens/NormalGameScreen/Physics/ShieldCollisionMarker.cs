using GridDominance.Shared.Screens.NormalGameScreen.Entities;

namespace GridDominance.Shared.Screens.NormalGameScreen.Physics
{
	public sealed class ShieldCollisionMarker
	{
		public readonly Cannon Source;

		public bool Active => Source.IsShielded;

		public ShieldCollisionMarker(Cannon src) { Source = src; }
	}
}
