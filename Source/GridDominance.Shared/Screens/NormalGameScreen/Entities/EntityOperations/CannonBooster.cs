using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities.Operation;

namespace GridDominance.Shared.Screens.NormalGameScreen.EntityOperations
{
	class CannonBooster : GameEntityOperation<Cannon>
	{
		private readonly float boostPower;

		public CannonBooster(float boost, float lifetime) : base("CannonBooster", lifetime)
		{
			boostPower = boost;
		}

		protected override void OnStart(Cannon entity)
		{
			entity.TotalBoost += boostPower;
		}

		protected override void OnProgress(Cannon entity, float progress, SAMTime gameTime, InputState istate) { }

		protected override void OnEnd(Cannon entity)
		{
			entity.TotalBoost -= boostPower;
		}

		protected override void OnAbort(Cannon entity)
		{
			entity.TotalBoost -= boostPower;
		}
	}
}
