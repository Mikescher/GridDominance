using GridDominance.Shared.Framework;
using GridDominance.Shared.Screens.GameScreen.Entities;

namespace GridDominance.Shared.Screens.GameScreen.FractionController
{
	class EmptyController : AbstractFractionController
	{
		public EmptyController(GameScreen owner, Cannon cannon, Fraction fraction) : base(0, owner, cannon, fraction)
		{
		}

		protected override void Calculate(InputState istate)
		{
			// DO NOTHING
		}
	}
}
