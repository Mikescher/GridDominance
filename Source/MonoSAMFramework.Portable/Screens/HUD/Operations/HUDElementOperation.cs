using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.HUD.Operations
{
	public abstract class HUDElementOperation<TElement> : IHUDElementOperation where TElement : HUDElement
	{
		protected float Lifetime { get; private set; }

		protected HUDElementOperation()
		{
			Lifetime = 0;
		}

		public bool Update(HUDElement element, SAMTime gameTime, InputState istate)
		{
			Lifetime += gameTime.ElapsedSeconds;

			return Update((TElement)element, gameTime, istate);
		}

		public void OnStart(HUDElement element)
		{
			OnStart((TElement)element);
		}

		public void OnEnd(HUDElement element)
		{
			OnEnd((TElement)element);
		}

		public abstract bool Update(TElement entity, SAMTime gameTime, InputState istate);

		protected abstract void OnStart(TElement element);
		protected abstract void OnEnd(TElement element);

	}
}
