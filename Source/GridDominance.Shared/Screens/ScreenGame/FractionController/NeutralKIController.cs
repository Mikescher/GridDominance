using System;
using System.Collections.Generic;
using GridDominance.Shared.Screens.ScreenGame.Entities;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.ScreenGame.Fractions;

namespace GridDominance.Shared.Screens.ScreenGame.FractionController
{
	class NeutralKIController : KIController
	{
		private readonly List<Func<GDEntity>> intelligence;

		private bool doBarrelRecharge = false;

		public override bool DoBarrelRecharge() => doBarrelRecharge;

		public NeutralKIController(GDGameScreen owner, Cannon cannon, Fraction fraction)
			: base(NEUTRAL_UPDATE_TIME, owner, cannon, fraction)
		{
			intelligence = new List<Func<GDEntity>>
			{
				FindTargetAttackingBullet
			};
		}

		protected override void Calculate(InputState istate)
		{
			doBarrelRecharge = CalculateKI(intelligence, false);
		}
	}
}
