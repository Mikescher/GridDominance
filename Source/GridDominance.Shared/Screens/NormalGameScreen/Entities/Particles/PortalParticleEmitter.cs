using MonoSAMFramework.Portable.Screens.Entities.Particles.CPUParticles;
using System;
using GridDominance.Levelfileformat.Blueprint;
using MonoSAMFramework.Portable.Screens.Entities.Particles;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities.Particles
{
	class DonutParticleEmitter : CPUParticleEmitter
	{
		public override FPoint Position { get; }

		public override FSize DrawingBoundingBox { get; }

		private readonly FPoint _start;
		private readonly Vector2 _direction;
		private readonly Vector2 _normal;

		public DonutParticleEmitter(GDGameScreen scrn, LevelBlueprint bp, FlatAlign4 a) 
			: base(scrn, CreateConfig(bp, a), GDConstants.ORDER_GAME_PORTALPARTICLE)
		{
			Position = new FPoint(bp.LevelWidth / 2f, bp.LevelHeight / 2f);

			switch (a)
			{
				case FlatAlign4.TOP:
					_start = new FPoint(0, 0);
					_direction = new Vector2(bp.LevelWidth, 0);
					_normal = new Vector2(0, 1);
					break;
				case FlatAlign4.RIGHT:
					_start = new FPoint(bp.LevelWidth, 0);
					_direction = new Vector2(0, bp.LevelHeight);
					_normal = new Vector2(-1, 0);
					break;
				case FlatAlign4.BOTTOM:
					_start = new FPoint(bp.LevelWidth, bp.LevelHeight);
					_direction = new Vector2(-bp.LevelWidth, 0);
					_normal = new Vector2(0, -1);
					break;
				case FlatAlign4.LEFT:
					_start = new FPoint(0, bp.LevelHeight);
					_direction = new Vector2(0, -bp.LevelHeight);
					_normal = new Vector2(1, 0);
					break;
			}

			DrawingBoundingBox = new FSize(bp.LevelWidth, bp.LevelHeight);
		}

		private static ParticleEmitterConfig CreateConfig(LevelBlueprint bp, FlatAlign4 a)
		{
			var w = 0f;
			switch (a)
			{
				case FlatAlign4.TOP:
				case FlatAlign4.BOTTOM:
					w = bp.LevelWidth;
					break;
				case FlatAlign4.LEFT:
				case FlatAlign4.RIGHT:
					w = bp.LevelHeight;
					break;
			}
			
			return new ParticleEmitterConfig.ParticleEmitterConfigBuilder
			{
				// star stuff
				TextureIndex = 10,
				SpawnRate = (0.1f * w),

				ParticleAlphaInitial = 0f,
				ParticleAlphaFinal = 1f,

				ParticleLifetimeMin = 1f,
				ParticleLifetimeMax = 2.5f,

				ParticleSizeInitial = 16,
				ParticleSizeFinal   = 0,

				Color = Color.Black,

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

			float u = FloatMath.GetRangedRandom(0, +1f);
			float l = FloatMath.GetRangedRandom(8, 24);
			float t = FloatMath.GetRangedRandom(_config.ParticleLifetimeMin, _config.ParticleLifetimeMax);

			var v = _normal * l;

			particlePool[ParticleCount].Position = _start + _direction * u + v;

			particlePool[ParticleCount].StartPosition = particlePool[ParticleCount].Position;

			particlePool[ParticleCount].Velocity = v / -t;

			particlePool[ParticleCount].MaxLifetime = t;

			particlePool[ParticleCount].SizeInitial = _config.ParticleSizeInitialMin;
			particlePool[ParticleCount].SizeFinal   = _config.ParticleSizeFinalMax;

			ParticleCount++;
		}
	}
}
