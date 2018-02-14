using System;

namespace MonoSAMFramework.Portable.UpdateAgents.Impl
{
	public class CyclicLambdaOperation<TEntity> : CyclicOperation<TEntity> where TEntity : IUpdateOperationOwner
	{
		private readonly Action<TEntity, int> _cmd;

		public override string Name { get; }

		public CyclicLambdaOperation(string n, float cycleTime, bool reliable, Action<TEntity, int> command) : base(cycleTime, reliable)
		{
			Name = n;

			_cmd = command;
		}
		
		protected override void OnCycle(TEntity element, int counter)
		{
			_cmd(element, counter);
		}
	}
}