using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities.Operation;

namespace GridDominance.Shared.Screens.NormalGameScreen.EntityOperations
{
	class BulletFadeAndDieOperation : GameEntityOperation<BaseBullet>
	{
		public BulletFadeAndDieOperation(float time) : base("BulletFadeAndDieOperation", time)
		{
		}

		protected override void OnStart(BaseBullet entity)
		{
			entity.BulletAlpha = 1;
			entity.IsDying = true;
		}

		protected override void OnProgress(BaseBullet entity, float progress, SAMTime gameTime, InputState istate)
		{
			entity.BulletAlpha = 1 - progress;
		}

		protected override void OnEnd(BaseBullet entity)
		{
			entity.BulletAlpha = 0;
			entity.Alive = false;
		}

		protected override void OnAbort(BaseBullet entity)
		{
			//
		}
	}
}
