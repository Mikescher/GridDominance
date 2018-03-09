using System;
using System.Collections.Generic;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.Entities.Cannons;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;

namespace GridDominance.Shared.Screens.NormalGameScreen.FractionController
{
	class RelayKIController : KIController
	{
		private readonly List<KIMethod> intelligence;

		public override bool DoBarrelRecharge() => !Owner.IsCountdown;
		public override bool SimulateBarrelRecharge() => false;

		public RelayKIController(GDGameScreen owner, Cannon cannon, Fraction fraction)
			: base(RELAY_UPDATE_TIME, owner, cannon, fraction, 0f)
		{
			if (owner.Blueprint.KIType == LevelBlueprint.KI_TYPE_RAYTRACE)
			{
				intelligence = new List<KIMethod>
				{
					KIMethod.CreateRaycast("SupportCannon",         FindTargetSupportCannon),
					KIMethod.CreateRaycast("NeutralCannon",         FindTargetNeutralCannon),
					KIMethod.CreateRaycast("EnemyCannon",           FindTargetEnemyCannon),
					KIMethod.CreateRaycast("RelayChain",            FindTargetRelayChain),
					KIMethod.CreateRaycast("FriendlyCannon",        FindTargetFriendlyCannon),
					KIMethod.CreateRaycast("BlockedEnemyCannon",    FindTargetBlockedEnemyCannon),
					KIMethod.CreateRaycast("BlockedFriendlyCannon", FindTargetBlockedFriendlyCannon),
					KIMethod.CreateRaycast("NearestEnemyCannon",    FindNearestEnemyCannon),
				};
			}
			else if (owner.Blueprint.KIType == LevelBlueprint.KI_TYPE_PRECALC)
			{
				intelligence = new List<KIMethod>
				{
					KIMethod.CreatePrecalc("SupportCannon",         FindTargetSupportCannonPrecalc),
					KIMethod.CreatePrecalc("NeutralCannon",         FindTargetNeutralCannonPrecalc),
					KIMethod.CreatePrecalc("EnemyCannon",           FindTargetEnemyCannonPrecalc),
					KIMethod.CreatePrecalc("RelayChain",            FindTargetRelayChainPrecalc),
					KIMethod.CreatePrecalc("FriendlyCannon",        FindTargetFriendlyCannonPrecalc),
					KIMethod.CreatePrecalc("BlockedEnemyCannon",    FindTargetBlockedEnemyCannonPrecalc),
					KIMethod.CreatePrecalc("BlockedFriendlyCannon", FindTargetBlockedFriendlyCannonPrecalc),
					KIMethod.CreateRaycast("NearestEnemyCannon",    FindNearestEnemyCannon),
				};
			}
			else if (owner.Blueprint.KIType == LevelBlueprint.KI_TYPE_PRESIMULATE)
			{
				intelligence = new List<KIMethod>
				{
					KIMethod.CreatePrecalc("SupportCannon",         FindTargetSupportCannonPrecalc),
					KIMethod.CreatePrecalc("NeutralCannon",         FindTargetNeutralCannonPrecalc),
					KIMethod.CreatePrecalc("EnemyCannon",           FindTargetEnemyCannonPrecalc),
					KIMethod.CreatePrecalc("RelayChain",            FindTargetRelayChainPrecalc),
					KIMethod.CreatePrecalc("FriendlyCannon",        FindTargetFriendlyCannonPrecalc),
					KIMethod.CreatePrecalc("BlockedEnemyCannon",    FindTargetBlockedEnemyCannonPrecalc),
					KIMethod.CreatePrecalc("BlockedFriendlyCannon", FindTargetBlockedFriendlyCannonPrecalc),
					KIMethod.CreateRaycast("NearestEnemyCannon",    FindNearestEnemyCannon),
				};
			}
			else
				throw new Exception("Unknown KIType: " + owner.Blueprint.KIType);
		}

		protected override void Calculate(InputState istate)
		{
			CalculateKI(intelligence, true);
		}
	}
}
