using MonoSAMFramework.Portable.Input;
using System;

namespace MonoSAMFramework.Portable.Screens.Entities.Operation
{
	public class SimpleGameEntityOperation<TEntity> : GameEntityOperation<TEntity> where TEntity : GameEntity
	{
		private readonly Action<TEntity, float> cmd;

		public SimpleGameEntityOperation(float operationlength, Action<TEntity, float> command) : base(operationlength)
		{
			cmd = command;
		}

		protected override void OnStart(TEntity entity)
		{
			cmd(entity, 0);
		}

		protected override void OnProgress(TEntity entity, float progress, InputState istate)
		{
			cmd(entity, progress);
		}

		protected override void OnEnd(TEntity entity)
		{
			cmd(entity, 1);
		}
	}
}
