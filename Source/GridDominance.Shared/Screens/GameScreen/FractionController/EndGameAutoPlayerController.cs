using System;
using System.Collections.Generic;
using GridDominance.Shared.Screens.ScreenGame.Entities;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.ScreenGame.FractionController
{
	class EndGameAutoPlayerController : KIController
	{
		private readonly List<KIMethod> intelligence;

		public override bool DoBarrelRecharge() => true;

		public EndGameAutoPlayerController(GDGameScreen owner, Cannon cannon, Fraction fraction)
			: base(STANDARD_UPDATE_TIME, owner, cannon, fraction)
		{
			intelligence = new List<KIMethod>
			{
				new KIMethod("AttackingBullet,",      FindTargetAttackingBullet),
				new KIMethod("SupportCannon",         FindTargetSupportCannon),
				new KIMethod("NeutralCannon",         FindTargetNeutralCannon),
				new KIMethod("EnemyCannon",           FindTargetEnemyCannon),
				new KIMethod("FriendlyCannon",        FindTargetFriendlyCannon),
				new KIMethod("BlockedEnemyCannon",    FindTargetBlockedEnemyCannon),
				new KIMethod("BlockedFriendlyCannon", FindTargetBlockedFriendlyCannon),
				new KIMethod("NearestEnemyCannon",    FindNearestEnemyCannon),
			};
		}

		protected override void Calculate(InputState istate)
		{
			CalculateKI(intelligence, true);
		}
	}
}
