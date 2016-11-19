using GridDominance.Shared.Screens.ScreenGame.Entities;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.Entities.Operation;

namespace GridDominance.Shared.Screens.ScreenGame.EntityOperations
{
	class BulletDamageAndDieOperation : GameEntityOperation<Bullet>
	{
		public BulletDamageAndDieOperation() : base(0.05f)
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
