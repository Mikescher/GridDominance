using System.Collections.Generic;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork
{
	public sealed class LaserSource
	{
		public readonly FPoint Position;
		public readonly LaserNetwork Owner;

		public bool LaserActive;
		public bool LaserPowered;
		public Fraction LaserFraction;
		public float LaserRotation;
		public object UserData;

		public readonly List<LaserRay> Lasers = new List<LaserRay>(LaserNetwork.MAX_LASER_PER_SOURCE);

		public LaserSource(LaserNetwork owner, FPoint position, Fraction fracNeutral, object userData)
		{
			Owner = owner;
			Position = position;

			LaserActive = false;
			LaserPowered = false;
			LaserFraction = fracNeutral;
			LaserRotation = 0f;
			UserData = userData;
		}

		public void SetState(bool active, Fraction fraction, float rotation, bool powered)
		{
			if (active   != LaserActive)   { LaserActive   = active;   Owner.Dirty = true; }
			if (rotation != LaserRotation) { LaserRotation = rotation; Owner.Dirty = true; }

			LaserPowered = powered;
			LaserFraction = fraction;
		}
	}
}
