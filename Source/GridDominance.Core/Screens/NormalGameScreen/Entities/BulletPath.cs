using System;
using System.Collections.Generic;
using GridDominance.Shared.Screens.NormalGameScreen.Entities.Cannons;
using Microsoft.Xna.Framework;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class BulletPath
	{
		public readonly Cannon TargetCannon;
		public readonly float CannonRotation;

		public readonly List<Tuple<Vector2, Vector2>> Rays;

		public BulletPath(Cannon c, float rot, List<Tuple<Vector2, Vector2>> trace)
		{
			TargetCannon = c;
			CannonRotation = rot;
			Rays = trace;
		}
	}
}
