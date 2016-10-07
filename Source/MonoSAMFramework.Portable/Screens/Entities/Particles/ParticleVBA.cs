namespace MonoSAMFramework.Portable.Screens.Entities.Particles
{
	public class ParticleVBA
	{
		public readonly ParticleVBO[] Data;

		public ParticleVBA(int particleCount)
		{
			Data = new ParticleVBO[particleCount * 4];
		}
	}
}
