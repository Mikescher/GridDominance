using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles
{
	/// <summary>
	/// https://github.com/CartBlanche/MonoGame-Samples/blob/master/Particle3DSample
	/// </summary>
	public abstract class ParticleEmitter : GameEntity, ISAMPostDrawable
	{
		private const int PARTICLE_POOL_SAFETY = 4; // always add X elements more to pool than calculated

		public override Color DebugIdentColor => Color.Gold * 0.1f;

		private float timeSinceLastSpawn = 0f;
		private float spawnDelay = 0f;

		private ParticleEmitterConfig _config;
		public ParticleEmitterConfig Config { get { return _config; } set { _config = value; RecalculateState(); } }

		private Effect particleEffect;
		private EffectParameter parameterOffset;
		private EffectParameter parameterVirtualViewport;
		private EffectParameter parameterCurrentTime;

		private Particle[] particlePool;
		private ParticleVBA vboArray;
		private float internalTime;

		private DynamicVertexBuffer vertexBuffer;
		private IndexBuffer indexBuffer;

		public int ParticleCount { get; private set; } = 0;

		protected ParticleEmitter(GameScreen scrn, ParticleEmitterConfig cfg) : base(scrn)
		{
			_config = cfg;
		}

		protected virtual void RecalculateState()
		{
			spawnDelay = _config.GetSpawnDelay();
			timeSinceLastSpawn = 0f;
			ParticleCount = 0;

			LoadShader();
		}

		private void LoadShader()
		{
			int maxParticleCount = FloatMath.Ceiling(_config.SpawnRateMax * _config.ParticleLifetimeMax) + PARTICLE_POOL_SAFETY;

			// Create Particle Pool

			particlePool = new Particle[maxParticleCount];
			vboArray = new ParticleVBA(maxParticleCount);
			for (int i = 0; i < maxParticleCount; i++) particlePool[i] = new Particle(vboArray, i);


			// Create VertexBuffer and IndexBuffer

			vertexBuffer = new DynamicVertexBuffer(Owner.GraphicsDevice, ParticleVBO.VertexDeclaration, maxParticleCount * 4, BufferUsage.WriteOnly);
			vertexBuffer.SetData(vboArray.Data);

			short[] indices = new short[maxParticleCount * 6];
			for (short i = 0; i < maxParticleCount; i++)
			{
				// TR triangle
				indices[i * 6 + 0] = (short)(i * 4 + 0);
				indices[i * 6 + 1] = (short)(i * 4 + 2);
				indices[i * 6 + 2] = (short)(i * 4 + 1);

				// BL triangle
				indices[i * 6 + 3] = (short)(i * 4 + 0);
				indices[i * 6 + 4] = (short)(i * 4 + 3);
				indices[i * 6 + 5] = (short)(i * 4 + 2);
			}
			indexBuffer = new IndexBuffer(Owner.GraphicsDevice, typeof(short), maxParticleCount * 6, BufferUsage.WriteOnly);
			indexBuffer.SetData(indices);

			// Load effect

			particleEffect = Owner.Game.Content.Load<Effect>("shaders/SAMParticleEffect").Clone();
			parameterOffset = particleEffect.Parameters["Offset"]; 
			parameterVirtualViewport = particleEffect.Parameters["VirtualViewport"];
			parameterCurrentTime = particleEffect.Parameters["CurrentTime"];

			particleEffect.Parameters["ColorInitial"].SetValue(Config.ColorInitial.ToVector4(Config.ParticleAlphaInitial));
			particleEffect.Parameters["ColorFinal"].SetValue(Config.ColorFinal.ToVector4(Config.ParticleAlphaInitial));
			particleEffect.Parameters["Texture"].SetValue(Config.Texture.Texture);
			particleEffect.Parameters["TextureProjection"].SetValue(Config.Texture.GetShaderProjectionMatrix().ToMatrix());

			internalTime = 0;
		}

		public override void OnInitialize(EntityManager manager)
		{
			RecalculateState();

			manager.RegisterPostDraw(this);
		}

		public override void OnRemove()
		{
			// NOP
		}

		protected override void OnUpdate(GameTime gameTime, InputState istate)
		{
			if (!IsInViewport) return; // No drawing - no updating (state is frozen)

			internalTime += gameTime.GetElapsedSeconds();

			for (int i = ParticleCount-1; i >= 0; i--)
			{
				if (internalTime - particlePool[i].StartLifetime > particlePool[i].MaxLifetime)
				{
					RemoveParticle(i);
				}
			}

			if (_config.Active)
			{
				timeSinceLastSpawn += gameTime.GetElapsedSeconds();

				while (timeSinceLastSpawn >= spawnDelay)
				{
					SpawnParticle(internalTime);
					timeSinceLastSpawn -= spawnDelay;

					spawnDelay = _config.GetSpawnDelay();
				}
			}
		}

		private void SpawnParticle(float time)
		{
			if (ParticleCount >= particlePool.Length) return; // Could happen if we lag big time

			particlePool[ParticleCount].MaxLifetime = _config.GetParticleLifetime();
			SetParticleSpawnPosition(ref particlePool[ParticleCount].StartPosition);
			_config.SetParticleVelocity(ref particlePool[ParticleCount].Velocity);
			particlePool[ParticleCount].SizeInitial = _config.GetParticleSizeInitial();
			particlePool[ParticleCount].SizeFinal = _config.GetParticleSizeFinal();

			particlePool[ParticleCount].Init(time);
			
			vertexBuffer.SetData(vboArray.Data, 0, (ParticleCount + 1) * 4); //TODO there should be a way to only send the four new VBOs instead of all at once

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

			particlePool[idx].MaxLifetime = particlePool[ParticleCount - 1].MaxLifetime;
			particlePool[idx].SizeInitial = particlePool[ParticleCount - 1].SizeInitial;
			particlePool[idx].SizeFinal = particlePool[ParticleCount - 1].SizeFinal;
			particlePool[idx].StartLifetime = particlePool[ParticleCount - 1].StartLifetime;
			particlePool[idx].StartPosition = particlePool[ParticleCount - 1].StartPosition;
			particlePool[idx].Velocity = particlePool[ParticleCount - 1].Velocity;

			particlePool[idx].UpdateVBO();
			
			ParticleCount--;
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			// all drawing happens in PostDraw
		}

		public void PostDraw()
		{
			if (!IsInViewport) return;

			var g = Owner.GraphicsDevice;

			if (ParticleCount > 0)
			{
				if (vertexBuffer.IsContentLost)
				{
					vertexBuffer.SetData(vboArray.Data);
				}

				var oldBlendState = g.BlendState;
				g.BlendState = BlendState.AlphaBlend;

				parameterOffset.SetValue(Owner.MapOffset);
				parameterVirtualViewport.SetValue(Owner.VAdapter.GetShaderMatrix());
				parameterCurrentTime.SetValue(internalTime);

				g.SetVertexBuffer(vertexBuffer);
				g.Indices = indexBuffer;

				var oldRaster = g.RasterizerState;
				g.RasterizerState = new RasterizerState { CullMode = CullMode.None };

				foreach (EffectPass pass in particleEffect.CurrentTechnique.Passes)
				{
					pass.Apply();
					g.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, ParticleCount * 2);
				}

				g.RasterizerState = oldRaster;
				g.BlendState = oldBlendState;
			}
		}

		protected override void DrawDebugBorders(IBatchRenderer sbatch)
		{
			base.DrawDebugBorders(sbatch);

			sbatch.DrawRectangle(Position - new FSize(8,8) * 0.5f, new FSize(8, 8), Color.LightGreen, 1);

			for (int i = 0; i < ParticleCount; i++)
			{
				var p = particlePool[i];

				var position = p.StartPosition + p.Velocity * (internalTime - particlePool[i].StartLifetime);

				sbatch.DrawLine(p.StartPosition, position, Color.GreenYellow * 0.5f);
			}
		}
	}
}
