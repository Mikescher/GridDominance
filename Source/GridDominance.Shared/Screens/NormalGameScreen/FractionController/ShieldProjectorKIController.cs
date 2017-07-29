using System;
using System.Collections.Generic;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;

namespace GridDominance.Shared.Screens.NormalGameScreen.FractionController
{
	class ShieldProjectorKIController : KIController
	{
		private readonly List<KIMethod> intelligence;

		public override bool DoBarrelRecharge() => !Owner.IsCountdown;
		public override bool SimulateBarrelRecharge() => false;

		public ShieldProjectorKIController(GDGameScreen owner, Cannon cannon, Fraction fraction)
			: base(LASER_UPDATE_TIME, owner, cannon, fraction, MIN_LASER_ROT)
		{
			if (owner.Blueprint.KIType == LevelBlueprint.KI_TYPE_RAYTRACE)
			{
				intelligence = new List<KIMethod>
				{
					KIMethod.CreateDefense("AttackingLaser",        FindTargetAttackingLaser),
					KIMethod.CreateRaycast("ShieldCannon",          FindTargetShieldCannon),
					KIMethod.CreateRaycast("FriendlyCannon",        FindTargetFriendlyCannon),
					KIMethod.CreateRaycast("SupportCannon",         FindTargetSupportCannon),
					KIMethod.CreateRaycast("BlockedFriendlyCannon", FindTargetBlockedFriendlyCannon),
				};
			}
			else if (owner.Blueprint.KIType == LevelBlueprint.KI_TYPE_PRECALC)
			{
				intelligence = new List<KIMethod>
				{
					KIMethod.CreateDefense("AttackingLaser",        FindTargetAttackingLaser),
					KIMethod.CreatePrecalc("ShieldCannon",          FindTargetShieldCannonPrecalc),
					KIMethod.CreatePrecalc("FriendlyCannon",        FindTargetFriendlyCannonPrecalc),
					KIMethod.CreatePrecalc("SupportCannon",         FindTargetSupportCannonPrecalc),
				};
			}
			else if (owner.Blueprint.KIType == LevelBlueprint.KI_TYPE_PRESIMULATE)
			{
				intelligence = new List<KIMethod>
				{
					KIMethod.CreateDefense("AttackingLaser",        FindTargetAttackingLaser),
					KIMethod.CreatePrecalc("ShieldCannon",          FindTargetShieldCannonPrecalc),
					KIMethod.CreatePrecalc("FriendlyCannon",        FindTargetFriendlyCannonPrecalc),
					KIMethod.CreatePrecalc("SupportCannon",         FindTargetSupportCannonPrecalc),
					KIMethod.CreatePrecalc("BlockedFriendlyCannon", FindTargetBlockedFriendlyCannonPrecalc),
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
