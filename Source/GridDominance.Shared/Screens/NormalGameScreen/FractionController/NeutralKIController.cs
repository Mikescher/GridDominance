using System.Collections.Generic;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;

namespace GridDominance.Shared.Screens.NormalGameScreen.FractionController
{
	class NeutralKIController : KIController
	{
		private readonly List<KIMethod> intelligence;

		private bool doBarrelRecharge = false;
		public override bool SimulateBarrelRecharge() => false;

		public override bool DoBarrelRecharge() => doBarrelRecharge;

		public NeutralKIController(GDGameScreen owner, Cannon cannon, Fraction fraction)
			: base(NEUTRAL_UPDATE_TIME, owner, cannon, fraction, 0f)
		{
			intelligence = new List<KIMethod>
			{
				KIMethod.CreateDefense("AttackingLaser",  FindTargetAttackingLaser),
				KIMethod.CreateRaycast("AttackingBullet", FindTargetAttackingBullet),
			};
		}

		protected override void Calculate(InputState istate)
		{
			doBarrelRecharge = CalculateKI(intelligence, false);
		}
	}
}
