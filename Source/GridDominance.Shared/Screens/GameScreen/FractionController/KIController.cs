using GridDominance.Shared.Framework;
using GridDominance.Shared.Screens.GameScreen.Entities;

namespace GridDominance.Shared.Screens.GameScreen.FractionController
{
	class KIController : AbstractFractionController
	{
		public KIController(GameScreen owner, Cannon cannon, Fraction fraction) : base(COMPUTER_UPDATE_TIME, owner, cannon, fraction)
		{
		}

		protected override void Calculate(InputState istate)
		{
			//TODO Implement
		}
	}
}
