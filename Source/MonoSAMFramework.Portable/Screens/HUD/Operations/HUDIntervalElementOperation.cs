using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.HUD.Operations
{
	public abstract class HUDIntervalElementOperation<TElement> : HUDElementOperation<TElement> where TElement : HUDElement
	{
		private readonly float _actionTime;
		private readonly float _sleepTime;

		private float _time = 0f;

		public bool Finished = false;

		protected HUDIntervalElementOperation(float startDelay, float actionTime, float sleepTime)
		{
			_time = -startDelay;

			_actionTime = actionTime;
			_sleepTime = sleepTime;
		}

		public override bool Update(TElement entity, SAMTime gameTime, InputState istate)
		{
			if (Finished) return false;
			
			bool before_incycle = (_time >= 0) && (_time < _actionTime);

			_time += gameTime.ElapsedSeconds;

			if (_time < 0) return true; // initial delay

			while (_time > _actionTime + _sleepTime) _time -= _actionTime + _sleepTime;

			bool after_incycle = (_time >= 0) && (_time < _actionTime);

			if (!before_incycle && after_incycle) OnCycleStart(entity, gameTime, istate);

			if (after_incycle) OnCycleProgress(entity, _time / _actionTime, gameTime, istate);

			if (before_incycle && !after_incycle) OnCycleEnd(entity, gameTime, istate);

			
			return true;
		}

		protected abstract void OnCycleStart(TElement entity, SAMTime gameTime, InputState istate);
		protected abstract void OnCycleProgress(TElement entity, float progress, SAMTime gameTime, InputState istate);
		protected abstract void OnCycleEnd(TElement entity, SAMTime gameTime, InputState istate);
	}
}
