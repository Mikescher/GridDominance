using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities.Operation;

namespace GridDominance.Shared.Screens.NormalGameScreen.EntityOperations
{
	class BulletDelayedDieOperation : GameEntityOperation<BaseBullet>
	{
		public const string NAME = "BulletDelayedDie";

		public BulletDelayedDieOperation(float time) : base(NAME, time)
		{
		}

		protected override void OnStart(BaseBullet entity)
		{
			entity.IsDying = true;
		}

		protected override void OnProgress(BaseBullet entity, float progress, SAMTime gameTime, InputState istate)
		{

		}

		protected override void OnEnd(BaseBullet entity)
		{
			entity.Alive = false;
		}

		protected override void OnAbort(BaseBullet entity)
		{
			//
		}
	}
}
