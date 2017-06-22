using MonoSAMFramework.Portable.Input;
using System;

namespace MonoSAMFramework.Portable.Screens.Entities.Operation
{
	public class CyclicGameEntityOperation<TEntity> : GameEntityOperation<TEntity> where TEntity : GameEntity
	{
		private readonly Action<TEntity, int> _cmd;
		private readonly bool _reliable;
		private readonly float _cyleTime;

		private int _cycleCounter = 0;
		private float _timeSinceLastCycle = 0f;

		public CyclicGameEntityOperation(string n, float cycleTime, bool reliable, Action<TEntity, int> command) : base(n, null)
		{
			_cyleTime = cycleTime;
			_cmd = command;
			_reliable = reliable;
		}

		protected override void OnStart(TEntity entity)
		{
			//
		}

		protected override void OnProgress(TEntity entity, float progress, SAMTime gameTime, InputState istate)
		{
			_timeSinceLastCycle += gameTime.ElapsedSeconds;

			if (_timeSinceLastCycle > _cyleTime)
			{
				_cmd(entity, _cycleCounter);

				if (_reliable)
					_timeSinceLastCycle -= _cyleTime;
				else
					_timeSinceLastCycle = 0;

				_cycleCounter++;
			}
		}

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
