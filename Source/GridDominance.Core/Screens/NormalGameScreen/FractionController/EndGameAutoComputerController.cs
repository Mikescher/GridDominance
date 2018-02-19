using System.Collections.Generic;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.Extensions;

namespace GridDominance.Shared.Screens.NormalGameScreen.FractionController
{
	class EndGameAutoComputerController : KIController
	{
		private readonly List<KIMethod> intelligence;

		public override bool DoBarrelRecharge() => false;
		public override bool SimulateBarrelRecharge() => false;

		private float last = 0f;

		public EndGameAutoComputerController(GDGameScreen owner, Cannon cannon, Fraction fraction)
			: base(STANDARD_UPDATE_TIME, owner, cannon, fraction, 0f)
		{
			intelligence = new List<KIMethod>
			{
				KIMethod.CreateCustom("RotCenter", RotCenter),
			};

			last = MonoSAMGame.CurrentTime.TotalElapsedSeconds;
		}

		private float? RotCenter()
		{
			var p = Cannon.Owner.MapFullBounds.Center - Cannon.Position;

			if (p.IsZero()) return 0f;

			return p.ToAngle();
		}

		protected override void Calculate(InputState istate)
		{
			CalculateKI(intelligence, true);
		}
	}
}
