using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath.VectorPath;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles.GPUParticles
{
	public class AnimatedPathGPUParticleEmitter : PathGPUParticleEmitter, IParticleOwner
	{
		private readonly float animationLength;
		private readonly float animationInitialDelay;

		public AnimatedPathGPUParticleEmitter(GameScreen scrn, Vector2 pos, VectorPath path, ParticleEmitterConfig cfg, float delay, float length, int order) : base(scrn, pos, path, cfg, order)
		{
			animationLength = length;
			animationInitialDelay = delay;
		}

		protected override void InitializeParticle(GPUParticle p, int index, int count)
		{
			base.InitializeParticle(p, index, count);

			p.StartTimeOffset = animationInitialDelay + (index * animationLength) / count;
		}

		public void ResetAnimation()
		{
			initializeTime = MonoSAMGame.CurrentTime.TotalElapsedSeconds;
		}
	}
}
