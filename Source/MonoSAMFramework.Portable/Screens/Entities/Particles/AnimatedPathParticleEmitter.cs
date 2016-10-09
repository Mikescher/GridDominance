using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.VectorPath;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles
{
	public class AnimatedPathParticleEmitter : PathParticleEmitter
	{
		private readonly float animationLength;
		private readonly float animationInitialDelay;

		public AnimatedPathParticleEmitter(GameScreen scrn, Vector2 pos, VectorPath path, ParticleEmitterConfig cfg, float delay, float length) : base(scrn, pos, path, cfg)
		{
			animationLength = length;
			animationInitialDelay = delay;
		}

		protected override void InitializeParticle(Particle p, int index, int count)
		{
			base.InitializeParticle(p, index, count);

			p.StartTimeOffset = animationInitialDelay + (index * animationLength) / count;
		}

		public void ResetAnimation()
		{
			initializeTime = MonoSAMGame.CurrentTime.GetTotalElapsedSeconds();
		}
	}
}
