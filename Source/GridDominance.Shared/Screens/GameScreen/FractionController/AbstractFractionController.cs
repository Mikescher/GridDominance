using GridDominance.Shared.Screens.ScreenGame.Entities;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using MonoSAMFramework.Portable.Screens;

namespace GridDominance.Shared.Screens.ScreenGame.FractionController
{
	public abstract class AbstractFractionController
	{
		protected readonly GDGameScreen Owner;

		private readonly float updateInterval;
		private float timeSinceLastUpdate = 0;

		protected readonly Cannon Cannon;
		protected readonly Fraction Fraction;

		protected AbstractFractionController(float interval, GDGameScreen owner, Cannon cannon, Fraction fraction)
		{
			updateInterval = interval;
			Cannon = cannon;
			Fraction = fraction;
			Owner = owner;
		}

		public void Update(SAMTime gameTime, InputState istate)
		{
			timeSinceLastUpdate -= gameTime.ElapsedSeconds;
			if (timeSinceLastUpdate <= 0)
			{
				timeSinceLastUpdate = updateInterval;

				Calculate(istate);
			}
		}

		protected abstract void Calculate(InputState istate);
		public abstract bool DoBarrelRecharge();
	}
}
