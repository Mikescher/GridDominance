using System.Collections.Generic;
using System.Linq;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.UpdateAgents.Impl
{
	public class CyclicSequenceOperation<TOwner> : SAMUpdateOp<TOwner> where TOwner : IUpdateOperationOwner
	{
		private readonly List<IUpdateOperation> operations = new List<IUpdateOperation>();
		private int _index = 0;

		private IUpdateOperation current = null;

		public CyclicSequenceOperation(IUpdateOperation op1, params IUpdateOperation[] ops)
		{
			operations.Add(op1);
			operations.AddRange(ops);
		}
		
		protected override void OnUpdate(TOwner owner, SAMTime gameTime, InputState istate)
		{
			if (current == null)
			{
				current = operations[_index];
				current.FullReset();
				current.InitUnchecked(owner);

				_index = (_index + 1) % operations.Count;
			}

			var r = current.UpdateUnchecked(owner, gameTime, istate);
			if (!r) current = null;
		}

		protected override void OnAbort(TOwner owner)
		{
			if (current != null) current.ManualOnAbort(owner);
		}

		public override string Name => $"CYCLE [{_index}] => [{string.Join(", ",operations.Concat(operations).Skip((_index-1+ operations.Count)% operations.Count).Take(operations.Count).Select(o => o.Name))}]";
	}
}
