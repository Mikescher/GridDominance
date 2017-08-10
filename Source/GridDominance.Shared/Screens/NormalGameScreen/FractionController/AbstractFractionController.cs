using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.LogProtocol;
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

		public float UpdateWaitPercentage => FloatMath.IsEpsilonZero(updateInterval) ? 0 : timeSinceLastUpdate / updateInterval;

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
				if (onlySingleUpdate)
				{
					if (Fraction.LastKiSingleCycle == MonoSAMGame.GameCycleCounter)
					{
						Fraction.KICycleWaitQueue.Enqueue(Cannon.BlueprintCannonID);
						return; // We want - but someone else was first
					}

					if (Fraction.KICycleWaitQueue.Count == 0)
					{
						DoUpdate(gameTime, istate);
						return; // We want and we don't have to wait
					}

					if (Fraction.KICycleWaitQueue.Peek() == Cannon.BlueprintCannonID)
					{
						Fraction.KICycleWaitQueue.Dequeue();
						DoUpdate(gameTime, istate);
						return; // We want and its out turn
					}

					if (Owner.CannonMap[Fraction.KICycleWaitQueue.Peek()].Fraction != Fraction)
					{
						SAMLog.Warning("AFC::DirtyCycleQueue", $"Cycle Queue for {Fraction.Type} contains {Fraction.KICycleWaitQueue.Peek()} ({Owner.CannonMap[Fraction.KICycleWaitQueue.Peek()].Fraction.Type})");
						Fraction.KICycleWaitQueue.Clear();
						return;
					}

					Fraction.KICycleWaitQueue.Enqueue(Cannon.BlueprintCannonID);
					return; // We want - but someone else has priority
				}
				else
				{
					DoUpdate(gameTime, istate);
					return;
				}
			}
		}

		private void DoUpdate(SAMTime gameTime, InputState istate)
		{
			timeSinceLastUpdate = updateInterval;

			if (onlySingleUpdate) Fraction.LastKiSingleCycle = MonoSAMGame.GameCycleCounter;
			Calculate(istate);
		}

		public virtual void OnRemove()
		{
			Fraction.KICycleWaitQueue.Clear();
		}

		protected virtual void OnExclusiveDown(InputState istate) { /* OVERRIDE ME */ }

		protected abstract void Calculate(InputState istate);
		public abstract bool DoBarrelRecharge();
		public abstract bool SimulateBarrelRecharge();
	}
}
