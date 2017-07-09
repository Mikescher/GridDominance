using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.ScreenGame;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.NormalGameScreen.FractionController
{
	public class TutorialController : AbstractFractionController
	{
		public bool RechargeBarrel = false;

		public TutorialController(GDGameScreen owner, Cannon cannon) 
			: base(0.1f, owner, cannon, cannon.Fraction, false)
		{
		}

		protected override void Calculate(InputState istate)
		{
			//
		}

		public override bool DoBarrelRecharge() => RechargeBarrel;
		public override bool SimulateBarrelRecharge() => false;
	}
}
