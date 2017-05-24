using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Screens.Entities.Particles;

namespace GridDominance.Shared.Screens
{
	public static class ParticlePresets
	{
		public static ParticleEmitterConfig.ParticleEmitterConfigBuilder GetConfigLetterGreenGas()
		{
			return new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// green gas
				TextureIndex = 12,
				SpawnRate = 200,

				ParticleLifetimeMin = 0.5f,
				ParticleLifetimeMax = 2.0f,

				ParticleVelocityMin = 1f,
				ParticleVelocityMax = 8f,

				ParticleSizeInitial = 1f,
				ParticleSizeFinalMin = 2f,
				ParticleSizeFinalMax = 32f,

				ParticleAlphaInitial = 1f,
				ParticleAlphaFinal = 0f,
				ColorInitial = Color.Lime,
				ColorFinal = Color.DarkGreen,
			};
		}

		public static ParticleEmitterConfig.ParticleEmitterConfigBuilder GetConfigLetterFireRed()
		{
			return new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// red fire
				TextureIndex = 12,
				SpawnRate = 100,
				ParticleLifetimeMin = 0.5f,
				ParticleLifetimeMax = 1.8f,
				ParticleVelocityMin = 4f,
				ParticleVelocityMax = 24f,
				ParticleSizeInitial = 34,
				ParticleSizeFinalMin = 0,
				ParticleSizeFinalMax = 24,
				ParticleAlphaInitial = 1f,
				ParticleAlphaFinal = 0f,
				ColorInitial = Color.DarkOrange,
				ColorFinal = Color.DarkRed,
			};
		}

		public static ParticleEmitterConfig.ParticleEmitterConfigBuilder GetConfigLetterBlueLines()
		{
			return new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// blue lines
				TextureIndex = 7,
				SpawnRate = 75,
				ParticleLifetimeMin = 0.5f,
				ParticleLifetimeMax = 1.8f,
				ParticleVelocityMin = 4f,
				ParticleVelocityMax = 8f,
				ParticleSizeInitial = 24,
				ParticleSizeFinalMin = 0,
				ParticleSizeFinalMax = 24,
				ParticleAlphaInitial = 1f,
				ParticleAlphaFinal = 0.5f,
				ColorInitial = Color.DeepSkyBlue,
				ColorFinal = Color.Turquoise,
			};
		}

		public static ParticleEmitterConfig.ParticleEmitterConfigBuilder GetConfigLetterGrayLetters()
		{
			return new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// gray bubbles
				TextureIndex = 14,
				SpawnRate = 175,
				ParticleLifetimeMin = 0.5f,
				ParticleLifetimeMax = 1.8f,
				ParticleVelocityMin = 0f,
				ParticleVelocityMax = 8f,
				ParticleSizeInitial = 0,
				ParticleSizeFinalMin = 24,
				ParticleSizeFinalMax = 24,
				ParticleAlphaInitial = 0.2f,
				ParticleAlphaFinal = 1f,
				ColorInitial = Color.DarkGray,
				ColorFinal = Color.DarkSlateGray,
			};
		}

		public static ParticleEmitterConfig.ParticleEmitterConfigBuilder GetConfigLetterGoldenBubbles()
		{
			return new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// golden bubbles 
				TextureIndex = 11,
				SpawnRate = 25,
				ParticleLifetimeMin = 2f,
				ParticleLifetimeMax = 4f,
				ParticleVelocityMin = 4f,
				ParticleVelocityMax = 8f,
				ParticleSizeInitial = 24,
				ParticleSizeFinalMin = 4,
				ParticleSizeFinalMax = 16,
				ParticleAlphaInitial = 1f,
				ParticleAlphaFinal = 0f,
				ColorInitial = Color.DimGray,
				ColorFinal = Color.Gold,
			};
		}

		public static ParticleEmitterConfig.ParticleEmitterConfigBuilder GetConfigLetterStarStuff()
		{
			return new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// star stuff
				TextureIndex = 3,
				SpawnRate = 25,
				ParticleLifetimeMin = 8f,
				ParticleLifetimeMax = 10f,
				ParticleVelocityMin = 1f,
				ParticleVelocityMax = 2f,
				ParticleSizeInitial = 24,
				ParticleSizeFinalMin = 4,
				ParticleSizeFinalMax = 16,
				ParticleAlphaInitial = 1f,
				ParticleAlphaFinal = 1f,
				ColorInitial = Color.Black,
				ColorFinal = Color.Gold,
			};
		}

		public static ParticleEmitterConfig.ParticleEmitterConfigBuilder GetConfigLetterGreenStars()
		{
			return new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// green stars
				TextureIndex = 5,
				SpawnRate = 125,
				ParticleLifetimeMin = 0.8f,
				ParticleLifetimeMax = 1.4f,
				ParticleVelocityMin = 0f,
				ParticleVelocityMax = 24f,
				ParticleSizeInitial = 24,
				ParticleSizeFinalMin = 24,
				ParticleSizeFinalMax = 24,
				ParticleAlphaInitial = 1f,
				ParticleAlphaFinal = 0f,
				ColorInitial = Color.DarkGreen,
				ColorFinal = Color.GreenYellow,
			};
		}

		public static ParticleEmitterConfig.ParticleEmitterConfigBuilder GetConfigFireLetterFineLines()
		{
			return new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// fine lines
				TextureIndex = 7,
				SpawnRate = 25,
				ParticleLifetimeMin = 4f,
				ParticleLifetimeMax = 4f,
				ParticleVelocityMin = 0f,
				ParticleVelocityMax = 0f,
				ParticleSizeInitial = 64,
				ParticleSizeFinalMin = 0,
				ParticleSizeFinalMax = 0,
				ParticleAlphaInitial = 0f,
				ParticleAlphaFinal = 1f,
				ColorInitial = Color.DimGray,
				ColorFinal = Color.Goldenrod,
			};
		}

		public static ParticleEmitterConfig.ParticleEmitterConfigBuilder GetConfigLetterSmokeyFire()
		{
			return new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// smokey fire
				TextureIndex = 12,
				SpawnRate = 125,
				ParticleLifetimeMin = 1.0f,
				ParticleLifetimeMax = 1.5f,
				ParticleVelocityMin = 0f,
				ParticleVelocityMax = 32f,
				ParticleSizeInitial = 8,
				ParticleSizeFinalMin = 24,
				ParticleSizeFinalMax = 32,
				ParticleAlphaInitial = 1f,
				ParticleAlphaFinal = 0f,
				ColorInitial = Color.DarkRed,
				ColorFinal = Color.SlateGray,
			};
		}
	}
}
