using System;
using System.Collections.Generic;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using MonoSAMFramework.Portable;

namespace GridDominance.Shared.Screens.NormalGameScreen.FractionController
{
	class EndGameAutoPlayerController : KIController
	{
		private readonly List<KIMethod> intelligence;

		public override bool DoBarrelRecharge() => false;

		private float last = 0f;

		public EndGameAutoPlayerController(GDGameScreen owner, Cannon cannon, Fraction fraction)
			: base(STANDARD_UPDATE_TIME, owner, cannon, fraction)
		{
			intelligence = new List<KIMethod>
			{
				KIMethod.CreateGeneric("IdleRotate", IdleRotate),
			};

			last = MonoSAMGame.CurrentTime.TotalElapsedSeconds;
		}

		private void IdleRotate(KIController ki)
		{
			ki.Cannon.Rotation.Set(ki.Cannon.Rotation.ActualValue + (MonoSAMGame.CurrentTime.ElapsedSeconds - last));

			last = MonoSAMGame.CurrentTime.TotalElapsedSeconds;
		}

		protected override void Calculate(InputState istate)
		{
			CalculateKI(intelligence, true);
		}
	}
}
