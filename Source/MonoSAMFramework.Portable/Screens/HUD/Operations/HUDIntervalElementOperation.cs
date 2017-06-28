using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.HUD.Operations
{
	public abstract class HUDIntervalElementOperation<TElement> : HUDElementOperation<TElement> where TElement : HUDElement
	{
		private readonly float _delayTime;
		private readonly float _cycleTime;

		private float _time = 0f;

		public bool Finished = false;

		protected HUDIntervalElementOperation(float delayTime, float cycleTime)
		{
			_delayTime = delayTime;
			_cycleTime = cycleTime;
		}

		public override bool Update(TElement entity, SAMTime gameTime, InputState istate)
		{
			if (Finished) return false;

			
			bool before_incycle = (_time > _delayTime) && (_time < _delayTime + _cycleTime);

			_time += gameTime.ElapsedSeconds;
			while (_time > _delayTime + _cycleTime) _time -= _delayTime + _cycleTime;

			bool after_incycle = (_time > _delayTime) && (_time < _delayTime + _cycleTime);

			if (!before_incycle && after_incycle) OnCycleStart(entity, gameTime, istate);

			if (after_incycle) OnCycleProgress(entity, (_time - _delayTime) / _cycleTime, gameTime, istate);

			if (before_incycle && !after_incycle) OnCycleEnd(entity, gameTime, istate);

			
			return true;
		}

		protected abstract void OnCycleStart(TElement entity, SAMTime gameTime, InputState istate);
		protected abstract void OnCycleProgress(TElement entity, float progress, SAMTime gameTime, InputState istate);
		protected abstract void OnCycleEnd(TElement entity, SAMTime gameTime, InputState istate);
	}
}
