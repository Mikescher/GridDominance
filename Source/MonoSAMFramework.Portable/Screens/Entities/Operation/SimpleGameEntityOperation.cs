using MonoSAMFramework.Portable.Input;
using System;

namespace MonoSAMFramework.Portable.Screens.Entities.Operation
{
	public class SimpleGameEntityOperation<TEntity> : GameEntityOperation<TEntity> where TEntity : GameEntity
	{
		private static readonly Action<TEntity> LAMBDA_EMPTY = e => { };

		private readonly Action<TEntity, float> cmd;

		private readonly Action<TEntity> cmdStart;
		private readonly Action<TEntity> cmdEnd;

		public SimpleGameEntityOperation(string n, float operationlength, Action<TEntity, float> command) : base(n, operationlength)
		{
			cmd = command;
			cmdStart = LAMBDA_EMPTY;
			cmdEnd = LAMBDA_EMPTY;
		}

		public SimpleGameEntityOperation(string n, float operationlength, Action<TEntity, float> command, Action<TEntity> start, Action<TEntity> end) : base(n, operationlength)
		{
			cmd = command;
			cmdStart = start;
			cmdEnd = end;
		}

		protected override void OnStart(TEntity entity)
		{
			cmdStart(entity);
			cmd(entity, 0);
		}

		protected override void OnProgress(TEntity entity, float progress, SAMTime gameTime, InputState istate)
		{
			cmd(entity, progress);
		}

		protected override void OnEnd(TEntity entity)
		{
			cmd(entity, 1);
			cmdEnd(entity);
		}

		protected override void OnAbort(TEntity entity)
		{
			//
		}
	}
}
