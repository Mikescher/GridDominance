using System;
using System.Collections.Generic;
using System.Linq;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD;

namespace MonoSAMFramework.Portable.UpdateAgents.Impl
{
	public class SequenceOperation<TElement> : SAMUpdateOp<TElement> where TElement : IUpdateOperationOwner
	{
		private readonly Queue<IUpdateOperation> opQueue = new Queue<IUpdateOperation>();

		private IUpdateOperation current = null;

		private readonly Action<TElement> actionStart;
		private readonly Action<TElement> actionEnd;

		public SequenceOperation(IUpdateOperation op1, params IUpdateOperation[] ops)
		{
			opQueue.Enqueue(op1);
			foreach (var op in ops)
				opQueue.Enqueue(op);

			actionStart = null;
			actionEnd = null;
		}

		public SequenceOperation(Action<TElement> aInit, Action<TElement> aFinal, IUpdateOperation op1, params IUpdateOperation[] ops)
		{
			opQueue.Enqueue(op1);
			foreach (var op in ops)
				opQueue.Enqueue(op);

			actionStart = aInit;
			actionEnd = aFinal;
		}

		protected override void OnInit(TElement element)
		{
			foreach (var op in opQueue) op.InitUnchecked(element);
		}

		protected override void OnStart(TElement element)
		{
			actionStart?.Invoke(element);

			if (current == null)
			{
				if (opQueue.Count == 0) return;

				current = opQueue.Dequeue();
			}
		}

		protected override void OnUpdate(TElement entity, SAMTime gameTime, InputState istate)
		{
			if (current == null)
			{
				if (opQueue.Count == 0) { Finish(); return; }

				current = opQueue.Dequeue();
			}

			var r = current.UpdateUnchecked(entity, gameTime, istate);

			if (!r) current = null;
		}

		protected override void OnEnd(TElement element)
		{
			actionEnd?.Invoke(element);
		}

		protected override void OnAbort(TElement element)
		{
			if (current != null) current.ManualOnAbort(element);

			actionEnd?.Invoke(element);
		}

		public override string Name => $"SEQ [{string.Join(", ", opQueue.Select(o => o.Name))}]";
	}
}
