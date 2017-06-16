using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.VectorPath;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles.CPUParticles
{
	public class AnimatedPathCPUParticleEmitter : CPUParticleEmitter
	{
		private readonly VectorPath vectorPath;
		private readonly FPoint vectorPathCenter;

		public override FPoint Position { get; }

		private FSize _boundingbox;
		public override FSize DrawingBoundingBox => _boundingbox;

		private readonly float animationLength;
		private readonly float animationInitialDelay;

		public AnimatedPathCPUParticleEmitter(GameScreen scrn, FPoint pos, VectorPath path, ParticleEmitterConfig cfg, float delay, float length, int order) : base(scrn, cfg, order)
		{
			vectorPath = path;
			vectorPathCenter = path.Boundings.Center;

			Position = pos;

			animationLength = length;
			animationInitialDelay = delay;
		}

		protected override FPoint SetParticleSpawnPosition(out bool doSpawn)
		{
			var len = FloatMath.GetRangedRandom(0, vectorPath.Length);

			float mintime = animationInitialDelay + animationLength * (len / vectorPath.Length);
			
			var pos = vectorPath.Get(len);

			doSpawn = (mintime < internalTime);
			return Position + (pos - vectorPathCenter);
		}

		protected override void RecalculateState()
		{
			base.RecalculateState();

			float maxDistance = Config.ParticleLifetimeMax * Config.ParticleVelocityMax;
			_boundingbox = vectorPath.Boundings.AsInflated(maxDistance + Config.ParticleSizeFinalMax, maxDistance + Config.ParticleSizeFinalMax).Size;
		}

		protected override void DrawDebugBorders(IBatchRenderer sbatch)
		{
			base.DrawDebugBorders(sbatch);

			sbatch.DrawRectangle(Position - new Vector2(8, 8) * 0.5f, new FSize(8, 8), Color.LightGreen, 1);
			sbatch.DrawRectangle(vectorPath.Boundings.AsTranslated(Position - vectorPathCenter), Color.LightBlue, 1);

			sbatch.DrawPath(Position.RelativeTo(vectorPathCenter), vectorPath, 48, Color.LightGreen, 1);

		}
	}
}
