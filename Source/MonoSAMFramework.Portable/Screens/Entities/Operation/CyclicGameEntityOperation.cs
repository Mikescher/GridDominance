using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.Entities.Operation
{
	public abstract class CyclicGameEntityOperation<TEntity> : GameEntityOperation<TEntity> where TEntity : GameEntity
	{
		private readonly bool _reliable;
		private readonly float _cyleTime;

		private int _cycleCounter = 0;
		private float _timeSinceLastCycle = 0f;

		protected CyclicGameEntityOperation(string n, float cycleTime, bool reliable) : base(n, null)
		{
			_cyleTime = cycleTime;
			_reliable = reliable;
		}

		protected override void OnProgress(TEntity entity, float progress, SAMTime gameTime, InputState istate)
		{
			_timeSinceLastCycle += gameTime.ElapsedSeconds;

			while (_timeSinceLastCycle > _cyleTime)
			{
				OnCycle(entity, _cycleCounter);

				if (_reliable)
					_timeSinceLastCycle -= _cyleTime;
				else
					_timeSinceLastCycle = 0;

				_cycleCounter++;

				if (_cyleTime < float.Epsilon) break; // prevent inf loop
			}
		}

		protected abstract void OnCycle(TEntity entity, int counter);

		protected override void OnEnd(TEntity entity)
		{
			//
		}
	}
}