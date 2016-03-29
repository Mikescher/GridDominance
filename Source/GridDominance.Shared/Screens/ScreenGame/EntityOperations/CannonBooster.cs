using GridDominance.Shared.Screens.GameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.GameScreen.EntityOperations
{
	class CannonBooster : GameEntityOperation<Cannon>
	{
		private readonly float boostPower;

		public CannonBooster(float boost, float lifetime) : base(lifetime)
		{
			boostPower = boost;
		}

		protected override void OnStart(Cannon entity)
		{
			entity.TotalBoost += boostPower;
		}

		protected override void OnProgress(Cannon entity, float progress, InputState istate) { }

		protected override void OnEnd(Cannon entity)
		{
			entity.TotalBoost -= boostPower;

		}
	}
}
