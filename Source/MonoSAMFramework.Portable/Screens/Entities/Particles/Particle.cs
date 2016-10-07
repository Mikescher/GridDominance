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

		private readonly ParticleVBA _vba;
		private readonly int _vbaIndex;

		public Vector2 StartPosition;

		public Vector2 Velocity;
		
		public float StartLifetime;
		public float MaxLifetime;

		public float SizeInitial;
		public float SizeFinal;

		public Particle(ParticleVBA vba, int index)
		{
			_vba = vba;
			_vbaIndex = index;
		}

		public void Init(GameTime gameTime)
		{
			StartLifetime = gameTime.GetTotalElapsedSeconds();

			UpdateVBO();
		}

		public void UpdateVBO()
		{
			_vba.Data[_vbaIndex * 4 + 0].Corner = CORNER_TL;
			_vba.Data[_vbaIndex * 4 + 0].StartPosition = StartPosition;
			_vba.Data[_vbaIndex * 4 + 0].Velocity = Velocity;
			_vba.Data[_vbaIndex * 4 + 0].StartTime = StartLifetime;
			_vba.Data[_vbaIndex * 4 + 0].LifeTime = MaxLifetime;
			_vba.Data[_vbaIndex * 4 + 0].StartSize = SizeInitial;
			_vba.Data[_vbaIndex * 4 + 0].FinalSize = SizeFinal;

			_vba.Data[_vbaIndex * 4 + 1].Corner = CORNER_TR;
			_vba.Data[_vbaIndex * 4 + 1].StartPosition = StartPosition;
			_vba.Data[_vbaIndex * 4 + 1].Velocity = Velocity;
			_vba.Data[_vbaIndex * 4 + 1].StartTime = StartLifetime;
			_vba.Data[_vbaIndex * 4 + 1].LifeTime = MaxLifetime;
			_vba.Data[_vbaIndex * 4 + 1].StartSize = SizeInitial;
			_vba.Data[_vbaIndex * 4 + 1].FinalSize = SizeFinal;

			_vba.Data[_vbaIndex * 4 + 2].Corner = CORNER_BR;
			_vba.Data[_vbaIndex * 4 + 2].StartPosition = StartPosition;
			_vba.Data[_vbaIndex * 4 + 2].Velocity = Velocity;
			_vba.Data[_vbaIndex * 4 + 2].StartTime = StartLifetime;
			_vba.Data[_vbaIndex * 4 + 2].LifeTime = MaxLifetime;
			_vba.Data[_vbaIndex * 4 + 2].StartSize = SizeInitial;
			_vba.Data[_vbaIndex * 4 + 2].FinalSize = SizeFinal;

			_vba.Data[_vbaIndex * 4 + 3].Corner = CORNER_BL;
			_vba.Data[_vbaIndex * 4 + 3].StartPosition = StartPosition;
			_vba.Data[_vbaIndex * 4 + 3].Velocity = Velocity;
			_vba.Data[_vbaIndex * 4 + 3].StartTime = StartLifetime;
			_vba.Data[_vbaIndex * 4 + 3].LifeTime = MaxLifetime;
			_vba.Data[_vbaIndex * 4 + 3].StartSize = SizeInitial;
			_vba.Data[_vbaIndex * 4 + 3].FinalSize = SizeFinal;
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
