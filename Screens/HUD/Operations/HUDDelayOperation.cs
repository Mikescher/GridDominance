using MonoSAMFramework.Portable.Input;

namespace MonoSAMFramework.Portable.Screens.HUD.Operations
{
	public class HUDBaseDelayOperation<TElement> : HUDTimedElementOperation<TElement> where TElement : HUDElement
	{
		protected HUDBaseDelayOperation(float duration) : base(duration)
		{
		}

		protected override void OnStart(TElement element)
		{
			//
		}

		protected override void OnEnd(TElement element)
		{
			//
		}

		public override string Name => "Delay";

		protected override void OnProgress(TElement element, float progress, InputState istate)
		{
			//
		}
	}

	public class HUDDelayOperation : HUDBaseDelayOperation<HUDElement>
	{
		public HUDDelayOperation(float duration) : base(duration)
		{
		}
	}
}
