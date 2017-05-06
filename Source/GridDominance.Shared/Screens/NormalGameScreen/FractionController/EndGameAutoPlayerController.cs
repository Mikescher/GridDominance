using System;
using System.Collections.Generic;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;

namespace GridDominance.Shared.Screens.NormalGameScreen.FractionController
{
	class EndGameAutoPlayerController : KIController
	{
		private readonly List<KIMethod> intelligence;

		public override bool DoBarrelRecharge() => true;

		public EndGameAutoPlayerController(GDGameScreen owner, Cannon cannon, Fraction fraction)
			: base(STANDARD_UPDATE_TIME, owner, cannon, fraction)
		{
			if (owner.Blueprint.KIType == LevelBlueprint.KI_TYPE_RAYTRACE)
			{
				intelligence = new List<KIMethod>
				{
					KIMethod.CreateRaycast("SupportCannon",         FindTargetSupportCannon),
					KIMethod.CreateRaycast("FriendlyCannon",        FindTargetFriendlyCannon),
					KIMethod.CreateRaycast("BlockedFriendlyCannon", FindTargetBlockedFriendlyCannon),
					KIMethod.CreateRaycast("NearestFriendlyCannon", FindNearestFriendlyCannon),
				};
			}
			else if (owner.Blueprint.KIType == LevelBlueprint.KI_TYPE_PRECALC)
			{

				intelligence = new List<KIMethod>
				{
					KIMethod.CreatePrecalc("SupportCannon",         FindTargetSupportCannonPrecalc),
					KIMethod.CreatePrecalc("FriendlyCannon",        FindTargetFriendlyCannonPrecalc),
					KIMethod.CreatePrecalc("BlockedFriendlyCannon", FindTargetBlockedFriendlyCannonPrecalc),
					KIMethod.CreateRaycast("NearestFriendlyCannon", FindNearestFriendlyCannon),
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
