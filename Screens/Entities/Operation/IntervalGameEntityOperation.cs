
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.Entities.Operation
{
	public abstract class IntervalGameEntityOperation<TEntity> : GameEntityOperation<TEntity> where TEntity : GameEntity
	{
		private readonly float _delayTime;
		private readonly float _cycleTime;

		private float _time = 0f;

		public IntervalGameEntityOperation(string n, float delayTime, float cycleTime) : base(n, null)
		{
			_delayTime = delayTime;
			_cycleTime = cycleTime;
		}

		protected override void OnStart(TEntity entity)
		{
			//
		}

		protected override void OnProgress(TEntity entity, float progress, SAMTime gameTime, InputState istate)
		{
			bool before_incycle = (_time > _delayTime) && (_time < _delayTime + _cycleTime);

			_time += gameTime.ElapsedSeconds;
			while (_time > _delayTime + _cycleTime) _time -= _delayTime + _cycleTime;

			bool after_incycle = (_time > _delayTime) && (_time < _delayTime + _cycleTime);

			if (!before_incycle && after_incycle) OnCycleStart(entity, gameTime, istate);

			if (after_incycle) OnCycleProgress(entity, (_time - _delayTime) / _cycleTime, gameTime, istate);

			if (before_incycle && !after_incycle) OnCycleEnd(entity, gameTime, istate);
		}

		protected abstract void OnCycleStart(TEntity entity, SAMTime gameTime, InputState istate);
		protected abstract void OnCycleProgress(TEntity entity, float progress, SAMTime gameTime, InputState istate);
		protected abstract void OnCycleEnd(TEntity entity, SAMTime gameTime, InputState istate);

		protected override void OnEnd(TEntity entity)
		{
			//
		}

		protected override void OnAbort(TEntity entity)
		{
			//
		}
	}
}
