using System.Linq;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.Entities.Cannons;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
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
		private float timeUntilNextUpdate = 0.5f;
		private bool _isWaitingForQueue = false;

		public readonly Cannon Cannon;
		public readonly Fraction Fraction;

		public float UpdateWaitPercentage => FloatMath.IsEpsilonZero(updateInterval) ? 0 : timeUntilNextUpdate / updateInterval;

		protected readonly FCircle innerBoundings;

		protected AbstractFractionController(float interval, GDGameScreen owner, Cannon cannon, Fraction fraction, bool singleUpdatePerCycle)
		{
			updateInterval = interval;
			onlySingleUpdate = singleUpdatePerCycle;
			Cannon = cannon;
			Fraction = fraction;
			Owner = owner;

			if (fraction.IsPlayer) timeUntilNextUpdate = 0f;

			innerBoundings = new FCircle(Cannon.Position, Cannon.Scale * Cannon.CANNON_OUTER_DIAMETER / 2);
		}

		public void Update(SAMTime gameTime, InputState istate)
		{
			if (istate.IsExclusiveJustDown && innerBoundings.Contains(istate.GamePointerPositionOnMap))
			{
				istate.Swallow(InputConsumer.GameEntity);
				OnExclusiveDown(istate);
			}

			if (Fraction.KICycleWaitQueue.Any() && Owner.CannonMap[Fraction.KICycleWaitQueue.Peek()].Fraction != Fraction)
			{
				SAMLog.Warning("AFC::DirtyCycleQueue", $"Cycle Queue for {Fraction.Type} contains {Fraction.KICycleWaitQueue.Peek()} ({Owner.CannonMap[Fraction.KICycleWaitQueue.Peek()].Fraction.Type})");
				Fraction.KICycleWaitQueue.Clear();
				return;
			}

			bool queuePriority = false;
			if (Fraction.KICycleWaitQueue.Any() && Fraction.KICycleWaitQueue.Peek() == Cannon.BlueprintCannonID)
			{
				Fraction.KICycleWaitQueue.Dequeue();
				queuePriority = true;
			}

			timeUntilNextUpdate -= gameTime.ElapsedSeconds;
			if (timeUntilNextUpdate <= 0)
			{
				if (onlySingleUpdate)
				{
					if (timeUntilNextUpdate < -KIController.MAX_UPDATE_TIME)
					{
						SAMLog.Warning("AFC::QTIMEOUT", $"Overriding SingleUpdate condition - Max wait time overstepped for cannon {Cannon.BlueprintCannonID}");

						DoUpdate(gameTime, istate);
						return; // Fuck it, we have waited long enough, now it's our turn
					}

					if (Fraction.LastKiSingleCycle == MonoSAMGame.GameCycleCounter)
					{
						if (_isWaitingForQueue) Fraction.KICycleWaitQueue.Enqueue(Cannon.BlueprintCannonID);
						_isWaitingForQueue = true;
						return; // We want - but someone else was first
					}

					if (Fraction.KICycleWaitQueue.Count == 0)
					{
						DoUpdate(gameTime, istate);
						return; // We want and we don't have to wait
					}

					if (Fraction.KICycleWaitQueue.Count > 128)
					{
						SAMLog.Warning("AFC::QFULL", $"Fraction.KICycleWaitQueue is full for cannon {Cannon.BlueprintCannonID}");

						return; // Queue is full - we delay our update
					}

					if (queuePriority)
					{
						DoUpdate(gameTime, istate);
						return; // We want and its out turn
					}

					Fraction.KICycleWaitQueue.Enqueue(Cannon.BlueprintCannonID);
					_isWaitingForQueue = true;
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
			_isWaitingForQueue = false;
			timeUntilNextUpdate = updateInterval;

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
