using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.VectorPath;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles.CPUParticles
{
	public class PathCPUParticleEmitter : CPUParticleEmitter
	{
		private readonly VectorPath vectorPath;
		private readonly Vector2 vectorPathCenter;

		public override Vector2 Position { get; }

		private FSize _boundingbox;
		public override FSize DrawingBoundingBox => _boundingbox;

		public PathCPUParticleEmitter(GameScreen scrn, Vector2 pos, VectorPath path, ParticleEmitterConfig cfg) : base(scrn, cfg)
		{
			vectorPath = path;
			vectorPathCenter = path.Boundings.Center;

			Position = pos;
		}

		protected override void SetParticleSpawnPosition(ref Vector2 vec, out bool doSpawn)
		{
			var len = FloatMath.GetRangedRandom(0, vectorPath.Length);
			var pos = vectorPath.Get(len);

			vec.X = Position.X + (pos.X - vectorPathCenter.X);
			vec.Y = Position.Y + (pos.Y - vectorPathCenter.Y);

			doSpawn = true;
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
			sbatch.DrawRectangle(vectorPath.Boundings.AsOffseted(Position - vectorPathCenter), Color.LightBlue, 1);

			sbatch.DrawPath(Position - vectorPathCenter, vectorPath, 48, Color.LightGreen, 1);

		}
	}
}
