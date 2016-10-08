using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
		public float StartTimeOffset;
		public Vector4 Random;

		public Particle(ParticleVBA vba, int index)
		{
			_vba = vba;
			_vbaIndex = index;
		}
		
		public void UpdateVBO()
		{
			_vba.Data[_vbaIndex * 4 + 0].Corner = CORNER_TL;
			_vba.Data[_vbaIndex * 4 + 0].StartPosition = StartPosition;
			_vba.Data[_vbaIndex * 4 + 0].StartTimeOffset = StartTimeOffset;
			_vba.Data[_vbaIndex * 4 + 0].Random = Random;

			_vba.Data[_vbaIndex * 4 + 1].Corner = CORNER_TR;
			_vba.Data[_vbaIndex * 4 + 1].StartPosition = StartPosition;
			_vba.Data[_vbaIndex * 4 + 1].StartTimeOffset = StartTimeOffset;
			_vba.Data[_vbaIndex * 4 + 1].Random = Random;

			_vba.Data[_vbaIndex * 4 + 2].Corner = CORNER_BR;
			_vba.Data[_vbaIndex * 4 + 2].StartPosition = StartPosition;
			_vba.Data[_vbaIndex * 4 + 2].StartTimeOffset = StartTimeOffset;
			_vba.Data[_vbaIndex * 4 + 2].Random = Random;

			_vba.Data[_vbaIndex * 4 + 3].Corner = CORNER_BL;
			_vba.Data[_vbaIndex * 4 + 3].StartPosition = StartPosition;
			_vba.Data[_vbaIndex * 4 + 3].StartTimeOffset = StartTimeOffset;
			_vba.Data[_vbaIndex * 4 + 3].Random = Random;
		}

		public bool IsCurrentlyRendered(float time, ParticleEmitterConfig cfg)
		{
			return false;
			//if (cfg == null) return false;
			//
			//int iteration = (int)((time + StartTimeOffset) / cfg.ParticleLifetimeMax);
			//float age = (time + StartTimeOffset) % cfg.ParticleLifetimeMax;
			//
			//var rand = new System.Random(Random.GetHashCode() * iteration); // not correct - shader uses different rand method
			//
			//float maxLifetime = FloatMath.Lerp(cfg.ParticleLifetimeMin, cfg.ParticleLifetimeMax, rand.Next());
			//
			//return (age < maxLifetime);
		}
	}

	public struct ParticleVBO
	{
		public Vector2 Corner;
		public Vector2 StartPosition;
		public float StartTimeOffset;
		public Vector4 Random;

		public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
		(
			new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
			new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.Position, 1),
			new VertexElement(16, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 0),
			new VertexElement(20, VertexElementFormat.Vector4, VertexElementUsage.Position, 2)
		);
	}
}
