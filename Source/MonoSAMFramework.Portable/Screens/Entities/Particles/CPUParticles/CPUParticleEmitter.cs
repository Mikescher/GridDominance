﻿using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles.CPUParticles
{
	public abstract class CPUParticleEmitter : GameEntity, IParticleOwner, IParticleEmitter
	{
		private const int PARTICLE_POOL_SAFETY = 4; // always add X elements more to pool than calculated

		public override Color DebugIdentColor => Color.Gold * 0.1f;

		private float timeSinceLastSpawn = 0f;
		private float spawnDelay = 0f;
		protected float internalTime = 0;

		protected ParticleEmitterConfig _config;
		public ParticleEmitterConfig Config { get { return _config; } set { _config = value; RecalculateState(); } }

		protected CPUParticle[] particlePool;

		public int ParticleCount { get; protected set; } = 0;

		public bool IsEnabled = true;
		bool IParticleEmitter.Enabled { get => IsEnabled; set => IsEnabled = value; }
		bool IParticleEmitter.Alive { get => Alive; set => Alive = value; }

		protected CPUParticleEmitter(GameScreen scrn, ParticleEmitterConfig cfg, int order) : base(scrn, order)
		{
			_config = cfg;
		}

		protected virtual void RecalculateState()
		{
			int maxParticleCount = FloatMath.Ceiling(_config.SpawnRate * _config.ParticleLifetimeMax) + PARTICLE_POOL_SAFETY;

			particlePool = new CPUParticle[maxParticleCount];
			for (int i = 0; i < maxParticleCount; i++) particlePool[i] = new CPUParticle();

			spawnDelay = _config.SpawnDelay;
			timeSinceLastSpawn = 0f;
			ParticleCount = 0;
		}

		public override void OnInitialize(EntityManager manager)
		{
			RecalculateState();
		}

		public override void OnRemove()
		{
			// NOP
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			if (!IsInViewport) return; // No drawing - no updating (state is frozen)
			if (!IsEnabled) return;

			internalTime += gameTime.ElapsedSeconds;

			for (int i = ParticleCount - 1; i >= 0; i--)
			{
				particlePool[i].CurrentLifetime += gameTime.ElapsedSeconds;
				if (particlePool[i].CurrentLifetime >= particlePool[i].MaxLifetime)
				{
					RemoveParticle(i);
				}
				else
				{
					particlePool[i].Position += particlePool[i].Velocity * gameTime.ElapsedSeconds;
				}
			}

			timeSinceLastSpawn += gameTime.ElapsedSeconds;
			while (timeSinceLastSpawn >= spawnDelay)
			{
				SpawnParticle();
				timeSinceLastSpawn -= spawnDelay;

				spawnDelay = _config.SpawnDelay;
			}
		}

		protected virtual void SpawnParticle()
		{
			if (ParticleCount >= particlePool.Length) return; // Could happen if we lag big time

			particlePool[ParticleCount].CurrentLifetime = 0;

			particlePool[ParticleCount].MaxLifetime = _config.GetParticleLifetime();

			bool doSpawn;

			particlePool[ParticleCount].Position = SetParticleSpawnPosition(out doSpawn);

			if (!doSpawn) return;

			particlePool[ParticleCount].StartPosition = particlePool[ParticleCount].Position;

			_config.SetParticleVelocity(ref particlePool[ParticleCount].Velocity);

			particlePool[ParticleCount].SizeInitial = _config.GetParticleSizeInitial();

			particlePool[ParticleCount].SizeFinal = _config.GetParticleSizeFinal();

			ParticleCount++;
		}

		protected abstract FPoint SetParticleSpawnPosition(out bool doSpawn);

		private void RemoveParticle(int idx)
		{
			if (idx == ParticleCount - 1)
			{
				ParticleCount--;
				return;
			}

			var tmp = particlePool[idx];
			particlePool[idx] = particlePool[ParticleCount - 1];
			particlePool[ParticleCount - 1] = tmp;

			ParticleCount--;
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			if (!IsEnabled) return;

			for (int i = 0; i < ParticleCount; i++)
			{
				var p = particlePool[i];
				var progress = p.CurrentLifetime / p.MaxLifetime;

				var size = FloatMath.Lerp(p.SizeInitial, p.SizeFinal, progress);
				var alpha = FloatMath.Lerp(_config.ParticleAlphaInitial, _config.ParticleAlphaFinal, progress);
				var color = _config.ColorInitial;
				if (_config.ColorIsChanging)
					color = ColorMath.Blend(_config.ColorInitial, _config.ColorFinal, progress);

				if (size > 0)
				{
					sbatch.DrawScaled(_config.Texture, p.Position, size / (_config.TextureSize * sbatch.TextureScale), color * alpha, 0f);
				}
			}
		}

		protected override void DrawDebugBorders(IBatchRenderer sbatch)
		{
			if (!IsEnabled) return;

			base.DrawDebugBorders(sbatch);

			sbatch.DrawRectangle(Position - new Vector2(8, 8) * 0.5f, new FSize(8, 8), Color.LightGreen, 1);

			for (int i = 0; i < ParticleCount; i++)
			{
				var p = particlePool[i];

				sbatch.DrawLine(p.StartPosition, p.Position, Color.GreenYellow * 0.5f);
			}
		}
	}
}
