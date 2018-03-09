using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities.EntityOperations
{
	class BulletDelayedDieOperation : FixTimeOperation<BaseBullet>
	{
		public override string Name => "BulletDelayedDie";

		public BulletDelayedDieOperation(float time) : base(time)
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
