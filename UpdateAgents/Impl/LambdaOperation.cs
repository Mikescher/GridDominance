using System;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.UpdateAgents.Impl
{
	public class LambdaOperation<TEntity> : FixTimeOperation<TEntity> where TEntity : IUpdateOperationOwner
	{
		private static readonly Action<TEntity> LAMBDA_EMPTY = e => { };

		private readonly Action<TEntity, float> cmd;

		private readonly Action<TEntity> cmdStart;
		private readonly Action<TEntity> cmdEnd;

		public override string Name { get; }

		public LambdaOperation(string n, float operationlength, Action<TEntity, float> command) : base(operationlength)
		{
			Name = n;

			cmd = command;
			cmdStart = LAMBDA_EMPTY;
			cmdEnd = LAMBDA_EMPTY;
		}

		public LambdaOperation(string n, float operationlength, Action<TEntity, float> command, Action<TEntity> start, Action<TEntity> end) : base(operationlength)
		{
			Name = n;

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
