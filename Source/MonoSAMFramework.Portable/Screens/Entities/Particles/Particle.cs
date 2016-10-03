using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoSAMFramework.Portable.GameMath;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles
{
	public class Particle
	{
		public readonly ParticleVBO[] VertexBuffer = new ParticleVBO[4];

		public Vector2 StartPosition;

		public Vector2 Position;

		public Vector2 Velocity;
		
		public float CurrentLifetime;
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

		public void Init()
		{
			VertexBuffer[0].Position.X = Position.X - (SizeInitial / 2);
			VertexBuffer[0].Position.Y = Position.Y + (SizeInitial / 2);

			VertexBuffer[1].Position.X = Position.X + (SizeInitial / 2);
			VertexBuffer[1].Position.Y = Position.Y + (SizeInitial / 2);

			VertexBuffer[2].Position.X = Position.X + (SizeInitial / 2);
			VertexBuffer[2].Position.Y = Position.Y - (SizeInitial / 2);

			VertexBuffer[3].Position.X = Position.X - (SizeInitial / 2);
			VertexBuffer[3].Position.Y = Position.Y - (SizeInitial / 2);
		}

		public bool Update(GameTime gameTime)
		{
			CurrentLifetime += gameTime.GetElapsedSeconds();
			if (CurrentLifetime >= MaxLifetime) return false;

			var progress = CurrentLifetime / MaxLifetime;

			Position.X += Velocity.X * gameTime.GetElapsedSeconds();
			Position.Y += Velocity.Y * gameTime.GetElapsedSeconds();

			var size = FloatMath.Lerp(SizeInitial, SizeFinal, progress);

			VertexBuffer[0].Position.X = Position.X - (size / 2);
			VertexBuffer[0].Position.Y = Position.Y + (size / 2);

			VertexBuffer[1].Position.X = Position.X + (size / 2);
			VertexBuffer[1].Position.Y = Position.Y + (size / 2);

			VertexBuffer[2].Position.X = Position.X + (size / 2);
			VertexBuffer[2].Position.Y = Position.Y - (size / 2);

			VertexBuffer[3].Position.X = Position.X - (size / 2);
			VertexBuffer[3].Position.Y = Position.Y - (size / 2);


			return true;
		}
	}

	public struct ParticleVBO
	{
		public Vector3 Position;

		public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
		(
			new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0)
		);
	}
}
