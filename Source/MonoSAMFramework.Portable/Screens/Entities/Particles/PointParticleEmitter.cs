using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles
{
	public class PointParticleEmitter : ParticleEmitter
	{
		private Vector2 _position;
		public override Vector2 Position => _position;

		private FSize _boundingbox;
		public override FSize DrawingBoundingBox => _boundingbox;

		public PointParticleEmitter(GameScreen scrn, Vector2 pos, ParticleEmitterConfig cfg) : base(scrn, cfg)
		{
			_position = pos;
		}

		public void SetPosition(Vector2 pos)
		{
			_position = pos;
		}

		protected override void SetParticleSpawnPosition(ref Vector2 vec)
		{
			vec.X = Position.X;
			vec.Y = Position.Y;
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
				sbatch.DrawCirclePiece(Position, DrawingBoundingBox.Width / 2, Config.ParticleSpawnAngleMin, Config.ParticleSpawnAngleMax, 32, Color.LightGreen, 1);
		}
	}
}
