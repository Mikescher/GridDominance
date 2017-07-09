using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens;

namespace GridDominance.Shared.Screens.NormalGameScreen.FractionController
{
	public abstract class AbstractFractionController
	{
		protected readonly GDGameScreen Owner;

		private readonly float updateInterval;
		private readonly bool onlySingleUpdate;
		private float timeSinceLastUpdate = 0.5f;

		public readonly Cannon Cannon;
		public readonly Fraction Fraction;

		protected readonly FCircle innerBoundings;

		protected AbstractFractionController(float interval, GDGameScreen owner, Cannon cannon, Fraction fraction, bool singleUpdatePerCycle)
		{
			updateInterval = interval;
			onlySingleUpdate = singleUpdatePerCycle;
			Cannon = cannon;
			Fraction = fraction;
			Owner = owner;

			if (fraction.IsPlayer) timeSinceLastUpdate = 0f;

			innerBoundings = new FCircle(Cannon.Position, Cannon.Scale * Cannon.CANNON_OUTER_DIAMETER / 2);
		}

		public void Update(SAMTime gameTime, InputState istate)
		{
			if (istate.IsExclusiveJustDown && innerBoundings.Contains(istate.GamePointerPositionOnMap))
			{
				istate.Swallow(InputConsumer.GameEntity);
				OnExclusiveDown(istate);
			}
			
			timeSinceLastUpdate -= gameTime.ElapsedSeconds;
			if (timeSinceLastUpdate <= 0)
			{
				if (onlySingleUpdate && Fraction.LastKiCycle == MonoSAMGame.GameCycleCounter) return;
				
				timeSinceLastUpdate = updateInterval;

				Fraction.LastKiCycle = MonoSAMGame.GameCycleCounter;
				Calculate(istate);
			}
		}

		public virtual void OnRemove() { /* OVERRIDE ME */ }

		protected virtual void OnExclusiveDown(InputState istate) { /* OVERRIDE ME */ }

		protected abstract void Calculate(InputState istate);
		public abstract bool DoBarrelRecharge();
		public abstract bool SimulateBarrelRecharge();
	}
}
