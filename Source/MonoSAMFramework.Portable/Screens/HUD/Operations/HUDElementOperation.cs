using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
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

		public bool Update(HUDElement element, GameTime gameTime, InputState istate)
		{
			Lifetime += gameTime.GetElapsedSeconds();

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

		public abstract bool Update(TElement entity, GameTime gameTime, InputState istate);

		protected abstract void OnStart(TElement element);
		protected abstract void OnEnd(TElement element);

	}
}
