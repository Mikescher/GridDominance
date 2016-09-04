using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles
{
	/// <summary>
	/// https://github.com/CartBlanche/MonoGame-Samples/blob/master/Particle3DSample
	/// </summary>
	public abstract class ParticleEmitter : GameEntity
	{
		private const int PARTICLE_POOL_SAFETY = 4; // always add X elements more to pool than calculated

		public override Color DebugIdentColor => Color.Gold * 0.1f;

		private float timeSinceLastSpawn = 0f;
		private float spawnDelay = 0f;

		private ParticleEmitterConfig _config;
		public ParticleEmitterConfig Config { get { return _config; } set { _config = value; RecalculateState(); } }

		private Effect particleEffect;

		private Particle[] particlePool;
		private DynamicVertexBuffer vertexBuffer;
		private IndexBuffer indexBuffer;

		public int ParticleCount { get; private set; } = 0;

		protected ParticleEmitter(GameScreen scrn, ParticleEmitterConfig cfg) : base(scrn)
		{
			_config = cfg;
		}

		protected virtual void RecalculateState()
		{
			int maxParticleCount = FloatMath.Ceiling(_config.SpawnRateMax * _config.ParticleLifetimeMax) + PARTICLE_POOL_SAFETY;

			particlePool = new Particle[maxParticleCount];
			for (int i = 0; i < maxParticleCount; i++) particlePool[i] = new Particle();

			spawnDelay = _config.GetSpawnDelay();
			timeSinceLastSpawn = 0f;
			ParticleCount = 0;

			//-------

			particleEffect = Owner.Game.Content.Load<Effect>("SAMParticleEffect");
			particleEffect.Parameters["TextureSampler"].SetValue(_config.Texture.Texture);
		}

		public override void OnInitialize()
		{
			RecalculateState();
		}

		public override void OnRemove()
		{
			// NOP
		}

		protected override void OnUpdate(GameTime gameTime, InputState istate)
		{
			if (!IsInViewport) return; // No drawing - no updating (state is frozen)

			for (int i = ParticleCount-1; i >= 0; i--)
			{
				particlePool[i].CurrentLifetime += gameTime.GetElapsedSeconds();
				if (particlePool[i].CurrentLifetime >= particlePool[i].MaxLifetime)
				{
					RemoveParticle(i);
				}
				else
				{
					particlePool[i].Position.X += particlePool[i].Velocity.X * gameTime.GetElapsedSeconds();
					particlePool[i].Position.Y += particlePool[i].Velocity.Y * gameTime.GetElapsedSeconds();
				}
			}

			if (_config.Active)
			{
				timeSinceLastSpawn += gameTime.GetElapsedSeconds();

				while (timeSinceLastSpawn >= spawnDelay)
				{
					SpawnParticle();
					timeSinceLastSpawn -= spawnDelay;

					spawnDelay = _config.GetSpawnDelay();
				}
			}
		}

		private void SpawnParticle()
		{
			if (ParticleCount >= particlePool.Length) return; // Could happen if we lag big time

			particlePool[ParticleCount].CurrentLifetime = 0;

			particlePool[ParticleCount].MaxLifetime = _config.GetParticleLifetime();

			SetParticleSpawnPosition(ref particlePool[ParticleCount].Position);
			particlePool[ParticleCount].StartPosition.X = particlePool[ParticleCount].Position.X;
			particlePool[ParticleCount].StartPosition.Y = particlePool[ParticleCount].Position.Y;

			_config.SetParticleVelocity(ref particlePool[ParticleCount].Velocity);

			particlePool[ParticleCount].SizeInitial = _config.GetParticleSizeInitial();

			particlePool[ParticleCount].SizeFinal = _config.GetParticleSizeFinal();

			ParticleCount++;
		}

		protected abstract void SetParticleSpawnPosition(ref Vector2 vec);

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
					sbatch.Draw(
						_config.Texture.Texture,
						p.Position,
						_config.TextureBounds,
						color * alpha,
						0f,
						_config.TextureCenter,
						size / _config.TextureSize,
						SpriteEffects.None,
						0);
				}
			}
		}

		protected override void DrawDebugBorders(IBatchRenderer sbatch)
		{
			base.DrawDebugBorders(sbatch);

			sbatch.DrawRectangle(Position - new FSize(8,8) * 0.5f, new FSize(8, 8), Color.LightGreen, 1);

			for (int i = 0; i < ParticleCount; i++)
			{
				var p = particlePool[i];

				sbatch.DrawLine(p.StartPosition, p.Position, Color.GreenYellow * 0.5f);
			}
		}
	}
}
