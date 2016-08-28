using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.MathHelper.FloatClasses;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles
{
	public class PointParticleEmitter : ParticleEmitter
	{
		public override Vector2 Position { get; }

		private FSize _boundingbox;
		public override FSize DrawingBoundingBox => _boundingbox;

		public PointParticleEmitter(GameScreen scrn, Vector2 pos, ParticleEmitterConfig cfg) : base(scrn, cfg)
		{
			Position = pos;
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
	}
}
