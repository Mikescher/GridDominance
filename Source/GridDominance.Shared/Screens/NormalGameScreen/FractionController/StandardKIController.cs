using System.Collections.Generic;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;

namespace GridDominance.Shared.Screens.NormalGameScreen.FractionController
{
	class StandardKIController : KIController
	{
		private readonly List<KIMethod> intelligence;

		public override bool DoBarrelRecharge() => true;

		public StandardKIController(GDGameScreen owner, Cannon cannon, Fraction fraction)
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
