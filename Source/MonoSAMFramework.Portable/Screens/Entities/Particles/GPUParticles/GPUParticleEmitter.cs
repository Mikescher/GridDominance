using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles.GPUParticles
{
	/// <summary>
	/// https://github.com/CartBlanche/MonoGame-Samples/blob/master/Particle3DSample
	/// </summary>
	public abstract class GPUParticleEmitter : GameEntity, ISAMPostDrawable
	{
		public override Color DebugIdentColor => Color.Gold * 0.1f;

		protected float initializeTime;

		private ParticleEmitterConfig _config;
		public ParticleEmitterConfig Config { get { return _config; } set { _config = value; RecalculateState(); } }

		private Effect particleEffect;
		private EffectParameter parameterOffset;
		private EffectParameter parameterVirtualViewport;
		private EffectParameter parameterCurrentTime;

		private GPUParticle[] particlePool;
		private GPUParticleVBA vboArray;

		private VertexBuffer vertexBuffer;
		private IndexBuffer indexBuffer;
		private int geometryCount;

		public int ParticleCount => geometryCount;

		protected GPUParticleEmitter(GameScreen scrn, ParticleEmitterConfig cfg) : base(scrn)
		{
			_config = cfg;
		}

		protected virtual void RecalculateState()
		{
			initializeTime = MonoSAMGame.CurrentTime.TotalElapsedSeconds;

			LoadShader();
		}

		private void LoadShader()
		{
			UnloadShader();

			geometryCount = FloatMath.Ceiling(_config.SpawnRate * _config.ParticleLifetimeMax);

			// Create Particle Pool

			particlePool = new GPUParticle[geometryCount];
			vboArray = new GPUParticleVBA(geometryCount);
			for (int i = 0; i < geometryCount; i++)
			{
				particlePool[i] = new GPUParticle(vboArray, i)
				{
					StartPosition = Position,
					Random = new Vector4(FloatMath.GetRandom(), FloatMath.GetRandom(), FloatMath.GetRandom(), FloatMath.GetRandom()),
					StartTimeOffset = Config.SpawnDelay * i
				};
				
				InitializeParticle(particlePool[i], i, geometryCount);

				particlePool[i].UpdateVBO();
			}
			
			// Create VertexBuffer and IndexBuffer

			vertexBuffer = new VertexBuffer(Owner.GraphicsDevice, GPUParticleVBO.VertexDeclaration, geometryCount * 4, BufferUsage.WriteOnly);
			vertexBuffer.SetData(vboArray.Data);

			if (geometryCount * 6 < short.MaxValue)
			{
				short[] indices = new short[geometryCount * 6];
				for (short i = 0; i < geometryCount; i++)
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
				indexBuffer = new IndexBuffer(Owner.GraphicsDevice, typeof(short), geometryCount * 6, BufferUsage.WriteOnly);
				indexBuffer.SetData(indices);
			}
			else
			{
				int[] indices = new int[geometryCount * 6];
				for (int i = 0; i < geometryCount; i++)
				{
					// TR triangle
					indices[i * 6 + 0] = (i * 4 + 0);
					indices[i * 6 + 1] = (i * 4 + 2);
					indices[i * 6 + 2] = (i * 4 + 1);

					// BL triangle
					indices[i * 6 + 3] = (i * 4 + 0);
					indices[i * 6 + 4] = (i * 4 + 3);
					indices[i * 6 + 5] = (i * 4 + 2);
				}
				indexBuffer = new IndexBuffer(Owner.GraphicsDevice, typeof(int), geometryCount * 6, BufferUsage.WriteOnly);
				indexBuffer.SetData(indices);
			}


			// Load effect

			particleEffect = Owner.Game.Content.Load<Effect>("shaders/SAMParticleEffect").Clone();

			// World Config

			parameterOffset = particleEffect.Parameters["Offset"]; 
			parameterVirtualViewport = particleEffect.Parameters["VirtualViewport"];
			parameterCurrentTime = particleEffect.Parameters["CurrentTime"];

			// Particle Config

			particleEffect.Parameters["Texture"]?.SetValue(Config.Texture.Texture);
			particleEffect.Parameters["TextureProjection"]?.SetValue(Config.Texture.GetShaderProjectionMatrix().ToMatrix());

			particleEffect.Parameters["ParticleLifetimeMin"]?.SetValue(Config.ParticleLifetimeMin);
			particleEffect.Parameters["ParticleLifetimeMax"]?.SetValue(Config.ParticleLifetimeMax);
			particleEffect.Parameters["ParticleRespawnTime"]?.SetValue(Config.ParticleRespawnTime);

			particleEffect.Parameters["ParticleSpawnAngleMin"]?.SetValue(Config.ParticleSpawnAngleMin);
			particleEffect.Parameters["ParticleSpawnAngleMax"]?.SetValue(Config.ParticleSpawnAngleMax);
			particleEffect.Parameters["ParticleSpawnAngleIsRandom"]?.SetValue(Config.ParticleSpawnAngleIsRandom);
			particleEffect.Parameters["FixedParticleSpawnAngle"]?.SetValue(Config.FixedParticleSpawnAngle);

			particleEffect.Parameters["ParticleVelocityMin"]?.SetValue(Config.ParticleVelocityMin);
			particleEffect.Parameters["ParticleVelocityMax"]?.SetValue(Config.ParticleVelocityMax);

			particleEffect.Parameters["ParticleAlphaInitial"]?.SetValue(Config.ParticleAlphaInitial);
			particleEffect.Parameters["ParticleAlphaFinal"]?.SetValue(Config.ParticleAlphaFinal);

			particleEffect.Parameters["ParticleSizeInitialMin"]?.SetValue(Config.ParticleSizeInitialMin);
			particleEffect.Parameters["ParticleSizeInitialMax"]?.SetValue(Config.ParticleSizeInitialMax);

			particleEffect.Parameters["ParticleSizeFinalMin"]?.SetValue(Config.ParticleSizeFinalMin);
			particleEffect.Parameters["ParticleSizeFinalMax"]?.SetValue(Config.ParticleSizeFinalMax);

			particleEffect.Parameters["ColorInitial"]?.SetValue(Config.ColorInitial.ToVector4(Config.ParticleAlphaInitial));
			particleEffect.Parameters["ColorFinal"]?.SetValue(Config.ColorFinal.ToVector4(Config.ParticleAlphaInitial));
		}

		private void UnloadShader()
		{
			vertexBuffer?.Dispose();
			indexBuffer?.Dispose();
			particlePool = null;
			vboArray = null;
		}

		public override void OnInitialize(EntityManager manager)
		{
			RecalculateState();

			manager.RegisterPostDraw(this);
		}

		public override void OnRemove()
		{
			UnloadShader();
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{

		}
		
		protected abstract void InitializeParticle(GPUParticle p, int index, int count);
		
		protected override void OnDraw(IBatchRenderer sbatch)
		{
			// all drawing happens in PostDraw
		}

		public void PostDraw()
		{
			if (!IsInViewport) return;

			var g = Owner.GraphicsDevice;

			var oldBlendState = g.BlendState;
			g.BlendState = BlendState.AlphaBlend;

			parameterOffset.SetValue(Owner.MapOffset + Position);
			parameterVirtualViewport.SetValue(Owner.VAdapterGame.GetShaderMatrix());
			parameterCurrentTime.SetValue(MonoSAMGame.CurrentTime.TotalElapsedSeconds - initializeTime);
//			parameterCurrentTime.SetValue(100f);

			g.SetVertexBuffer(vertexBuffer);
			g.Indices = indexBuffer;

			// Rasterizer takes a shitton of time
			//var oldRaster = g.RasterizerState;
			//g.RasterizerState = new RasterizerState { CullMode = CullMode.None };

			foreach (EffectPass pass in particleEffect.CurrentTechnique.Passes)
			{
				pass.Apply();
				g.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, geometryCount * 2);
			}

			//g.RasterizerState = oldRaster;
			g.BlendState = oldBlendState;
		}

		protected override void DrawDebugBorders(IBatchRenderer sbatch)
		{
			base.DrawDebugBorders(sbatch);

			sbatch.DrawRectangle(Position - new FSize(8,8) * 0.5f, new FSize(8, 8), Color.LightGreen, 1);
		}
	}
}
