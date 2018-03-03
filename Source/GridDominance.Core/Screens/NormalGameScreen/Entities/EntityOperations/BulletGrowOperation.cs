using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.NormalGameScreen.EntityOperations
{
	class BulletGrowOperation : FixTimeOperation<Bullet>
	{
		public override string Name => "BulletGrowOperation";

		public BulletGrowOperation(float time) : base(time)
		{
		}

		protected override void OnStart(Bullet entity)
		{
			entity.BulletExtraScale = 0;
		}

		protected override void OnProgress(Bullet entity, float progress, SAMTime gameTime, InputState istate)
		{
			entity.BulletExtraScale = progress;
		}

		protected override void OnEnd(Bullet entity)
		{
			entity.BulletExtraScale = 1;
		}

		protected override void OnAbort(Bullet entity)
		{
			//
		}
	}
}
