using Microsoft.Xna.Framework;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles
{
	public struct Particle
	{
		public Vector2 StartPosition;

		public Vector2 Position;
		public Vector2 Velocity;
		
		public float CurrentLifetime;
		public float MaxLifetime;

		public float SizeInitial;
		public float SizeFinal;
	}
}
