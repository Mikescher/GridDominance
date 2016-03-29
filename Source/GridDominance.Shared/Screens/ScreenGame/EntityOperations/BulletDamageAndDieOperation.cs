using GridDominance.Shared.Screens.ScreenGame.Entities;
using GridDominance.Shared.Screens.GameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.GameScreen.EntityOperations
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
