using GridDominance.Shared.Screens.ScreenGame.Entities;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.ScreenGame.FractionController
{
	class EmptyController : AbstractFractionController
	{
		public override bool DoBarrelRecharge() => false;

		public EmptyController(GDGameScreen owner, Cannon cannon, Fraction fraction) 
			: base(0, owner, cannon, fraction)
		{
		}

		protected override void Calculate(InputState istate)
		{
			// DO NOTHING
		}
	}
}
