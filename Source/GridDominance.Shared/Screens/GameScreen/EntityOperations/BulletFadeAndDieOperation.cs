using GridDominance.Shared.Screens.GameScreen.Entities;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.GameScreen.EntityOperations
{
	class BulletFadeAndDieOperation : GDEntityOperation<Bullet>
	{
		public BulletFadeAndDieOperation() : base(1.0f)
		{
		}

		protected override void OnStart(Bullet entity)
		{
			entity.BulletAlpha = 1;
			entity.IsDying = true;
		}

		protected override void OnProgress(Bullet entity, float progress, InputState istate)
		{
			entity.BulletAlpha = 1 - progress;
		}

		protected override void OnEnd(Bullet entity)
		{
			entity.BulletAlpha = 0;
			entity.Alive = false;

		}
	}
}
