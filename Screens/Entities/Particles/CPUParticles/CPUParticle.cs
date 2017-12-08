using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles.CPUParticles
{
	public sealed class CPUParticle
	{
		public FPoint StartPosition;

		public FPoint Position;
		public Vector2 Velocity;

		public float CurrentLifetime;
		public float MaxLifetime;

		public float SizeInitial;
		public float SizeFinal;
	}
}
