using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;

namespace GridDominance.Shared.Screens.NormalGameScreen.FractionController
{
	class RemoteController : AbstractFractionController
	{
		private readonly bool _dbc;
		public override bool DoBarrelRecharge() => _dbc;

		public RemoteController(GDGameScreen owner, Cannon cannon, Fraction fraction, bool recharge) 
			: base(0f, owner, cannon, fraction, false)
		{
			_dbc = recharge;
		}

		public override void OnRemove()
		{
			Cannon.CrosshairSize.Set(0f);
		}

		protected override void OnExclusiveDown(InputState istate)
		{
			//
		}

		protected override void Calculate(InputState istate)
		{
			//
		}
	}
}
