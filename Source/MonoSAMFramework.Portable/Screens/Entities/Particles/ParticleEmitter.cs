using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using System.Linq;

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


			spawnDelay = _config.GetSpawnDelay();
			timeSinceLastSpawn = 0f;
			ParticleCount = 0;

			//-------

			particlePool = new Particle[maxParticleCount];
			for (int i = 0; i < maxParticleCount; i++) particlePool[i] = new Particle();

			vertexBuffer = new DynamicVertexBuffer(Owner.GraphicsDevice, ParticleVBO.VertexDeclaration, maxParticleCount*4, BufferUsage.WriteOnly);
			vertexBuffer.SetData(particlePool.SelectMany(p => p.VertexBuffer).ToArray());

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

			particleEffect = Owner.Game.Content.Load<Effect>("shaders/SAMParticleEffect");
			//particleEffect.Parameters["TextureSampler"].SetValue(_config.Texture.Texture);
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

			for (int i = ParticleCount-1; i >= 0; i--)
			{
				if (!particlePool[i].Update(gameTime))
				{
					RemoveParticle(i);
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

			vertexBuffer.SetData(particlePool.SelectMany(p => p.VertexBuffer).ToArray()); //TODO not cool
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

			particlePool[ParticleCount].Init();

			//vertexBuffer.SetData(particlePool[ParticleCount].VertexBuffer, ParticleCount * 4, 4);

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

			//--------------------------------------------------

			
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

		public void PostDraw()
		{
			var g = Owner.GraphicsDevice;

			if (ParticleCount > 0)
			{
				if (vertexBuffer.IsContentLost)
				{
					vertexBuffer.SetData(particlePool.SelectMany(p => p.VertexBuffer).ToArray());
				}

				//particleEffect.Parameters["World"].SetValue(Matrix.CreateTranslation(0, 0, 0));
				//particleEffect.Parameters["View"].SetValue(Matrix.CreateLookAt(new Vector3(0, 0, -10), Vector3.Zero, Vector3.UnitX));
				//particleEffect.Parameters["Projection"].SetValue(Matrix.CreateOrthographic(1024, 640, -100f, 100f));

				particleEffect.Parameters["Offset"].SetValue(Matrix.CreateTranslation(Owner.MapOffsetX, Owner.MapOffsetY, 0));
				particleEffect.Parameters["VirtualViewport"].SetValue(Matrix.CreateOrthographicOffCenter(Owner.VAdapter.VirtualTotalBoundingBoxLeft, Owner.VAdapter.VirtualTotalBoundingBoxRight, Owner.VAdapter.VirtualTotalBoundingBoxBottom, Owner.VAdapter.VirtualTotalBoundingBoxTop, -100, 100));
				//particleEffect.Parameters["ScaleMatrix"].SetValue(Owner.VAdapter.GetScaleMatrix());


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
