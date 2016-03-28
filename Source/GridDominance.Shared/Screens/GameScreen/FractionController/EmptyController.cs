using GridDominance.Shared.Screens.GameScreen.Entities;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.GameScreen.FractionController
{
	class EmptyController : AbstractFractionController
	{
		public override bool DoBarrelRecharge() => false;

		public EmptyController(GameScreen owner, Cannon cannon, Fraction fraction) 
			: base(0, owner, cannon, fraction)
		{
		}

		protected override void Calculate(InputState istate)
		{
			// DO NOTHING
		}
	}
}
