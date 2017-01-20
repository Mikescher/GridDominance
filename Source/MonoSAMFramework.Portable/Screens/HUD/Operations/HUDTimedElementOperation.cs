using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.HUD.Operations
{
	public abstract class HUDTimedElementOperation<TElement> : HUDElementOperation<TElement> where TElement : HUDElement
	{
		private readonly float length;

		public float Progress => Lifetime / length;

		protected HUDTimedElementOperation(float operationlength)
		{
			length = operationlength;

		}

		public override bool Update(TElement entity, SAMTime gameTime, InputState istate)
		{
			if (Lifetime >= length) return false;

			OnProgress(entity, Lifetime / length, istate);
			return true;
		}

		protected abstract void OnProgress(TElement element, float progress, InputState istate);
	}
}
