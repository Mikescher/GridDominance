using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.UpdateAgents.Impl
{
	public abstract class IntervalOperation<TElement> : SAMUpdateOp<TElement> where TElement : IUpdateOperationOwner
	{
		private readonly float _actionTime;
		private readonly float _sleepTime;

		private float _time = 0f;

		// [startDelay] ( [actionTime] [sleepTime] ) ( [actionTime] [sleepTime] ) ( [actionTime] [sleepTime] ) ( [actionTime] [sleepTime] ) ...

		protected IntervalOperation(float startDelay, float actionTime, float sleepTime)
		{
			_time = -startDelay;

			_actionTime = actionTime;
			_sleepTime = sleepTime;
		}

		protected override void OnUpdate(TElement entity, SAMTime gameTime, InputState istate)
		{
			bool before_incycle = (_time >= 0) && (_time < _actionTime);

			_time += gameTime.ElapsedSeconds;

			if (_time < 0) return; // initial delay

			while (_time > _actionTime + _sleepTime) _time -= _actionTime + _sleepTime;

			bool after_incycle = (_time >= 0) && (_time < _actionTime);

			if (!before_incycle && after_incycle) OnCycleStart(entity, gameTime, istate);

			if (after_incycle) OnCycleProgress(entity, _time / _actionTime, gameTime, istate);

			if (before_incycle && !after_incycle) OnCycleEnd(entity, gameTime, istate);
		}

		protected abstract void OnCycleStart(TElement entity, SAMTime gameTime, InputState istate);
		protected abstract void OnCycleProgress(TElement entity, float progress, SAMTime gameTime, InputState istate);
		protected abstract void OnCycleEnd(TElement entity, SAMTime gameTime, InputState istate);
	}
}
