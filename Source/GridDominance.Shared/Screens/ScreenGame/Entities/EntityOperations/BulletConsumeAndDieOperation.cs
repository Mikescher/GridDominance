using GridDominance.Shared.Screens.ScreenGame.Entities;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities.Operation;

namespace GridDominance.Shared.Screens.ScreenGame.EntityOperations
{
	class BulletConsumeAndDieOperation : GameEntityOperation<Bullet>
	{
		public BulletConsumeAndDieOperation() : base("BulletConsumeAndDieOperation", 0.15f)
		{
		}

		protected override void OnStart(Bullet entity)
		{
			entity.BulletExtraScale = 1;
			entity.IsDying = true;
		}

		protected override void OnProgress(Bullet entity, float progress, SAMTime gameTime, InputState istate)
		{
			entity.BulletExtraScale = 1 - progress;
		}

		protected override void OnEnd(Bullet entity)
		{
			entity.BulletExtraScale = 0;
			entity.Alive = false;
		}

		protected override void OnAbort(Bullet entity)
		{
			//
		}
	}
}
