using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.HUD.Operations
{
	public abstract class HUDModuloElementOperation<TElement> : HUDElementOperation<TElement> where TElement : HUDElement
	{
		public bool Finished = false;

		private readonly float pTime;

		protected HUDModuloElementOperation(float periodicTime)
		{
			pTime = periodicTime;
		}

		public override bool Update(TElement entity, GameTime gameTime, InputState istate)
		{
			if (Finished) return false;

			OnProgress(entity, Lifetime % pTime, istate);

			return true;
		}

		protected abstract void OnProgress(TElement element, float value, InputState istate);
	}
}
