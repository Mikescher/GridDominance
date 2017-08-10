using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;

namespace GridDominance.Shared.Screens.NormalGameScreen.FractionController
{
	class RemoteController : AbstractFractionController
	{
		private readonly bool _dbc;
		private readonly bool _sbc;
		public override bool DoBarrelRecharge() => _dbc && !Owner.IsCountdown;
		public override bool SimulateBarrelRecharge() => _sbc && !Owner.IsCountdown && !Fraction.IsNeutral;

		public RemoteController(GDGameScreen owner, Cannon cannon, Fraction fraction, bool recharge, bool recoilSim) 
			: base(0f, owner, cannon, fraction, false)
		{
			_dbc = recharge;
			_sbc = recoilSim;
		}

		public override void OnRemove()
		{
			base.OnRemove();
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
