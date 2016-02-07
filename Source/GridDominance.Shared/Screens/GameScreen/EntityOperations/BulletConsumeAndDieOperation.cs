using GridDominance.Shared.Framework;
using GridDominance.Shared.Screens.GameScreen.Entities;

namespace GridDominance.Shared.Screens.GameScreen.EntityOperations
{
	class BulletConsumeAndDieOperation : GDEntityOperation<Bullet>
	{
		public BulletConsumeAndDieOperation() : base(0.15f)
		{
		}

		protected override void OnStart(Bullet entity)
		{
			entity.SpriteBullet.Alpha = 1;
			entity.IsDying = true;
		}

		protected override void OnProgress(Bullet entity, float progress, InputState istate)
		{
			entity.SpriteBullet.Alpha = 1 - progress;
		}

		protected override void OnEnd(Bullet entity)
		{
			entity.SpriteBullet.Alpha = 0;
			entity.Alive = false;

		}
	}
}
