using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using Microsoft.Xna.Framework;

namespace GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork
{
	public sealed class LaserSource
	{
		public readonly Vector2 Position;

		public bool LaserActive;
		public Fraction LaserFraction;
		public float LaserRotation;

		public bool IsDirty;

		public int LaserCount = 0;
		public LaserRay[] Lasers = new LaserRay[LaserNetwork.MAX_LASER_PER_SOURCE];

		public LaserSource(Vector2 position, Fraction fracNeutral)
		{
			Position = position;

			LaserActive = false;
			LaserFraction = fracNeutral;
			LaserRotation = 0f;

			IsDirty = true;
		}

		public void SetState(bool active, Fraction fraction, float rotation)
		{
			if (active   != LaserActive)   { LaserActive   = active;   IsDirty = true; }
			if (rotation != LaserRotation) { LaserRotation = rotation; IsDirty = true; }

			LaserFraction = fraction;
		}
	}
}
