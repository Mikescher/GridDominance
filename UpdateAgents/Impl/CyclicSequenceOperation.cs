using System;
using System.Collections.Generic;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.UpdateAgents.Impl
{
	public class CyclicSequenceOperation<TOwner> : SAMUpdateOp<TOwner> where TOwner : IUpdateOperationOwner
	{
		private readonly List<Func<IUpdateOperation>> operations = new List<Func<IUpdateOperation>>();
		private int _index = 0;

		private IUpdateOperation current = null;

		public CyclicSequenceOperation(Func<IUpdateOperation> op1, params Func<IUpdateOperation>[] ops)
		{
			operations.Add(op1);
			operations.AddRange(ops);
		}
		
		protected override void OnUpdate(TOwner owner, SAMTime gameTime, InputState istate)
		{
			if (current == null)
			{
				current = operations[_index]();
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

		public override string Name => $"CYCLE [{_index}] => {current?.Name}";
	}
}
