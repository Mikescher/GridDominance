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
		private readonly Vector2 vectorPathCenter;

		public override Vector2 Position { get; }

		private FSize _boundingbox;
		public override FSize DrawingBoundingBox => _boundingbox;

		private readonly float animationLength;
		private readonly float animationInitialDelay;

		public AnimatedPathCPUParticleEmitter(GameScreen scrn, Vector2 pos, VectorPath path, ParticleEmitterConfig cfg, float delay, float length) : base(scrn, cfg)
		{
			vectorPath = path;
			vectorPathCenter = path.Boundings.Center;

			Position = pos;

			animationLength = length;
			animationInitialDelay = delay;
		}

		protected override void SetParticleSpawnPosition(ref Vector2 vec, out bool doSpawn)
		{
			var len = FloatMath.GetRangedRandom(0, vectorPath.Length);

			float mintime = animationInitialDelay + animationLength * (len / vectorPath.Length);
			
			var pos = vectorPath.Get(len);

			vec.X = Position.X + (pos.X - vectorPathCenter.X);
			vec.Y = Position.Y + (pos.Y - vectorPathCenter.Y);

			doSpawn = (mintime < internalTime);
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

			sbatch.DrawRectangle(Position - new FSize(8, 8) * 0.5f, new FSize(8, 8), Color.LightGreen, 1);
			sbatch.DrawRectangle(vectorPath.Boundings.AsTranslated(Position - vectorPathCenter), Color.LightBlue, 1);

			sbatch.DrawPath(Position - vectorPathCenter, vectorPath, 48, Color.LightGreen, 1);

		}
	}
}
