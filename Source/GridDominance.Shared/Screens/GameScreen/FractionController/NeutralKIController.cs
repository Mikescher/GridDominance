using System;
using System.Collections.Generic;
using GridDominance.Shared.Screens.ScreenGame.Entities;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.ScreenGame.FractionController
{
	class NeutralKIController : KIController
	{
		private readonly List<KIMethod> intelligence;

		private bool doBarrelRecharge = false;

		public override bool DoBarrelRecharge() => doBarrelRecharge;

		public NeutralKIController(GDGameScreen owner, Cannon cannon, Fraction fraction)
			: base(NEUTRAL_UPDATE_TIME, owner, cannon, fraction)
		{
			intelligence = new List<KIMethod>
			{
				new KIMethod("AttackingBullet", FindTargetAttackingBullet),
			};
		}

		protected override void Calculate(InputState istate)
		{
			doBarrelRecharge = CalculateKI(intelligence, false);
		}
	}
}
