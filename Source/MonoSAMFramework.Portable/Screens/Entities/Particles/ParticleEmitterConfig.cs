using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles
{
	public class ParticleEmitterConfig
	{
		public class ParticleEmitterConfigBuilder
		{
			public TextureRegion2D Texture;

			public float SpawnRateMin = 0f; // particles per second
			public float SpawnRateMax = 0f; // particles per second
			public float? SpawnRate
			{
				get { return FloatMath.FloatEquals(SpawnRateMin, SpawnRateMax) ? SpawnRateMin : (float?)null; }
				set { if (value != null) { SpawnRateMin = value.Value; SpawnRateMax = value.Value; } }
			}

			public float ParticleLifetimeMin = 0;
			public float ParticleLifetimeMax = 0;
			public float? ParticleLifetime
			{
				get { return FloatMath.FloatEquals(ParticleLifetimeMin, ParticleLifetimeMax) ? ParticleLifetimeMin : (float?) null; }
				set { if (value != null) { ParticleLifetimeMin = value.Value; ParticleLifetimeMax = value.Value;}}
			}

			public float ParticleSpawnAngleMin = 0;
			public float ParticleSpawnAngleMax = FloatMath.TAU;
			public float? ParticleSpawnAngle
			{
				get { return FloatMath.FloatEquals(ParticleSpawnAngleMin, ParticleSpawnAngleMax) ? ParticleSpawnAngleMin : (float?) null; }
				set { if (value != null) { ParticleSpawnAngleMin = value.Value; ParticleSpawnAngleMax = value.Value;}}
			}

			public float ParticleVelocityMin = 0;
			public float ParticleVelocityMax = FloatMath.TAU;
			public float? ParticleVelocity
			{
				get { return FloatMath.FloatEquals(ParticleVelocityMin, ParticleVelocityMax) ? ParticleVelocityMin : (float?)null; }
				set { if (value != null) { ParticleVelocityMin = value.Value; ParticleVelocityMax = value.Value; } }
			}

			public float ParticleAlphaInitial = 1f;
			public float ParticleAlphaFinal = 1f;
			public float? ParticleAlpha
			{
				get { return FloatMath.FloatEquals(ParticleAlphaInitial, ParticleAlphaFinal) ? ParticleAlphaInitial : (float?)null; }
				set { if (value != null) { ParticleAlphaInitial = value.Value; ParticleAlphaFinal = value.Value; } }
			}

			public float ParticleSizeInitialMin = 1f;
			public float ParticleSizeInitialMax = 1f;
			public float? ParticleSizeInitial
			{
				get { return FloatMath.FloatEquals(ParticleSizeInitialMin, ParticleSizeInitialMax) ? ParticleSizeInitialMin : (float?)null; }
				set { if (value != null) { ParticleSizeInitialMin = value.Value; ParticleSizeInitialMax = value.Value; } }
			}

			public float ParticleSizeFinalMin = 1f;
			public float ParticleSizeFinalMax = 1f;
			public float? ParticleSizeFinal
			{
				get { return FloatMath.FloatEquals(ParticleSizeFinalMin, ParticleSizeFinalMax) ? ParticleSizeFinalMin : (float?)null; }
				set { if (value != null) { ParticleSizeFinalMin = value.Value; ParticleSizeFinalMax = value.Value; } }
			}

			public float? ParticleSize
			{
				get { return FloatMath.FloatEquals(ParticleSizeInitialMin, ParticleSizeInitialMax, ParticleSizeFinalMin, ParticleSizeFinalMax) ? ParticleSizeInitialMin : (float?)null; }
				set { if (value != null) { ParticleSizeInitialMin = value.Value; ParticleSizeInitialMax = value.Value; ParticleSizeFinalMin = value.Value; ParticleSizeFinalMax = value.Value; } }
			}

			public Color Color = Color.White;

			public ParticleEmitterConfig Build() => new ParticleEmitterConfig(this);
		}

		private static readonly Vector2 vectorOne = Vector2.UnitX;

		public readonly bool Active;

		public readonly TextureRegion2D Texture;
		public readonly Rectangle TextureBounds;
		public readonly Vector2 TextureCenter;
		public readonly float TextureSize;

		public readonly float SpawnRateMin;
		public readonly float SpawnRateMax;

		public readonly float SpawnDelayMin;
		public readonly float SpawnDelayMax;
		public readonly bool SpawnDelayIsRandom;

		public readonly float ParticleLifetimeMin;
		public readonly float ParticleLifetimeMax;
		public readonly bool ParticleLifetimeIsRandom;

		public readonly float ParticleSpawnAngleMin;
		public readonly float ParticleSpawnAngleMax;
		public readonly bool ParticleSpawnAngleIsRandom;
		public readonly bool ParticleSpawnAngleIsTotal;
		public readonly Vector2 FixedParticleSpawnAngle;

		public readonly float ParticleVelocityMin;
		public readonly float ParticleVelocityMax;
		public readonly bool ParticleVelocityIsRandom;

		public readonly float ParticleAlphaInitial;
		public readonly float ParticleAlphaFinal;

		public readonly float ParticleSizeInitialMin;
		public readonly float ParticleSizeInitialMax;
		public readonly bool ParticleSizeInitialIsRandom;

		public readonly float ParticleSizeFinalMin;
		public readonly float ParticleSizeFinalMax;
		public readonly bool ParticleSizeFinalIsRandom;

		public Color Color;

		internal ParticleEmitterConfig(ParticleEmitterConfigBuilder b)
		{
			Texture = b.Texture;
			TextureBounds = Texture.Bounds;
			TextureCenter = Texture.Center();
			TextureSize = TextureBounds.Width;

			SpawnRateMin = b.SpawnRateMin;
			SpawnRateMax = b.SpawnRateMax;

			SpawnDelayMin = 1f / b.SpawnRateMin;
			SpawnDelayMax= 1f / b.SpawnRateMax;
			SpawnDelayIsRandom = FloatMath.FloatInequals(SpawnRateMin, SpawnRateMax);

			ParticleLifetimeMin = b.ParticleLifetimeMin;
			ParticleLifetimeMax = b.ParticleLifetimeMax;
			ParticleLifetimeIsRandom = FloatMath.FloatInequals(ParticleLifetimeMin, ParticleLifetimeMax);

			ParticleSpawnAngleMin = b.ParticleSpawnAngleMin;
			ParticleSpawnAngleMax = b.ParticleSpawnAngleMax;
			ParticleSpawnAngleIsRandom = FloatMath.FloatInequals(ParticleSpawnAngleMin, ParticleSpawnAngleMax);
			ParticleSpawnAngleIsTotal = FloatMath.FloatInequals(ParticleSpawnAngleMin, ParticleSpawnAngleMax) && FloatMath.EpsilonEquals(FloatMath.PositiveModulo(ParticleSpawnAngleMin, FloatMath.TAU), FloatMath.PositiveModulo(ParticleSpawnAngleMax, FloatMath.TAU));
			if (!ParticleSpawnAngleIsRandom) FixedParticleSpawnAngle = vectorOne.Rotate(ParticleSpawnAngleMin);

			ParticleVelocityMin = b.ParticleVelocityMin;
			ParticleVelocityMax = b.ParticleVelocityMax;
			ParticleVelocityIsRandom = FloatMath.FloatInequals(ParticleVelocityMin, ParticleVelocityMax);

			ParticleAlphaInitial = b.ParticleAlphaInitial;
			ParticleAlphaFinal = b.ParticleAlphaFinal;

			ParticleSizeInitialMin = b.ParticleSizeInitialMin;
			ParticleSizeInitialMax = b.ParticleSizeInitialMax;
			ParticleSizeInitialIsRandom = FloatMath.FloatInequals(ParticleSizeInitialMin, ParticleSizeInitialMax);

			ParticleSizeFinalMin = b.ParticleSizeFinalMin;
			ParticleSizeFinalMax = b.ParticleSizeFinalMax;
			ParticleSizeFinalIsRandom = FloatMath.FloatInequals(ParticleSizeFinalMin, ParticleSizeFinalMax);

			Color = b.Color;

			Active = (Texture != null) && (SpawnRateMin > 0) && (ParticleLifetimeMin > 0);
		}

		public float GetParticleLifetime()
		{
			if (ParticleLifetimeIsRandom)
				return FloatMath.GetRangedRandom(ParticleLifetimeMin, ParticleLifetimeMax);
			else
				return ParticleLifetimeMin;
		}

		public float GetParticleAngle()
		{
			if (ParticleSpawnAngleIsRandom)
				return FloatMath.GetRangedRandom(ParticleSpawnAngleMin, ParticleSpawnAngleMax);
			else
				return ParticleSpawnAngleMin;
		}

		public void SetParticleVelocity(ref Vector2 vec)
		{
			var velocity = GetParticleVelocity();

			if (ParticleSpawnAngleIsRandom)
			{
				var rvec = vectorOne.Rotate(GetParticleAngle());

				vec.X = rvec.X * velocity;
				vec.Y = rvec.Y * velocity;
			}
			else
			{
				vec.X = FixedParticleSpawnAngle.X * velocity;
				vec.Y = FixedParticleSpawnAngle.Y * velocity;
			}
		}

		public float GetParticleVelocity()
		{
			if (ParticleVelocityIsRandom)
				return FloatMath.GetRangedRandom(ParticleVelocityMin, ParticleVelocityMax);
			else
				return ParticleVelocityMin;
		}

		public float GetParticleSizeInitial()
		{
			if (ParticleSizeInitialIsRandom)
				return FloatMath.GetRangedRandom(ParticleSizeInitialMin, ParticleSizeInitialMax);
			else
				return ParticleSizeInitialMin;
		}

		public float GetParticleSizeFinal()
		{
			if (ParticleSizeFinalIsRandom)
				return FloatMath.GetRangedRandom(ParticleSizeFinalMin, ParticleSizeFinalMax);
			else
				return ParticleSizeFinalMin;
		}

		public float GetSpawnDelay()
		{
			if (SpawnDelayIsRandom)
				return FloatMath.GetRangedRandom(SpawnDelayMin, SpawnDelayMax);
			else
				return SpawnDelayMin;
		}
	}
}
