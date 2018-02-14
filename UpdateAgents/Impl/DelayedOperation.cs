using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.UpdateAgents.Impl
{
	public class DelayedOperation<TOwner> : SAMUpdateOp<TOwner>, IProgressableOperation where TOwner : IUpdateOperationOwner
	{
		private readonly SAMUpdateOp<TOwner> _operation = null;

		private readonly float _offset;

		public float Progress => Lifetime / _offset;

		public DelayedOperation(SAMUpdateOp<TOwner> op, float delay)
		{
			_offset = delay;
			_operation = op;
		}

		protected override void OnUpdate(TOwner owner, SAMTime gameTime, InputState istate)
		{
			if (Lifetime < _offset) return;

			owner.AddOperation(_operation);
			Finish();
		}

		public override string Name => "{DELAY} + " + _operation.Name;
	}
}
