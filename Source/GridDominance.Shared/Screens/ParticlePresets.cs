using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath.VectorPath;
using MonoSAMFramework.Portable.Screens.Entities.Particles;

namespace GridDominance.Shared.Screens
{
	public static class ParticlePresets
	{
		public static ParticleEmitterConfig GetConfigLetterFireRed(float size, char chr)
		{
			return new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// red fire
				Texture = Textures.TexParticle[12],
				SpawnRate = 100 * PathPresets.LETTERS[chr].Length,
				ParticleLifetimeMin = 0.5f,
				ParticleLifetimeMax = 1.8f,
				ParticleVelocityMin = 4f * size,
				ParticleVelocityMax = 24f * size,
				ParticleSizeInitial = 24 * size,
				ParticleSizeFinalMin = 0 * size,
				ParticleSizeFinalMax = 24 * size,
				ParticleAlphaInitial = 1f,
				ParticleAlphaFinal = 0f,
				ColorInitial = Color.DarkOrange,
				ColorFinal = Color.DarkRed,
			}.Build();
		}

		public static ParticleEmitterConfig GetConfigLetterBlueLines(float size, char chr)
		{
			return new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// blue lines
				Texture = Textures.TexParticle[7],
				SpawnRate = 75 * PathPresets.LETTERS[chr].Length,
				ParticleLifetimeMin = 0.5f,
				ParticleLifetimeMax = 1.8f,
				ParticleVelocityMin = 4f * size,
				ParticleVelocityMax = 8f * size,
				ParticleSizeInitial = 24 * size,
				ParticleSizeFinalMin = 0 * size,
				ParticleSizeFinalMax = 24 * size,
				ParticleAlphaInitial = 1f,
				ParticleAlphaFinal = 0.5f,
				ColorInitial = Color.DeepSkyBlue,
				ColorFinal = Color.Turquoise,
			}.Build();
		}

		public static ParticleEmitterConfig GetConfigLetterGrayLetters(float size, char chr)
		{
			return new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// gray letters
				Texture = Textures.TexParticle[14],
				SpawnRate = 175 * PathPresets.LETTERS[chr].Length,
				ParticleLifetimeMin = 0.5f,
				ParticleLifetimeMax = 1.8f,
				ParticleVelocityMin = 0f * size,
				ParticleVelocityMax = 8f * size,
				ParticleSizeInitial = 0 * size,
				ParticleSizeFinalMin = 24 * size,
				ParticleSizeFinalMax = 24 * size,
				ParticleAlphaInitial = 0.2f,
				ParticleAlphaFinal = 1f,
				ColorInitial = Color.DarkGray,
				ColorFinal = Color.DarkSlateGray,
			}.Build();
		}

		public static ParticleEmitterConfig GetConfigLetterGoldenBubbles(float size, char chr)
		{
			return new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// golden bubbles 
				Texture = Textures.TexParticle[11],
				SpawnRate = 25 * PathPresets.LETTERS[chr].Length,
				ParticleLifetimeMin = 2f,
				ParticleLifetimeMax = 4f,
				ParticleVelocityMin = 4f * size,
				ParticleVelocityMax = 8f * size,
				ParticleSizeInitial = 24 * size,
				ParticleSizeFinalMin = 4 * size,
				ParticleSizeFinalMax = 16 * size,
				ParticleAlphaInitial = 1f,
				ParticleAlphaFinal = 0f,
				ColorInitial = Color.DimGray,
				ColorFinal = Color.Gold,
			}.Build();
		}

		public static ParticleEmitterConfig GetConfigLetterStarStuff(float size, char chr)
		{
			return new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// star stuff
				Texture = Textures.TexParticle[3],
				SpawnRate = 25 * PathPresets.LETTERS[chr].Length,
				ParticleLifetimeMin = 8f,
				ParticleLifetimeMax = 10f,
				ParticleVelocityMin = 1f * size,
				ParticleVelocityMax = 2f * size,
				ParticleSizeInitial = 24 * size,
				ParticleSizeFinalMin = 4 * size,
				ParticleSizeFinalMax = 16 * size,
				ParticleAlphaInitial = 1f,
				ParticleAlphaFinal = 1f,
				ColorInitial = Color.Black,
				ColorFinal = Color.Gold,
			}.Build();
		}

		public static ParticleEmitterConfig GetConfigLetterGreenStars(float size, char chr)
		{
			return new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// green stars
				Texture = Textures.TexParticle[5],
				SpawnRate = 125 * PathPresets.LETTERS[chr].Length,
				ParticleLifetimeMin = 0.8f,
				ParticleLifetimeMax = 1.4f,
				ParticleVelocityMin = 0f * size,
				ParticleVelocityMax = 24f * size,
				ParticleSizeInitial = 24 * size,
				ParticleSizeFinalMin = 24 * size,
				ParticleSizeFinalMax = 24 * size,
				ParticleAlphaInitial = 1f,
				ParticleAlphaFinal = 0f,
				ColorInitial = Color.DarkGreen,
				ColorFinal = Color.GreenYellow,
			}.Build();
		}

		public static ParticleEmitterConfig GetConfigFireLetterFineLines(float size, char chr)
		{
			return new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// fine lines
				Texture = Textures.TexParticle[7],
				SpawnRate = 25 * PathPresets.LETTERS[chr].Length,
				ParticleLifetimeMin = 4f,
				ParticleLifetimeMax = 4f,
				ParticleVelocityMin = 0f * size,
				ParticleVelocityMax = 0f * size,
				ParticleSizeInitial = 64 * size,
				ParticleSizeFinalMin = 0 * size,
				ParticleSizeFinalMax = 0 * size,
				ParticleAlphaInitial = 0f,
				ParticleAlphaFinal = 1f,
				ColorInitial = Color.DimGray,
				ColorFinal = Color.Goldenrod,
			}.Build();
		}

		public static ParticleEmitterConfig GetConfigLetterSmokeyFire(float size, char chr)
		{
			return new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// smokey fire
				Texture = Textures.TexParticle[12],
				SpawnRate = 125 * PathPresets.LETTERS[chr].Length,
				ParticleLifetimeMin = 1.0f,
				ParticleLifetimeMax = 1.5f,
				ParticleVelocityMin = 0f * size,
				ParticleVelocityMax = 32f * size,
				ParticleSizeInitial = 8 * size,
				ParticleSizeFinalMin = 24 * size,
				ParticleSizeFinalMax = 32 * size,
				ParticleAlphaInitial = 1f,
				ParticleAlphaFinal = 0f,
				ColorInitial = Color.DarkRed,
				ColorFinal = Color.SlateGray,
			}.Build();
		}
	}
}
