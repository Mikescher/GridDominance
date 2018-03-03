using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;

namespace GridDominance.Shared.Screens.NormalGameScreen.FractionController
{
	class EmptyController : AbstractFractionController
	{
		public override bool DoBarrelRecharge() => false;
		public override bool SimulateBarrelRecharge() => false;

		public EmptyController(GDGameScreen owner, Cannon cannon, Fraction fraction) 
			: base(0, owner, cannon, fraction, false)
		{
		}

		protected override void Calculate(InputState istate)
		{
			// DO NOTHING

			Cannon.KITarget = null;
		}
	}
}
