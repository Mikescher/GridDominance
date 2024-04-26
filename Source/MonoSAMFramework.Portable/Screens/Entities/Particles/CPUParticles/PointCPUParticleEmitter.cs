using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles.CPUParticles
{
	public class PointCPUParticleEmitter : CPUParticleEmitter
	{
		private FPoint _position;
		public override FPoint Position => _position;

		private FSize _boundingbox;
		public override FSize DrawingBoundingBox => _boundingbox;

		public PointCPUParticleEmitter(GameScreen scrn, FPoint pos, ParticleEmitterConfig cfg, int order) : base(scrn, cfg, order)
		{
			_position = pos;
		}

		public void SetPosition(FPoint pos)
		{
			_position = pos;
		}

		protected override FPoint SetParticleSpawnPosition(out bool doSpawn)
		{
			doSpawn = true;
			return Position;
		}

		protected override void RecalculateState()
		{
			base.RecalculateState();

			float maxDistance = Config.ParticleLifetimeMax * Config.ParticleVelocityMax;

			_boundingbox = new FSize(maxDistance * 2 + Config.ParticleSizeFinalMax, maxDistance * 2 + Config.ParticleSizeFinalMax);
		}

		protected override void DrawDebugBorders(IBatchRenderer sbatch)
		{
			base.DrawDebugBorders(sbatch);

			if (Config.ParticleSpawnAngleIsTotal)
				sbatch.DrawCircle(Position, DrawingBoundingBox.Width / 2, 32, Color.LightGreen, 1);
			else if (Config.ParticleSpawnAngleIsRandom)
				sbatch.DrawPiePiece(Position, DrawingBoundingBox.Width / 2, Config.ParticleSpawnAngleMin, Config.ParticleSpawnAngleMax, 32, Color.LightGreen, 1);
		}
	}
}
