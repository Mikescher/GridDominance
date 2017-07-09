using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities.Operation;

namespace GridDominance.Shared.Screens.NormalGameScreen.EntityOperations
{
	class CannonBooster : GameEntityOperation<Cannon>
	{
		public CannonBooster(float lifetime) : base("CannonBooster", lifetime)
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
