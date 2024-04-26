namespace MonoSAMFramework.Portable.Screens.Entities.Particles.GPUParticles
{
	public class GPUParticleVBA
	{
		public readonly GPUParticleVBO[] Data;

		public GPUParticleVBA(int particleCount)
		{
			Data = new GPUParticleVBO[particleCount * 4];
		}
	}
}
