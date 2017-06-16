using MonoSAMFramework.Portable.Screens.Entities.Particles.CPUParticles;
using System;
using MonoSAMFramework.Portable.Screens.Entities.Particles;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Extensions;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities.Particles
{
	class PortalParticleEmitter : CPUParticleEmitter
	{
		public override FPoint Position { get; }

		public override FSize DrawingBoundingBox { get; }

		private readonly Portal _portal;

		public PortalParticleEmitter(Portal p) : base(p.Owner, CreateConfig(p), GDConstants.ORDER_GAME_PORTALPARTICLE)
		{
			Position = p.Position;
			_portal = p;

			DrawingBoundingBox = new FSize(p.DrawingBoundingBox.Width + 128, p.DrawingBoundingBox.Height + 128);
		}

		private static ParticleEmitterConfig CreateConfig(Portal p)
		{
			return new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// star stuff
				TextureIndex = 10,
				SpawnRate = (0.1f * p.Length),

				ParticleAlphaInitial = 0f,
				ParticleAlphaFinal = 1f,

				ParticleLifetimeMin = 1f,
				ParticleLifetimeMax = 2.5f,

				ParticleSizeInitial = 32,
				ParticleSizeFinal   = 0,

				Color = p.Color,

			}.Build(Textures.TexParticle);

		}

		protected override FPoint SetParticleSpawnPosition(out bool doSpawn)
		{
			throw new NotSupportedException();
		}


		protected override void SpawnParticle()
		{
			if (ParticleCount >= particlePool.Length) return; // Could happen if we lag big time

			particlePool[ParticleCount].CurrentLifetime = 0;

			float u = FloatMath.GetRangedRandom(-1f, +1f);
			float a = FloatMath.GetRangedRandom(FloatMath.RAD_NEG_030, FloatMath.RAD_POS_030);
			float l = FloatMath.GetRangedRandom(16, 48);
			float t = FloatMath.GetRangedRandom(_config.ParticleLifetimeMin, _config.ParticleLifetimeMax);

			var v = _portal.VecNormal.Rotate(a) * l;

			particlePool[ParticleCount].Position = _portal.Position + _portal.VecDirection * u + v;

			particlePool[ParticleCount].StartPosition.X = particlePool[ParticleCount].Position.X;
			particlePool[ParticleCount].StartPosition.Y = particlePool[ParticleCount].Position.Y;

			particlePool[ParticleCount].Velocity = v / -t;

			particlePool[ParticleCount].MaxLifetime = t;

			particlePool[ParticleCount].SizeInitial = _config.ParticleSizeInitialMin;
			particlePool[ParticleCount].SizeFinal   = _config.ParticleSizeFinalMax;

			ParticleCount++;
		}
	}
}
