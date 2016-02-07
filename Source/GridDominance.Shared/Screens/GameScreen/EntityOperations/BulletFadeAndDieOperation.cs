using GridDominance.Shared.Framework;
using GridDominance.Shared.Screens.GameScreen.Entities;

namespace GridDominance.Shared.Screens.GameScreen.EntityOperations
{
	class BulletFadeAndDieOperation : GDEntityOperation<Bullet>
	{
		public BulletFadeAndDieOperation() : base(1.0f)
		{
		}

		protected override void OnStart(Bullet entity, InputState istate)
		{
			entity.SpriteBullet.Alpha = 1;
		}

		protected override void OnProgress(Bullet entity, float progress, InputState istate)
		{
			entity.SpriteBullet.Alpha = 1 - progress;
		}

		protected override void OnEnd(Bullet entity, InputState istate)
		{
			entity.SpriteBullet.Alpha = 0;
			entity.Alive = false;

		}
	}
}
