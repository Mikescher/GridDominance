using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using Microsoft.Xna.Framework;

namespace GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork
{
	public sealed class LaserSource
	{
		public readonly Vector2 Position;
		public readonly LaserNetwork Owner;

		public bool LaserActive;
		public Fraction LaserFraction;
		public float LaserRotation;
		public object UserData;

		public int LaserCount = 0;
		public LaserRay[] Lasers = new LaserRay[LaserNetwork.MAX_LASER_PER_SOURCE];

		public LaserSource(LaserNetwork owner, Vector2 position, Fraction fracNeutral, object userData)
		{
			Owner = owner;
			Position = position;

			LaserActive = false;
			LaserFraction = fracNeutral;
			LaserRotation = 0f;
			UserData = userData;
		}

		public void SetState(bool active, Fraction fraction, float rotation)
		{
			if (active   != LaserActive)   { LaserActive   = active;   Owner.Dirty = true; }
			if (rotation != LaserRotation) { LaserRotation = rotation; Owner.Dirty = true; }

			LaserFraction = fraction;
		}
	}
}
