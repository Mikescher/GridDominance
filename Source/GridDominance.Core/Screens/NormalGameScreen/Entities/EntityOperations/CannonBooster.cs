using GridDominance.Shared.Screens.NormalGameScreen.Entities.Cannons;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities.EntityOperations
{
	class CannonBooster : FixTimeOperation<Cannon>
	{
		public override string Name => "CannonBooster";

		public CannonBooster(float lifetime) : base(lifetime)
		{
			//
		}

		protected override void OnStart(Cannon entity)
		{
			entity.BulletBoostCount++;
		}

		protected override void OnProgress(Cannon entity, float progress, SAMTime gameTime, InputState istate) { }

		protected override void OnEnd(Cannon entity)
		{
			entity.BulletBoostCount--;
		}

		protected override void OnAbort(Cannon entity)
		{
			entity.BulletBoostCount--;
		}
	}
}
