using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.VectorPath;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles.GPUParticles
{
	public class PathGPUParticleEmitter : GPUParticleEmitter
	{
		private readonly VectorPath vectorPath;
		private readonly FPoint vectorPathCenter;

		public override FPoint Position { get; }

		private FSize _boundingbox;
		public override FSize DrawingBoundingBox => _boundingbox;

		public PathGPUParticleEmitter(GameScreen scrn, FPoint pos, VectorPath path, ParticleEmitterConfig cfg, int order) : base(scrn, cfg, order)
		{
			vectorPath = path;
			vectorPathCenter = path.Boundings.Center;

			Position = pos;
		}

		protected override void InitializeParticle(GPUParticle p, int index, int count)
		{
			var pos = vectorPath.Get(index * vectorPath.Length / count);

			p.StartPosition = pos.RelativeTo(vectorPathCenter);
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
