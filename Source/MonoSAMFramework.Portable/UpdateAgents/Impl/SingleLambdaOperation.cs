using System;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.UpdateAgents.Impl
{
	public class SingleLambdaOperation<TEntity> : SingleTimeOperation<TEntity> where TEntity : IUpdateOperationOwner
	{
		private readonly Action<TEntity> cmd;
		
		public override string Name { get; }

		public SingleLambdaOperation(string n, Action<TEntity> command) : base()
		{
			Name = n;

			cmd = command;
		}

		protected override void OnExecute(TEntity owner, SAMTime gameTime, InputState istate)
		{
			cmd(owner);
		}
	}
}
