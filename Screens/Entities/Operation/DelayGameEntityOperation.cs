using MonoSAMFramework.Portable.Input;
using System;

namespace MonoSAMFramework.Portable.Screens.Entities.Operation
{
	public class DelayGameEntityOperation : IGameEntityOperation
	{
		private readonly IGameEntityOperation _chain;
		private readonly float _length;

		private float _time;

		public float Progress { get; private set; }
		public string Name { get; private set; }

		public DelayGameEntityOperation(string n, float operationlength, IGameEntityOperation nextOp)
		{
			_time = 0;
			_chain = nextOp;
			_length = operationlength;

			Name = n;
		}
		
		public bool Update(GameEntity entity, SAMTime gameTime, InputState istate)
		{
			_time += gameTime.ElapsedSeconds;

			Progress = _time / _length;

			return _time < _length;
		}

		void IGameEntityOperation.OnStart(GameEntity gameEntity)
		{
			//
		}

		void IGameEntityOperation.OnEnd(GameEntity gameEntity)
		{
			gameEntity.AddEntityOperation(_chain);
		}

		void IGameEntityOperation.OnAbort(GameEntity gameEntity)
		{
			//
		}

		public void ForceSetProgress(float p)
		{
			_time = p * _length;
		}
	}
}
