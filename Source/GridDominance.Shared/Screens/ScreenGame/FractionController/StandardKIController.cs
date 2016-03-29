using System;
using System.Collections.Generic;
using GridDominance.Shared.Screens.ScreenGame.Entities;
using MonoSAMFramework.Portable.Input;

namespace GridDominance.Shared.Screens.ScreenGame.FractionController
{
	class StandardKIController : KIController
	{
		private readonly List<Func<GDEntity>> intelligence;

		public override bool DoBarrelRecharge() => true;

		public StandardKIController(GDGameScreen owner, Cannon cannon, Fraction fraction)
			: base(STANDARD_UPDATE_TIME, owner, cannon, fraction)
		{
			intelligence = new List<Func<GDEntity>>
			{
				FindTargetAttackingBullet,
				FindTargetSupportCannon,
				FindTargetNeutralCannon,
				FindTargetEnemyCannon,
				FindTargetFriendlyCannon,
				FindTargetBlockedEnemyCannon,
				FindTargetBlockedFriendlyCannon,
				FindNearestEnemyCannon
			};
		}

		protected override void Calculate(InputState istate)
		{
			CalculateKI(intelligence, true);
		}
	}
}
