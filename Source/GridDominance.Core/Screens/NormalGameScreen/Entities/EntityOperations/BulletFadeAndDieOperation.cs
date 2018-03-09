using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities.EntityOperations
{
	class BulletFadeAndDieOperation : FixTimeOperation<BaseBullet>
	{
		public const string NAME = "BulletFadeAndDie";
		public override string Name => "BulletFadeAndDie";

		private readonly bool _realKill;

		public BulletFadeAndDieOperation(float time, bool kill = true) : base(time)
		{
			_realKill = kill;
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
			entity.BulletExtraScale = 0;
			entity.BulletAlpha = 0;
			if (_realKill) entity.Alive = false;
		}

		protected override void OnAbort(BaseBullet entity)
		{
			//
		}
	}
}
