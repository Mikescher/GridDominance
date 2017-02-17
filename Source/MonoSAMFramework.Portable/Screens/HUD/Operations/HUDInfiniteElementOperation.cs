using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.HUD.Operations
{
	public abstract class HUDInfiniteElementOperation<TElement> : HUDElementOperation<TElement> where TElement : HUDElement
	{
		public bool Finished = false;

		protected HUDInfiniteElementOperation()
		{
			// NOP
		}

		public override bool Update(TElement entity, SAMTime gameTime, InputState istate)
		{
			if (Finished) return false;

			OnProgress(entity, Lifetime, istate);

			return true;
		}

		protected abstract void OnProgress(TElement element, float lifetime, InputState istate);
	}
}
