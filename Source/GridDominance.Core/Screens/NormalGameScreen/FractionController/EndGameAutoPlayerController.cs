using System.Collections.Generic;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.Entities.Cannons;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using MonoSAMFramework.Portable;

namespace GridDominance.Shared.Screens.NormalGameScreen.FractionController
{
	class EndGameAutoPlayerController : KIController
	{
		private readonly List<KIMethod> intelligence;

		public override bool DoBarrelRecharge() => false;
		public override bool SimulateBarrelRecharge() => false;

		private float last = 0f;

		public EndGameAutoPlayerController(GDGameScreen owner, Cannon cannon, Fraction fraction)
			: base(STANDARD_UPDATE_TIME, owner, cannon, fraction, 0f)
		{
			intelligence = new List<KIMethod>
			{
				KIMethod.CreateGeneric("IdleRotate", IdleRotate),
			};

			last = MonoSAMGame.CurrentTime.TotalElapsedSeconds;
		}

		private void IdleRotate()
		{
			Cannon.Rotation.Set(Cannon.Rotation.ActualValue + (MonoSAMGame.CurrentTime.ElapsedSeconds - last));

			last = MonoSAMGame.CurrentTime.TotalElapsedSeconds;
		}

		protected override void Calculate(InputState istate)
		{
			CalculateKI(intelligence, true);
		}
	}
}
