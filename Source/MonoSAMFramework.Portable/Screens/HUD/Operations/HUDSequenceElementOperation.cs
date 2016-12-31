using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;
using System.Collections.Generic;

namespace MonoSAMFramework.Portable.Screens.HUD.Operations
{
	public class HUDSequenceElementOperation<TElement> : HUDElementOperation<TElement> where TElement : HUDElement
	{
		private readonly Queue<IHUDElementOperation> opQueue = new Queue<IHUDElementOperation>();

		private IHUDElementOperation current = null;

		public HUDSequenceElementOperation(IHUDElementOperation op1, params IHUDElementOperation[] ops)
		{
			opQueue.Enqueue(op1);
			foreach (var op in ops)
				opQueue.Enqueue(op);
		}

		public override bool Update(TElement entity, GameTime gameTime, InputState istate)
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

		protected override void OnStart(TElement element)
		{
			if (current == null)
			{
				if (opQueue.Count == 0) return;

				current = opQueue.Dequeue();
				current.OnStart(element);
			}
		}

		protected override void OnEnd(TElement element)
		{
			//
		}
	}
}
