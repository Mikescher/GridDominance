using MonoSAMFramework.Portable.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.Screens.HUD.Operations
{
	public class HUDSequenceElementOperation<TElement> : HUDElementOperation<TElement> where TElement : HUDElement
	{
		private readonly Queue<IHUDElementOperation> opQueue = new Queue<IHUDElementOperation>();

		private IHUDElementOperation current = null;

		private readonly Action<TElement> actionStart;
		private readonly Action<TElement> actionEnd;

		public HUDSequenceElementOperation(IHUDElementOperation op1, params IHUDElementOperation[] ops)
		{
			opQueue.Enqueue(op1);
			foreach (var op in ops)
				opQueue.Enqueue(op);

			actionStart = null;
			actionEnd = null;
		}

		public HUDSequenceElementOperation(Action<TElement> aInit, Action<TElement> aFinal, IHUDElementOperation op1, params IHUDElementOperation[] ops)
		{
			opQueue.Enqueue(op1);
			foreach (var op in ops)
				opQueue.Enqueue(op);

			actionStart = aInit;
			actionEnd = aFinal;
		}

		protected override void OnStart(TElement element)
		{
			actionStart?.Invoke(element);

			if (current == null)
			{
				if (opQueue.Count == 0) return;

				current = opQueue.Dequeue();
				current.OnStart(element);
			}
		}

		public override bool Update(TElement entity, SAMTime gameTime, InputState istate)
		{
			if (current == null)
			{
				if (opQueue.Count == 0) return false;

				current = opQueue.Dequeue();
				current.OnStart(entity);
			}

			bool r = current.Update(entity, gameTime, istate);

			if (!r)
			{
				current.OnEnd(entity);
				current = null;
			}

			return true;
		}

		protected override void OnEnd(TElement element)
		{
			actionEnd?.Invoke(element);
		}

		public override string Name => $"[{string.Join(", ", opQueue.Select(o => o.Name))}]";
	}
}
