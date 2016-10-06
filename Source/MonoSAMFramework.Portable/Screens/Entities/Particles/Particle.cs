using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.Extensions;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles
{
	public class Particle
	{
		public static readonly Vector2 CORNER_TL = new Vector2(-1, +1);
		public static readonly Vector2 CORNER_TR = new Vector2(+1, +1);
		public static readonly Vector2 CORNER_BR = new Vector2(+1, -1);
		public static readonly Vector2 CORNER_BL = new Vector2(-1, -1);

		public readonly ParticleVBO[] VertexBuffer = new ParticleVBO[4];

		public Vector2 StartPosition;

		public Vector2 Velocity;
		
		public float StartLifetime;
		public float MaxLifetime;

		public float SizeInitial;
		public float SizeFinal;

		public Particle()
		{
			VertexBuffer[0] = new ParticleVBO(); // TL
			VertexBuffer[1] = new ParticleVBO(); // TR
			VertexBuffer[2] = new ParticleVBO(); // BR
			VertexBuffer[3] = new ParticleVBO(); // BL
		}

		public void Init(GameTime gameTime)
		{
			StartLifetime = gameTime.GetTotalElapsedSeconds();

			VertexBuffer[0].Corner = CORNER_TL;
			VertexBuffer[0].StartPosition = StartPosition;
			VertexBuffer[0].Velocity = Velocity;
			VertexBuffer[0].StartTime = StartLifetime;
			VertexBuffer[0].LifeTime = MaxLifetime;
			VertexBuffer[0].StartSize = SizeInitial;
			VertexBuffer[0].FinalSize = SizeFinal;

			VertexBuffer[1].Corner = CORNER_TR;
			VertexBuffer[1].StartPosition = StartPosition;
			VertexBuffer[1].Velocity = Velocity;
			VertexBuffer[1].StartTime = StartLifetime;
			VertexBuffer[1].LifeTime = MaxLifetime;
			VertexBuffer[1].StartSize = SizeInitial;
			VertexBuffer[1].FinalSize = SizeFinal;

			VertexBuffer[2].Corner = CORNER_BR;
			VertexBuffer[2].StartPosition = StartPosition;
			VertexBuffer[2].Velocity = Velocity;
			VertexBuffer[2].StartTime = StartLifetime;
			VertexBuffer[2].LifeTime = MaxLifetime;
			VertexBuffer[2].StartSize = SizeInitial;
			VertexBuffer[2].FinalSize = SizeFinal;

			VertexBuffer[3].Corner = CORNER_BL;
			VertexBuffer[3].StartPosition = StartPosition;
			VertexBuffer[3].Velocity = Velocity;
			VertexBuffer[3].StartTime = StartLifetime;
			VertexBuffer[3].LifeTime = MaxLifetime;
			VertexBuffer[3].StartSize = SizeInitial;
			VertexBuffer[3].FinalSize = SizeFinal;
		}
	}

	public struct ParticleVBO
	{
		public Vector2 Corner;

		public Vector2 StartPosition;
		public Vector2 Velocity;

		public float StartTime;
		public float LifeTime;

		public float StartSize;
		public float FinalSize;

		public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
		(
			new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
			new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.Position, 1),
			new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.Normal, 0),
			new VertexElement(24, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 0),
			new VertexElement(28, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1),
			new VertexElement(32, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 2),
			new VertexElement(36, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 3)
		);
	}
}
