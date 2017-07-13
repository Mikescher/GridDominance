using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.HUD.Operations
{
	public class HUDDelayedElementOperation<TElement> : HUDElementOperation<TElement> where TElement : HUDElement
	{
		private readonly IHUDElementOperation _operation = null;

		private readonly float _offset;
		private bool started = false;

		public HUDDelayedElementOperation(IHUDElementOperation op, float delay)
		{
			_offset = delay;
			_operation = op;
		}

		protected override void OnStart(TElement element)
		{
			//
		}

		public override bool Update(TElement entity, SAMTime gameTime, InputState istate)
		{
			if (Lifetime < _offset) return true;

			if (!started)
			{
				_operation.OnStart(entity);
			}

			return _operation.Update(entity, gameTime, istate);
		}

		protected override void OnEnd(TElement element)
		{
			_operation.OnEnd(element);
		}

		public override string Name => _operation.Name + "#DELAY";
	}
}
