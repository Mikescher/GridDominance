using System;
using System.Collections.Generic;
using GridDominance.Shared.Framework;
using Microsoft.Xna.Framework;

namespace GridDominance.Shared.Screens.GameScreen.Background
{
	class BackgroundParticle
	{
		public const float PARTICLE_SPEED = 100;
		public const float ROTATION_SPEED_MIN = 3;
		public const float ROTATION_SPEED_MAX = 9;

		public const int PARTICLE_WIDTH = 8;
		public const float PARTICLE_ALPHA = 0.5f;

		public bool Alive = true;

		public readonly Fraction Fraction;
		public int RemainingLifetime;

		public readonly Direction4 Direction;

		public int OriginX;
		public int OriginY;

		public float X;
		public float Y;

		public float Rotation;
		public float RotationSpeed;

		public List<BackgroundParticle> TravelSection;

		public BackgroundParticle(int ox, int oy, Fraction f, int life, float px, float py, Direction4 dir)
		{
			OriginX = ox;
			OriginY = oy;

			X = px;
			Y = py;

			Fraction = f;
			RemainingLifetime = life;
			Direction = dir;

			Rotation = FloatMath.GetRangedRandom(FloatMath.TAU);
			RotationSpeed = FloatMath.GetRandomSign() * FloatMath.GetRangedRandom(ROTATION_SPEED_MIN, ROTATION_SPEED_MAX);
		}

		public BackgroundParticle(BackgroundParticle source, float px, float py, Direction4 dir)
		{
			OriginX = source.OriginX;
			OriginY = source.OriginY;

			X = px;
			Y = py;

			Fraction = source.Fraction;
			RemainingLifetime = source.RemainingLifetime;
			Direction = dir;

			Rotation = source.Rotation;
			RotationSpeed = source.RotationSpeed;
		}
	}
}
